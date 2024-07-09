﻿using System;
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
        public TimeoutCheckWorker(LiteDB_Worker liteDB_Worker, NoticeTransmitter noticeTransmitter, List<ClientData> clientList, string TimeoutMessageParameter)
        {
            this.liteDB_Worker = liteDB_Worker;
            this.noticeTransmitter = noticeTransmitter;

            this.clientList = clientList;
            this.TimeoutMessageParameter = TimeoutMessageParameter;
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
        List<ClientData> clientList;
        NoticeTransmitter noticeTransmitter;

        private Task worker;
        private CancellationTokenSource tokenSource;

        public bool IsBusy = false;
        public int Interval = 1000;
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
                if (File.Exists(dbFilename))
                {
                    SocketMessage[] dataset0;
                    SocketMessage[] dataset1;

                    Debug.WriteLine("OpenLiteDB\t" + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name);

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
                        var latestRecord = dataset0.Where(x => x.clientName == clientTarget.clientName).FirstOrDefault();

                        //MessageRecord Timeout
                        var listedTimeoutMessage = dataset1.Where(x => x.clientName == clientTarget.clientName).OrderByDescending(x => x.connectTime).ToList();
                        if (clientTarget.lastAccessTime == null) { clientTarget.lastAccessTime = DateTime.Now; };//First Time

                        //Acccess Time Update
                        if (latestRecord != null && clientTarget.lastAccessTime < latestRecord.connectTime)
                        {
                            clientTarget.lastAccessTime = latestRecord.connectTime;
                        };

                        bool flag1 = (DateTime.Now - clientTarget.lastAccessTime).TotalSeconds > clientTarget.timeoutLength;
                        bool flag2 = listedTimeoutMessage.Count == 0;

                        if (clientTarget.timeoutCheck && flag1 && flag2)
                        {
                            SocketMessage timeoutMessage = new SocketMessage(clientTarget.lastAccessTime, clientTarget.clientName, "Timeout", clientTarget.timeoutMessage, "", "Once");
                            timeoutMessage.parameter = TimeoutMessageParameter;
                            noticeTransmitter.AddNotice(clientTarget, timeoutMessage);
                            clientTarget.lastTimeoutDetectedTime = DateTime.Now;
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
                worker = Task.Run(() => Worker(tokenSource.Token));
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