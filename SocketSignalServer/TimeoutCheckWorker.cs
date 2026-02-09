using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SocketSignalServer
{
    public class TimeoutCheckWorker : IDisposable
    {
        private readonly LiteDB_Worker liteDB_Worker;
        private readonly List<ClientInfo> clientList;
        private readonly NoticeTransmitter noticeTransmitter;

        private Task worker;
        private CancellationTokenSource tokenSource;

        public bool IsBusy { get; private set; }
        public int Interval = 10000;
        public string TimeoutMessageParameter = "";

        private string _dbFilename;
        public string dbFilename
        {
            get { return _dbFilename; }
            set
            {
                if (Directory.Exists(value))
                {
                    _dbFilename = value;
                }
            }
        }

        public TimeoutCheckWorker(
            LiteDB_Worker liteDB_Worker,
            NoticeTransmitter noticeTransmitter,
            List<ClientInfo> clientList,
            string TimeoutMessageParameter)
        {
            this.liteDB_Worker = liteDB_Worker;
            this.noticeTransmitter = noticeTransmitter;
            this.clientList = clientList;
            this.TimeoutMessageParameter = TimeoutMessageParameter;

            tokenSource = new CancellationTokenSource();
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

        // -----------------------------
        // Worker
        // -----------------------------
        private void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // skip null file
                    if (liteDB_Worker == null)
                    {
                        SafeDelay(token);
                        continue;
                    }

                    var col = liteDB_Worker.LoadData();
                    if (col == null || col.Count == 0)
                    {
                        SafeDelay(token);
                        continue;
                    }

                    var ordered = col.OrderByDescending(x => x.connectTime).ToArray();

                    var dataset0 = ordered.Where(x => x.status != "Timeout").ToArray();
                    var dataset1 = ordered.Where(x => x.status == "Timeout" && !x.check).ToArray();


                    var latestMap = dataset0
                        .GroupBy(x => x.clientName)
                        .ToDictionary(g => g.Key, g => g.First());


                    var timeoutMap = dataset1
                        .GroupBy(x => x.clientName)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var clientTarget in clientList)
                    {
                        if (clientTarget == null) continue;

                        // first access
                        if (clientTarget.LastAccessTime == null)
                        {
                            clientTarget.LastAccessTime = DateTime.Now;
                        }

                        // update access
                        SocketMessage latestRecord;
                        if (latestMap.TryGetValue(clientTarget.Name, out latestRecord))
                        {
                            if (clientTarget.LastAccessTime < latestRecord.connectTime)
                            {
                                clientTarget.LastAccessTime = latestRecord.connectTime;
                            }
                        }

                        // collect new Timeout message
                        List<SocketMessage> timeoutList;
                        bool hasTimeoutMessage = timeoutMap.TryGetValue(clientTarget.Name, out timeoutList)
                                                 && timeoutList.Count > 0;

                        bool isTimeout = (DateTime.Now - clientTarget.LastAccessTime).TotalSeconds
                                         > clientTarget.TimeoutLength;

                        if (clientTarget.TimeoutCheck && isTimeout && !hasTimeoutMessage)
                        {
                            var timeoutMessage = new SocketMessage(
                                clientTarget.LastAccessTime,
                                clientTarget.Name,
                                "Timeout",
                                clientTarget.TimeoutMessage,
                                "",
                                "Once");

                            timeoutMessage.parameter = TimeoutMessageParameter;

                            try
                            {
                                noticeTransmitter?.AddNotice(clientTarget, timeoutMessage);
                            }
                            catch (Exception exNotice)
                            {
                                Debug.WriteLine($"{GetType().Name}::Notice exception: {exNotice}");
                            }

                            clientTarget.LastTimeoutDetectedTime = DateTime.Now;

                            try
                            {
                                liteDB_Worker.SaveData(timeoutMessage);
                            }
                            catch (Exception exSave)
                            {
                                Debug.WriteLine($"{GetType().Name}::SaveData exception: {exSave}");
                            }

                            Debug.WriteLine($"Timeout\t{GetType().Name}::{nameof(Worker)} {timeoutMessage}");
                        }
                    }

                    SafeDelay(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::{nameof(Worker)} exception: {ex}");
                    SafeDelay(token);
                }
            }
        }

        private void SafeDelay(CancellationToken token)
        {
            try
            {
                Task.Delay(Interval, token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                
            }
        }

        // -----------------------------
        // Start / Stop
        // -----------------------------
        public bool Start()
        {
            if (worker != null && !worker.IsCompleted)
            {
                return false;
            }

            var token = tokenSource.Token;
            worker = Task.Run(() => Worker(token), token);
            IsBusy = true;
            return true;
        }

        public void Stop()
        {
            if (worker == null) return;

            try
            {
                tokenSource.Cancel();
                worker.Wait(2000);
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
            finally
            {
                IsBusy = false;
            }
        }
    }
}
//2026.02.05