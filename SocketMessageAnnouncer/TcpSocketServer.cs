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
        public TcpSocketServer()
        {
            LastReceiveTime = DateTime.Now;
            ReceivedSocketQueue = new ConcurrentQueue<string>();

        }

        //===================
        // Member variable
        //===================
        public DateTime LastReceiveTime;

        /// <summary>
        /// Received TcpSocket Queue.
        /// </summary>
        public ConcurrentQueue<string> ReceivedSocketQueue;
        public int _ReceivedSocketQueueMaxSize = 1024;
        public string ResponceMessage = "";
        public int _bufferSize = 1024;
        
        private Task ListeningTask;
        private static CancellationTokenSource cts;
        private static CancellationToken token;
        public bool ListeningRun = false;


        //===================
        // Member function
        //===================
        public void StartListening(int port, string encoding = "ASCII")
        {
            cts = new CancellationTokenSource();
            token = cts.Token;
            ListeningTask = Task.Run(() =>
             {
                 Listening(port, encoding);

             }, token);
        }

        public void StopListening()
        {
            cts.Cancel();
            ListeningTask.Wait();
            return;
        }

        public void Listening(int port, string encoding = "ASCII")
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            var tcpServer = new TcpListener(localEndPoint);

            try
            {
                tcpServer.Start(); ListeningRun = true;
                while (!token.IsCancellationRequested)
                {
                    using (var tcpClient = tcpServer.AcceptTcpClient())
                    {
                        var request = Receive(tcpClient, encoding);

                        if (ReceivedSocketQueue.Count >= _ReceivedSocketQueueMaxSize) { string b = ""; ReceivedSocketQueue.TryDequeue(out b); }
                        LastReceiveTime = DateTime.Now;
                        ReceivedSocketQueue.Enqueue(LastReceiveTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\t" + request);
                    }
                }

                tcpServer.Stop(); ListeningRun = false;
                cts.Dispose();

                return;
            }
            catch (Exception ex)
            {
                ListeningRun = false;
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
            }
            return;
        }

        public string Receive(TcpClient tcpClient, string encoding = "ASCII")
        {
            byte[] buffer = new byte[_bufferSize];
            string request = "";

            try
            {
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    if (encoding == "ASCII")
                    {
                        do
                        {
                            int byteSize = stream.Read(buffer, 0, buffer.Length);
                            request += Encoding.ASCII.GetString(buffer, 0, byteSize);
                        }
                        while (stream.DataAvailable);

                        //Responce code for client
                        var response = "received : " + request;
                        if (ResponceMessage.Length > 0) { response = ResponceMessage; }
                        buffer = Encoding.ASCII.GetBytes(response);
                        stream.Write(buffer, 0, buffer.Length);
                        Debug.WriteLine($"Response : {response}");
                    }
                    else //UTF8
                    {
                        do
                        {
                            int byteSize = stream.Read(buffer, 0, buffer.Length);
                            request += Encoding.UTF8.GetString(buffer, 0, byteSize);
                        }
                        while (stream.DataAvailable);

                        //Responce code for client
                        var response = "received : " + request;
                        if (ResponceMessage.Length > 0) { response = ResponceMessage; }
                        buffer = Encoding.UTF8.GetBytes(response);
                        stream.Write(buffer, 0, buffer.Length);
                        Debug.WriteLine($"Response : {response}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(GetType().Name + "::" + System.Reflection.MethodBase.GetCurrentMethod().Name + " " + ex.ToString());
            }

            return request;
        }

    }
}
