using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tcpServer;

namespace SocketSignalServer
{
    public class SocketListeningAndStoreWorker : IDisposable
    {
        /// <param name="liteDB_Worker"></param>
        /// <param name="Port"></param>
        /// <param name="EncodingName">ex.)"ASCII","UTF8"</param>
        public SocketListeningAndStoreWorker(LiteDB_Worker liteDB_Worker, int Port, string EncodingName = "UTF8")
        {
            this.liteDB_Worker = liteDB_Worker;
            tokenSource = new CancellationTokenSource();
            tcpSrv = new TcpSocketServer();
            
            LastCheckTime = DateTime.Now;
            tcpSrv.Start(Port, EncodingName);

        }

        public void Dispose()
        {
            if (tokenSource != null)
            {
                tcpSrv.Stop();
                
                tokenSource.Cancel();
                Thread.Sleep(100);
                if (worker != null) Task.WaitAll(new Task[] { worker });
                tokenSource.Dispose();
            }
        }

        public int Port { get { return tcpSrv.Port; } }
        public int WorkerLoopWait = 50;
        public DateTime LastCheckTime { get; private set; }
        public string TimeoutMessageParameter = "";
        public bool IsBusy = false;

        private LiteDB_Worker liteDB_Worker;
        private TcpSocketServer tcpSrv;
        private CancellationTokenSource tokenSource;
        private Task worker;

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
                Thread.Sleep(100);
                await worker;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                //Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                //Debug.WriteLine(ex.ToString());
            }
        }

        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if ((tcpSrv.LastReceiveTime - LastCheckTime).TotalSeconds > 0 && tcpSrv.ReceivedSocketQueue.Count > 0)
                {
                    addNewDataFromServerQueueToDataBase();
                    LastCheckTime = DateTime.Now;
                }
                Thread.Sleep(WorkerLoopWait);
            }
            token.ThrowIfCancellationRequested();
        }

        private void addNewDataFromServerQueueToDataBase()
        {
            string receivedSocketMessage = "";
            //============
            // ReadQueue and Entry dataBase file
            //============
            while (tcpSrv.ReceivedSocketQueue.TryDequeue(out receivedSocketMessage))
            {
                string[] cols = receivedSocketMessage.Split('\t');

                if (cols.Length >= 4)
                {
                    DateTime connectTime;
                    if (DateTime.TryParse(cols[0], out connectTime)) { connectTime = DateTime.Now; } else { continue; }

                    string clientName = cols[1];
                    string status = cols[2];
                    string message = cols[3];
                    string parameter = cols.Length > 4 ? cols[4] : "";
                    string checkStyle = cols.Length > 5 ? cols[5] : "Once";

                    SocketMessage socketMessage = new SocketMessage(connectTime, clientName, status, message, parameter, checkStyle);

                    try
                    {
                        liteDB_Worker.SaveData(socketMessage);
                    }
                    catch (Exception ex)
                    {
                        tcpSrv.ResponceMessage = "DatabaseLocked";
                        Debug.Write(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);
                        Debug.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}
