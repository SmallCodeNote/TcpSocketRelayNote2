using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace tcpServer
{
    class TcpSocketServer
    {
        //===================
        // Constructor
        //===================
        /// <param name="encodingName">ex.)"ASCII","UTF8"</param>
        public TcpSocketServer(int port, string encodingName = "ASCII")
        {
            LastReceiveTime = DateTime.Now;
            ReceivedSocketQueue = new ConcurrentQueue<string>();
            this.Port = port;
            this.EncodingName = encodingName;
            IsBusy = false;
        }

        //===================
        // Member variable
        //===================
        public DateTime LastReceiveTime;
        public int Port { get; private set; }

        /// <summary>ex.)"ASCII","UTF8"</summary>
        public string EncodingName = "ASCII";

        /// <summary> Received TcpSocket Queue. </summary>
        public ConcurrentQueue<string> ReceivedSocketQueue;

        public int ReceivedSocketQueueMaxSize = 1024;
        public int ReceiveBufferSize = 1024;
        public string ResponceMessage = "";

        private Task ListeningTask;
        private static CancellationTokenSource tokenSource;
        private static CancellationToken token;
        public bool IsBusy { get; private set; }

        private TcpListener tcpListener;

        //===================
        // Member function
        //===================
        public void Start()
        {
            if (ListeningTask == null || ListeningTask.IsCompleted)
            {
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                ListeningTask = Task.Run(() => { ListeningMessage(); }, token);
                IsBusy = true;
            }
        }

        public void Stop()
        {
            if (ListeningTask != null && !ListeningTask.IsCompleted)
            {
                tokenSource.Cancel();
                Thread.Sleep(100);
                ListeningTask.Wait();
            }
        }

        public void ListeningMessage()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            tcpListener = new TcpListener(localEndPoint);
            tcpListener.Start(); IsBusy = true;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    removeOverflowQueueFromReceivedSocketQueue();

                    using (var tcpClient = tcpListener.AcceptTcpClient())
                    {
                        Debug.WriteLine($"tcpListener.AcceptTcpClient()");
                        string ReceivedMessage = getReceivedMessage(tcpClient);
                        LastReceiveTime = DateTime.Now;
                        ReceivedSocketQueue.Enqueue(LastReceiveTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\t" + ReceivedMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
            }

            tcpListener.Stop(); IsBusy = false;
            tokenSource.Dispose();
        }

        private void removeOverflowQueueFromReceivedSocketQueue()
        {
            while (ReceivedSocketQueue.Count >= ReceivedSocketQueueMaxSize)
            {
                string b = ""; ReceivedSocketQueue.TryDequeue(out b);
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " removeOverflowQueueFromReceivedSocketQueue");
            }
        }
        private string getHTTPResponceStatusAndHeader()
        {
            List<string> Lines = new List<string>();
            Lines.Add("HTTP/1.1 200 OK");
            Lines.Add("Server: TcpSocketServerClass");
            Lines.Add("Date: " + DateTime.UtcNow.ToString("R"));
            Lines.Add("Content-Length: " + ResponceMessage.Length);
            Lines.Add("Content-Type: text/xml; charset=UTF-8");

            string responceString = String.Join("\r\n", Lines) + "\r\n\r\n";

            return responceString;

        }

        public string getReceivedMessage(TcpClient tcpClient)
        {
            byte[] buffer = new byte[ReceiveBufferSize];
            string ReceivedMessage = "";

            System.Text.Encoding Encoder = System.Text.Encoding.ASCII;
            if (EncodingName == "UTF8") Encoder = System.Text.Encoding.UTF8;

            try
            {
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    do
                    {
                        int byteSize = stream.Read(buffer, 0, buffer.Length);
                        ReceivedMessage += Encoder.GetString(buffer, 0, byteSize);

                    } while (stream.DataAvailable);

                    //Responce for client


                    var response = DateTime.Now.ToString("HH:mm:ss.fff") + " received : " + ReceivedMessage;

                    string[] Cols = ReceivedMessage.Replace("\r\n", "\n").Split('\n');

                    if (Cols.Length > 0)
                    {
                        string[] ColsB = Cols[0].Split(' ');

                        if (ColsB.Length > 0 && ColsB[ColsB.Length - 1].IndexOf("HTTP") >= 0)
                        {
                            response = getHTTPResponceStatusAndHeader() + ResponceMessage;
                        }
                    }

                    buffer = Encoder.GetBytes(response);
                    stream.Write(buffer, 0, buffer.Length);
                    Debug.WriteLine($"Response : {response}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
            }

            return ReceivedMessage;
        }

    }
}
