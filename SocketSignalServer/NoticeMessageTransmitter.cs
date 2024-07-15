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
        //===================
        // Constructor
        //===================
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
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        //===================
        // Member variable
        //===================
        private Task worker;
        private CancellationTokenSource tokenSource;

        /// <summary> Received Notice Queue. </summary>
        public ConcurrentQueue<NoticeMessage> NoticeQueue;

        /// <summary> Running Notice.</summary>
        public ConcurrentDictionary<string, NoticeMessageHandling> NoticeRunningDictionary;

        /// <summary> FailLog / Address,Time</summary>
        public ConcurrentDictionary<string, NoticeMessage> FailLogDictionary;

        /// <summary> FailLog / Address,FailCount</summary>
        public ConcurrentDictionary<string, int> FailCountDictionary;

        /// <summary> CheckInterval(ms)</summary>
        public int Interval = 200;

        /// <summary> timeout length (sec.) </summary>
        public int HttpTimeout = 3;

        public bool voiceOFF = true;

        /// <summary> failover system class  true:Active / false:Standby </summary>
        public bool isActive = false;

        //===================
        // Member function
        //===================
        public bool AddNotice(NoticeMessage notice)
        {
            if (!NoticeRunningDictionary.ContainsKey(notice.Key))
            {
                NoticeQueue.Enqueue(notice);
                return true;
            };
            return false;
        }

        public bool AddNotice(ClientInfo targetClient, SocketMessage socketMessage)
        {
            bool result = true;
            foreach (var messageDestination in targetClient.MessageDestinationsList)
            {
                NoticeMessage item = new NoticeMessage(messageDestination.Address, socketMessage);
                result = result && AddNotice(item);
            }
            return result;
        }

        public void Start()
        {
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

        /// <summary>
        /// NoticeCheck
        /// </summary>
        public void Worker(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (NoticeRunningDictionary.Count > 0)
                    {
                        foreach (var item in NoticeRunningDictionary)
                        {
                            if (item.Value.FinishNotice)
                            {
                                int failCount = 0;

                                if (FailLogDictionary.ContainsKey(item.Value.Address))
                                {
                                    FailLogDictionary.TryRemove(item.Value.Address, out NoticeMessage b);
                                    FailCountDictionary.TryRemove(item.Value.Address, out failCount);
                                }

                                if (item.Value.isTimeout) {

                                    FailLogDictionary.TryAdd(item.Value.Address,item.Value.Notice);
                                    FailCountDictionary.TryAdd(item.Value.Address, failCount+1);
                                }

                               NoticeMessageHandling h;
                                if (NoticeRunningDictionary.TryRemove(item.Key, out h)) { h.Dispose(); };
                            }
                        }
                    }

                    while (NoticeQueue.Count > 0)
                    {
                        if (NoticeQueue.TryDequeue(out NoticeMessage b))
                        {
                            NoticeMessageHandling handling = new NoticeMessageHandling(b, voiceOFF, isActive, HttpTimeout);

                            if (NoticeRunningDictionary.TryAdd(b.Key, handling))
                            {
                                handling.StartNotice(token);
                            }
                        }
                    }

                    Task.Delay(TimeSpan.FromMilliseconds(Interval), token).Wait();
                }
                catch (Exception ex)
                {
                    Debug.Write(DateTime.Now.ToString("HH:mm:ss") + " " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " ");
                    Debug.WriteLine(ex.ToString());
                }
            }

            token.ThrowIfCancellationRequested();
        }
    }

    public class NoticeMessageHandling : IDisposable
    {
        //===================
        // Constructor
        //===================
        public NoticeMessageHandling(NoticeMessage notice, bool voiceOFF, bool isActive, int httpTimeout = 3)
        {
            this.notice = notice;
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, httpTimeout);

            this.voiceOFF = voiceOFF;
            this.isActive = isActive;

            Tasks = new List<Task>();
        }

        public void Dispose()
        {
            httpClient.Dispose();
            GC.SuppressFinalize(this);
        }

        //===================
        // Member variable
        //===================
        private NoticeMessage notice;
        private HttpClient httpClient;

        public bool FinishNotice
        {
            get
            {
                if (Tasks.Count == 0 || Tasks.Exists(task => !task.IsCompleted)) { return false; }
                else { return true; }
            }
        }
        public bool isTimeout = false;
        public NoticeMessage Notice { get { return notice; } }
        public string Address { get { return notice.Address; } }

        /// <summary>[Seconds]</summary>
        public int NoticeWorkerTimeout = 60;
        /// <summary>[Seconds]</summary>
        public int WaitSpeechFinishCheckStart = 10;

        public bool voiceOFF = true;

        /// <summary>[Milliseconds]</summary>
        private int threadSleepLength = 100;

        /// <summary> failover system class  true:Active / false:Standby </summary>
        public bool isActive = false;

        /// <summary> Http Timeout(sec.) </summary>
        public int HttpTimeout
        {
            get { return (int)httpClient.Timeout.TotalSeconds; }
            set { httpClient.Timeout = new TimeSpan(0, 0, value); }
        }

        public List<Task> Tasks;

        //===================
        // Member function
        //===================

        /// <summary>
        /// WaitNoticeFinishTask
        /// </summary>
        /// <returns></returns>
        public void StartNotice(CancellationToken token)
        {
            Tasks.Add(Task.Run(() =>
            {
                notice.SendNoticeTime = DateTime.Now;
                if (!WaitNoticeFinish(token, 0)) {
                    return;
                }
                SendNotice();
                WaitNoticeFinish(token, WaitSpeechFinishCheckStart);
            },token));
        }

        /// <summary> SendNoticeCommand </summary>
        /// <returns></returns>
        private string SendNotice()
        {
            string speech = (notice.Message != null && notice.Message.Length > 0) ? "speech=" + notice.Message : "";
            string parameter = (notice.Parameter != null && notice.Parameter.Length > 0) ? notice.Parameter : "";
            string separator = (speech.Length > 0 && parameter.Length > 0) ? "&" : "";

            if (speech.Length == 0 && parameter.Length == 0) { return ""; }
            if (voiceOFF || !isActive) { return ""; }

            string url = @"http://" + notice.Address + @"/api/control?" + parameter + separator + speech;

            try
            {
                string pageBody = httpClient.GetStringAsync(url).Result; Debug.Write(DateTime.Now.ToString("HH:mm:ss") + " " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " pageBody = " + pageBody.Replace("\r\n", " "));
                return pageBody;
            }
            catch (Exception ex)
            {
                Debug.Write(DateTime.Now.ToString("HH:mm:ss") + " " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " httpClient.GetStringAsync(url).Result Error");
                Debug.WriteLine(ex.ToString());
            }
            return "";
        }

        private bool WaitNoticeFinish(CancellationToken token, int WaitAskStart)
        {
            if (WaitAskStart > 0) Task.Delay(TimeSpan.FromSeconds(WaitAskStart), token).Wait();
            if (voiceOFF || !isActive) return true; //"voiceOFF WaitStop "
            if (token.IsCancellationRequested) return false;

            string url = @"http://" + notice.Address + @"/api/status?format=xml";

            bool waitContinue = true;
            DateTime startTime = DateTime.Now;

            do
            {
                if (token.IsCancellationRequested) { return false; }
                else
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        string pageBody = httpClient.GetStringAsync(url).Result;
                        doc.LoadXml(pageBody);

                        XmlNode soundNode = doc.SelectSingleNode("//sound[@name='SOUND']");
                        string soundValue = soundNode.Attributes["value"].Value;
                        waitContinue = soundValue != "0";
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("WaitEnd_ERROR " + DateTime.Now.ToString("HH:mm:ss") + " " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]]" + ex.ToString());
                        Task.Delay(httpClient.Timeout, token).Wait();
                    }
                }

                if ((DateTime.Now - startTime).TotalSeconds > NoticeWorkerTimeout)
                {
                    Debug.WriteLine("WaitEnd_TimeOut " + DateTime.Now.ToString("HH:mm:ss") + " " + GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]]");
                    isTimeout = true;
                    return false;
                }

                Task.Delay(TimeSpan.FromMilliseconds(threadSleepLength), token).Wait();

            } while (waitContinue);

            return true;
        }
    }

    public class NoticeMessage
    {
        public string Address;
        public string Message;
        public string Parameter;
        public DateTime KeyTime;
        public DateTime SendNoticeTime;

        public string Key
        {
            get { return this.Address + "_" + KeyTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "_" + Message; }
        }

        //===================
        // Member function
        //===================
        public NoticeMessage(string address, SocketMessage socket)
        {
            this.Address = address;
            this.Message = socket.message;
            this.Parameter = socket.parameter;
            this.KeyTime = socket.connectTime;
            this.SendNoticeTime = socket.connectTime;
        }

        public static bool operator ==(NoticeMessage c1, NoticeMessage c2)
        {
            return c1.Key == c2.Key;
        }

        public static bool operator !=(NoticeMessage c1, NoticeMessage c2)
        {
            return c1.Key != c2.Key;

        }

    }

}
