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
    public class DebugFilePathWorker : IDisposable
    {
        private Task worker;
        private CancellationTokenSource tokenSource;

        private string _outDirPath = "";
        public int IntervalSec = 1;

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
            tokenSource = new CancellationTokenSource();
            Start(outDirPath);
        }

        public void Dispose()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                DebugOutFilenameReset(outDirPath);
                Task.Delay(TimeSpan.FromSeconds(IntervalSec), token).Wait();
            }
        }

        public void Start(string outDirPath)
        {
            this.outDirPath = outDirPath;

            if (worker == null || worker.IsCompleted)
            {
                var token = tokenSource.Token;
                worker = Task.Run(() => { try { Worker(token); } catch { } }, token);
            }
        }

        public void Stop()
        {
            tokenSource.Cancel();
            Thread.Sleep(100);
            worker.Wait();
        }

        public void DebugOutFilenameReset(string targetDir)
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
