using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketSignalServer
{
    public class DebugFilePathWorker : IDisposable
    {
        private Task worker;
        private CancellationTokenSource tokenSource;
        private readonly object startStopLock = new object();

        private string _outDirPath = "";
        public int IntervalSec = 1;

        public string outDirPath
        {
            get { return _outDirPath; }
            set
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(value) && Directory.Exists(value))
                    {
                        _outDirPath = value;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::outDirPath setter error: {ex}");
                }
            }
        }

        public DebugFilePathWorker(string outDirPath)
        {
            tokenSource = new CancellationTokenSource();
            Start(outDirPath);
        }

        public void Dispose()
        {
            try
            {
                Stop();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::Dispose exception: {ex}");
            }

            tokenSource?.Dispose();
        }

        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    DebugOutFilenameReset(_outDirPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::Worker DebugOutFilenameReset error: {ex}");
                }

                try
                {
                    Task.Delay(IntervalSec * 1000, token).Wait();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::Worker Delay error: {ex}");
                }
            }
        }

        public void Start(string outDirPath)
        {
            lock (startStopLock)
            {
                this.outDirPath = outDirPath;

                if (worker != null && !worker.IsCompleted)
                    return;

                if (tokenSource.IsCancellationRequested)
                {
                    tokenSource.Dispose();
                    tokenSource = new CancellationTokenSource();
                }

                var token = tokenSource.Token;

                worker = Task.Run(() =>
                {
                    try { Worker(token); }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{GetType().Name}::Start Worker exception: {ex}");
                    }
                }, token);
            }
        }

        public void Stop()
        {
            lock (startStopLock)
            {
                try
                {
                    tokenSource.Cancel();

                    if (worker != null)
                    {
                        worker.Wait(1000);
                    }
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        Debug.WriteLine($"{GetType().Name}::Stop inner exception: {ex}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::Stop exception: {ex}");
                }
            }
        }

        private void DebugOutFilenameReset(string targetDir)
        {
            string outFilename = "";

            try
            {
                if (string.IsNullOrWhiteSpace(targetDir))
                    return;

                if (targetDir == "{ExecutablePath}")
                {
                    targetDir = Path.GetDirectoryName(Application.ExecutablePath);
                    if (string.IsNullOrEmpty(targetDir))
                        return;
                }

                if (!Directory.Exists(targetDir))
                    return;

                var now = DateTime.Now;
                string yyyy = now.ToString("yyyy");
                string yyyyMM = now.ToString("yyyyMM");
                string yyyyMMdd = now.ToString("yyyyMMdd");
                string yyyyMMdd_HHmm0 = now.ToString("yyyyMMdd_HHmm0") + ".txt";

                outFilename = Path.Combine(targetDir, yyyy, yyyyMM, yyyyMMdd, yyyyMMdd_HHmm0);

                var dir = Path.GetDirectoryName(outFilename);
                if (string.IsNullOrEmpty(dir))
                    return;

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var dtl = Debug.Listeners["Default"] as DefaultTraceListener;
                if (dtl != null && dtl.LogFileName != outFilename)
                {
                    dtl.LogFileName = outFilename;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::DebugOutFilenameReset filename={outFilename} error={ex}");
            }
        }
    }
}