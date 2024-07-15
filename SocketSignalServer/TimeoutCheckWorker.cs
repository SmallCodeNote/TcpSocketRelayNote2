using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketSignalServer
{
    public class TimeoutCheckWorker : IDisposable
    {
        public TimeoutCheckWorker(LiteDB_Worker liteDB_Worker, NoticeTransmitter noticeTransmitter, List<ClientInfo> clientList, string TimeoutMessageParameter)
        {
            this.liteDB_Worker = liteDB_Worker;
            this.noticeTransmitter = noticeTransmitter;

            this.clientList = clientList;
            this.TimeoutMessageParameter = TimeoutMessageParameter;
            tokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                Thread.Sleep(100);
                if (worker != null) Task.WaitAll(new Task[] { worker });
                tokenSource.Dispose();
            }
        }

        LiteDB_Worker liteDB_Worker;
        List<ClientInfo> clientList;
        NoticeTransmitter noticeTransmitter;

        private Task worker;
        private CancellationTokenSource tokenSource;

        public bool IsBusy = false;
        public int Interval = 10000;
        public string TimeoutMessageParameter = "";

        private string _dbFilename;
        public string dbFilename
        {
            get { return _dbFilename; }
            set
            {
                if (Directory.Exists(value)) { _dbFilename = value; }
            }
        }

        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (true)
                {
                    SocketMessage[] dataset0, dataset1;

                    var col = liteDB_Worker.LoadData();

                    dataset0 = col
                            .Where(x => x.status != "Timeout")
                            .OrderByDescending(x => x.connectTime).ToArray();
                    dataset1 = col
                        .Where(x => x.status == "Timeout" && !x.check)
                        .OrderByDescending(x => x.connectTime).ToArray();

                    foreach (var clientTarget in clientList)
                    {
                        //MessageRecord from Client
                        var latestRecord = dataset0.Where(x => x.clientName == clientTarget.Name).FirstOrDefault();

                        //MessageRecord Timeout
                        var listedTimeoutMessage = dataset1.Where(x => x.clientName == clientTarget.Name).OrderByDescending(x => x.connectTime).ToList();
                        if (clientTarget.LastAccessTime == null) { clientTarget.LastAccessTime = DateTime.Now; };//First Time

                        //Acccess Time Update
                        if (latestRecord != null && clientTarget.LastAccessTime < latestRecord.connectTime)
                        {
                            clientTarget.LastAccessTime = latestRecord.connectTime;
                        };

                        bool flag1 = (DateTime.Now - clientTarget.LastAccessTime).TotalSeconds > clientTarget.TimeoutLength;
                        bool flag2 = listedTimeoutMessage.Count == 0;

                        if (clientTarget.TimeoutCheck && flag1 && flag2)
                        {
                            SocketMessage timeoutMessage = new SocketMessage(clientTarget.LastAccessTime, clientTarget.Name, "Timeout", clientTarget.TimeoutMessage, "", "Once");
                            timeoutMessage.parameter = TimeoutMessageParameter;
                            noticeTransmitter.AddNotice(clientTarget, timeoutMessage);
                            clientTarget.LastTimeoutDetectedTime = DateTime.Now;

                            Debug.WriteLine("Timeout\t" + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name +" "+ timeoutMessage.ToString());
                        }
                    }
                }

                Task.Delay(TimeSpan.FromMilliseconds(Interval), token).Wait();
            }
            token.ThrowIfCancellationRequested();
        }

        public bool Start()
        {
            if (worker == null)
            {
                var token = tokenSource.Token;
                worker = Task.Run(() => Worker(token),token);
                IsBusy = true;
                return true;
            }

            return false;
        }

        public async void Stop()
        {
            tokenSource.Cancel();
            await worker;
            IsBusy = false;
        }

    }
}
