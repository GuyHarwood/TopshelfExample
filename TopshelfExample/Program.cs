using System;
using System.Configuration;
using Topshelf;

namespace TopshelfExample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var folder = ConfigurationManager.AppSettings["Files.Location"];

            if (string.IsNullOrWhiteSpace(folder))
            {
                throw new Exception("Files.Location is requred");
            }

            HostFactory.Run(x => 
            {
                x.Service<FileService>(s => 
                {
                    s.ConstructUsing(name => new FileService(folder));
                    s.WhenStarted(tc => tc.Start()); 
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem(); 

                x.SetDescription("Processes files"); 
                x.SetDisplayName("File Processor"); 
                x.SetServiceName("FileProcessorSvc"); 
            });
        }
    }
}