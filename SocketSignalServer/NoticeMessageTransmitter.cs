using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Xml;

namespace SocketSignalServer
{
    public class NoticeTransmitter : IDisposable
    {
        public NoticeTransmitter(bool voiceOFF = true, bool isActive = false)
        {
            NoticeQueue = new ConcurrentQueue<NoticeMessage>();
            NoticeRunningDictionary = new ConcurrentDictionary<string, NoticeMessageHandling>();
            FailLogDictionary = new ConcurrentDictionary<string, NoticeMessage>();
            FailCountDictionary = new ConcurrentDictionary<string, int>();

            this.voiceOFF = voiceOFF;
            this.isActive = isActive;

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

        private Task worker;
        private CancellationTokenSource tokenSource;

        public ConcurrentQueue<NoticeMessage> NoticeQueue;
        public ConcurrentDictionary<string, NoticeMessageHandling> NoticeRunningDictionary;
        public ConcurrentDictionary<string, NoticeMessage> FailLogDictionary;
        public ConcurrentDictionary<string, int> FailCountDictionary;

        public int Interval = 200;
        public int HttpTimeout = 3;
        public bool voiceOFF = true;
        public bool isActive = false;

        // -----------------------------
        // AddNotice
        // -----------------------------
        public bool AddNotice(NoticeMessage notice)
        {
            if (notice == null) return false;

            // skip running key
            if (!NoticeRunningDictionary.ContainsKey(notice.Key))
            {
                NoticeQueue.Enqueue(notice);
                return true;
            }
            return false;
        }

        public bool AddNotice(ClientInfo targetClient, SocketMessage socketMessage)
        {
            if (targetClient == null || socketMessage == null) return false;

            bool result = true;

            foreach (var dest in targetClient.MessageDestinationsList)
            {
                if (dest == null) continue;

                var item = new NoticeMessage(dest.Address, socketMessage);
                result = result && AddNotice(item);
            }

            return result;
        }

        // -----------------------------
        // Start / Stop
        // -----------------------------
        public void Start()
        {
            if (worker == null || worker.IsCompleted)
            {
                var token = tokenSource.Token;
                worker = Task.Run(() =>
                {
                    try { Worker(token); }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{GetType().Name}::Worker fatal exception: {ex}");
                    }
                }, token);
            }
        }

