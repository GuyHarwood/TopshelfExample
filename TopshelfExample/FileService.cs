using System.IO;
using System.Threading;
using log4net;

namespace TopshelfExample
{
    public class FileService
    {
        private readonly DirectoryInfo _directoryInfo;
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileService));
        private readonly AutoResetEvent _stopRequest = new AutoResetEvent(false);
        private Thread _worker;

        public FileService(string path)
        {
            if (!Directory.Exists(path))
            {
                _directoryInfo = Directory.CreateDirectory(path);
            }
            else
                _directoryInfo = new DirectoryInfo(path);
        }

        private void DoWork(object arg)
        {
            // Worker thread loop
            for (;;)
            {
                // Run this code once every 10 seconds or stop right away if the service 
                // is stopped
                if (_stopRequest.WaitOne(3000)) return;
                // Do work...
                var files = _directoryInfo.GetFiles("*");

                foreach (var file in files)
                {
                    if (!file.Extension.Equals(".processed"))
                    {
                        Log.InfoFormat("Processing {0}", file.FullName);
                        file.MoveTo(file.Directory.FullName + "\\" + string.Format("{0}.processed",file.Name));
                    }
                }
            }
        }

        public void Stop()
        {
            // Signal worker to stop and wait until it does
            _stopRequest.Set();
            _worker.Join();
        }

        public void Start()
        {
            // Start the worker thread
            _worker = new Thread(DoWork);
            _worker.Start();
        }
    }
}