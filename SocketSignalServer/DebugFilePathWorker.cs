using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketSignalServer
{
    public class DebugFilePathWorker
    {
        private Task worker;
        private bool _WorkerRun = true;
        public int IntervalSec = 1;

        private string _outDirPath = "";
        public string outDirPath
        {
            get { return _outDirPath; }
            set
            {
                if (Directory.Exists(value)) { _outDirPath = value; }
            }
        }

       

        public DebugFilePathWorker(string outDirPath)
        {
            Start(outDirPath);
        }

        private void Worker()
        {
            while (_WorkerRun)
            {
                DebugOutFilenameReset(outDirPath);
                Thread.Sleep(IntervalSec * 1000);
            }
        }

        public void Start(string outDirPath)
        {
            this.outDirPath = outDirPath;

            if (!_WorkerRun || worker == null || worker.IsCompleted)
            {
                _WorkerRun = true;
                worker = Task.Run(() => Worker());
            }
        }

        public void Stop()
        {
            _WorkerRun = false;
            Task.WaitAll(new Task[] { worker });
        }

        private void DebugOutFilenameReset(string targetDir)
        {
            string outFilename = "";
            try
            {
                if (targetDir == "{ExecutablePath}") { targetDir = Path.GetDirectoryName(Application.ExecutablePath); }
                if (Directory.Exists(targetDir))
                {
                    outFilename = Path.Combine(targetDir, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("yyyyMM"), DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd_HHmm").Substring(0, 12) + "0.txt");
                    if (!Directory.Exists(Path.GetDirectoryName(outFilename))) { Directory.CreateDirectory(Path.GetDirectoryName(outFilename)); };

                    DefaultTraceListener dtl = (DefaultTraceListener)Debug.Listeners["Default"];
                    if (dtl.LogFileName != outFilename) { dtl.LogFileName = outFilename; };
                }
            }
            catch (Exception ex)
            {
                Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " filename " + outFilename);
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