        public void Stop()
        {
            try
            {
                tokenSource.Cancel();
                if (worker != null)
                {
                    worker.Wait(2000);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::Stop exception: {ex}");
            }
        }

        // -----------------------------
        // Worker
        // -----------------------------
        public void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // ---- Running Notice check ----
                    if (!NoticeRunningDictionary.IsEmpty)
                    {
                        foreach (var kv in NoticeRunningDictionary.ToArray())
                        {
                            var handling = kv.Value;

                            if (handling.FinishNotice)
                            {
                                int failCount = 0;

                                // FailLog update
                                if (handling.isTimeout)
                                {
                                    FailLogDictionary[kv.Key] = handling.Notice;
                                    FailCountDictionary.AddOrUpdate(kv.Key, 1, (k, v) => v + 1);
                                }
                                else
                                {
                                    FailLogDictionary.TryRemove(kv.Key, out _);
                                    FailCountDictionary.TryRemove(kv.Key, out failCount);
                                }

                                // Remove from running
                                NoticeRunningDictionary.TryRemove(kv.Key, out var removed);
                                removed?.Dispose();
                            }
                        }
                    }

                    // ---- NoticeQueue process ----
                    NoticeMessage msg;
                    while (NoticeQueue.TryDequeue(out msg))
                    {
                        var handling = new NoticeMessageHandling(msg, voiceOFF, isActive, HttpTimeout);

                        if (NoticeRunningDictionary.TryAdd(msg.Key, handling))
                        {
                            handling.StartNotice(token);
                        }
                        else
                        {
                            handling.Dispose();
                        }
                    }

                    // ---- Interval ----
                    Task.Delay(Interval, token).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::Worker exception: {ex}");
                }
            }
        }
    }

    // ============================================================
    // NoticeMessageHandling
    // ============================================================
    public class NoticeMessageHandling : IDisposable
    {
        private readonly NoticeMessage notice;
        private readonly HttpClient httpClient;

        public bool isTimeout = false;
        public bool voiceOFF = true;
        public bool isActive = false;

        public int NoticeWorkerTimeout = 60;
        public int WaitSpeechFinishCheckStart = 10;
        private int threadSleepLength = 1000;

        public List<Task> Tasks;

        public NoticeMessageHandling(NoticeMessage notice, bool voiceOFF, bool isActive, int httpTimeout = 3)
        {
            this.notice = notice;
            this.voiceOFF = voiceOFF;
            this.isActive = isActive;

            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(httpTimeout);

            Tasks = new List<Task>();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public bool FinishNotice
        {
            get
            {
                return Tasks.Count > 0 && Tasks.TrueForAll(t => t.IsCompleted);
            }
        }

        public NoticeMessage Notice => notice;
        public string Address => notice.Address;

        // -----------------------------
        // StartNotice
        // -----------------------------
        public void StartNotice(CancellationToken token)
        {
            Tasks.Add(Task.Run(() =>
            {
                notice.SendNoticeTime = DateTime.Now;

                if (!WaitNoticeFinish(token, 0))
                    return;

                SendNotice();

                WaitNoticeFinish(token, WaitSpeechFinishCheckStart);

            }, token));
        }

        // -----------------------------
        // SendNotice
        // -----------------------------
        private string SendNotice()
        {
            if (voiceOFF || !isActive)
                return "";

            string speech = string.IsNullOrEmpty(notice.Message) ? "" : "speech=" + notice.Message;
            string parameter = string.IsNullOrEmpty(notice.Parameter) ? "" : notice.Parameter;
            string separator = (speech.Length > 0 && parameter.Length > 0) ? "&" : "";

            if (speech.Length == 0 && parameter.Length == 0)
                return "";

            string url = $"http://{notice.Address}/api/control?{parameter}{separator}{speech}";

            try
            {
                return httpClient.GetStringAsync(url).Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::SendNotice error: {ex}");
                return "";
            }
        }

        // -----------------------------
        // WaitNoticeFinish
        // -----------------------------
        private bool WaitNoticeFinish(CancellationToken token, int waitStart)
        {
            try
            {
                if (waitStart > 0)
                    Task.Delay(TimeSpan.FromSeconds(waitStart), token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            if (voiceOFF || !isActive)
                return true;

            string url = $"http://{notice.Address}/api/status?format=xml";
            DateTime start = DateTime.Now;

            while (true)
            {
                if (token.IsCancellationRequested)
                    return false;

                try
                {
                    string xml = httpClient.GetStringAsync(url).Result;

                    var doc = new XmlDocument();
                    doc.LoadXml(xml);

                    var soundNode = doc.SelectSingleNode("//sound[@name='SOUND']");
                    if (soundNode != null)
                    {
                        string value = soundNode.Attributes["value"].Value;
                        if (value == "0")
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::WaitNoticeFinish error: {ex}");
                    SafeDelay(token, httpClient.Timeout);
                }

                if ((DateTime.Now - start).TotalSeconds > NoticeWorkerTimeout)
                {
                    isTimeout = true;
                    return false;
                }

                SafeDelay(token, TimeSpan.FromMilliseconds(threadSleepLength));
            }
        }

        private void SafeDelay(CancellationToken token, TimeSpan span)
        {
            try
            {
                Task.Delay(span, token).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    // ============================================================
    // NoticeMessage
    // ============================================================
    public class NoticeMessage
    {
        public string Address;
        public string Message;
        public string Parameter;
        public DateTime KeyTime;
        public DateTime SendNoticeTime;

        public string Key => $"{Address}_{KeyTime:yyyy/MM/dd HH:mm:ss.fff}_{Message}";

        public NoticeMessage(string address, SocketMessage socket)
        {
            Address = address;
            Message = socket.message;
            Parameter = socket.parameter;
            KeyTime = socket.connectTime;
            SendNoticeTime = socket.connectTime;
        }
    }
}
//2026.2.1