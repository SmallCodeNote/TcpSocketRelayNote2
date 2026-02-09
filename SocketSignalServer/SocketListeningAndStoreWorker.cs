using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using tcpServer;

namespace SocketSignalServer
{
    public class SocketListeningAndStoreWorker : IDisposable
    {
        public int Port { get { return tcpSrv.Port; } }
        public int WorkerLoopWait = 50;
        public DateTime LastCheckTime { get; private set; }
        public bool IsBusy = false;

        private LiteDB_Worker liteDB_Worker;
        private TcpSocketServer tcpSrv;
        private CancellationTokenSource tokenSource;
        private Task worker;

        private BlockingCollection<SocketMessage> dbQueue = new BlockingCollection<SocketMessage>();
        private Task dbWriterTask;

        /// <summary> Database write batch queue size </summary>
        public int BatchSize = 50;

        /// <summary> Database write batch queue wait(ms) </summary>
        public int BatchWaitMs = 200;


        public SocketListeningAndStoreWorker(LiteDB_Worker liteDB_Worker, int Port, string EncodingName = "UTF8")
        {
            this.liteDB_Worker = liteDB_Worker;
            tokenSource = new CancellationTokenSource();
            tcpSrv = new TcpSocketServer();

            LastCheckTime = DateTime.Now;
            tcpSrv.Start(Port, EncodingName);

            dbWriterTask = Task.Run(() => DbWriterLoop(tokenSource.Token));
        }

        public void Dispose()
        {
            try
            {
                tcpSrv.Stop();
                tokenSource.Cancel();

                dbQueue.CompleteAdding();
                dbWriterTask?.Wait();

                if (worker != null)
                    worker.Wait();
            }
            catch { }
            finally
            {
                tokenSource.Dispose();
            }
        }

        public void Start()
        {
            if (worker == null)
            {
                var token = tokenSource.Token;
                worker = Task.Run(() => Worker(token), token);
                IsBusy = true;
            }
        }

        public async void Stop()
        {
            try
            {
                tokenSource.Cancel();
                await worker;
                IsBusy = false;
            }
            catch { }
        }

        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if ((tcpSrv.LastReceiveTime - LastCheckTime).TotalSeconds > 0 &&
                    tcpSrv.ReceivedSocketQueue.Count > 0)
                {
                    addNewDataFromServerQueueToDbQueue();
                    LastCheckTime = DateTime.Now;
                }

                Thread.Sleep(WorkerLoopWait);
            }
        }

        private void DbWriterLoop(CancellationToken token)
        {
            var batch = new System.Collections.Generic.List<SocketMessage>(BatchSize);

            while (!dbQueue.IsCompleted && !token.IsCancellationRequested)
            {
                try
                {
                    SocketMessage msg;

                    if (!dbQueue.TryTake(out msg, BatchWaitMs, token))
                    {
                        FlushBatch(batch);
                        continue;
                    }

                    batch.Add(msg);


                    while (batch.Count < BatchSize && dbQueue.TryTake(out msg))
                    {
                        batch.Add(msg);
                    }

                    FlushBatch(batch);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("DbWriterLoop error: " + ex);
                }
            }

            FlushBatch(batch);
        }

        private void FlushBatch(System.Collections.Generic.List<SocketMessage> batch)
        {
            if (batch.Count == 0) return;

            try
            {
                foreach (var msg in batch)
                {
                    try
                    {
                        liteDB_Worker.SaveData(msg);
                    }
                    catch (Exception ex)
                    {
                        tcpSrv.ResponceMessage = "DatabaseLocked";
                        Debug.WriteLine("FlushBatch SaveData error: " + ex);
                    }
                }
            }
            finally
            {
                batch.Clear();
            }
        }

        private void addNewDataFromServerQueueToDbQueue()
        {
            string receivedSocketMessage = "";

            while (tcpSrv.ReceivedSocketQueue.TryDequeue(out receivedSocketMessage))
            {
                string[] cols = receivedSocketMessage.Split('\t');
                if (cols.Length < 4) continue;

                DateTime connectTime;
                if (!DateTime.TryParse(cols[0], out connectTime)) continue;

                string clientName = cols[1];
                string status = cols[2];
                string message = cols[3];
                string parameter = cols.Length > 4 ? cols[4] : "";
                string checkStyle = cols.Length > 5 ? cols[5] : "Once";

                var socketMessage = new SocketMessage(connectTime, clientName, status, message, parameter, checkStyle);

                dbQueue.Add(socketMessage);
            }
        }
    }
}