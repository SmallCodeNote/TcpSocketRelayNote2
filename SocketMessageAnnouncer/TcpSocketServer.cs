using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tcpServer
{
    class TcpSocketServer : IDisposable
    {
        //===================
        // Member variable
        //===================
        public DateTime LastReceiveTime { get; private set; }
        public int Port { get; private set; }

        /// <summary>ex.)"ASCII","UTF8"</summary>
        public string EncodingName { get; private set; } = "ASCII";

        /// <summary> Received TcpSocket Queue. </summary>
        public ConcurrentQueue<string> ReceivedSocketQueue { get; }

        public int ReceivedSocketQueueMaxSize { get; set; } = 1024;
        public int ReceiveBufferSize { get; set; } = 1024;
        public string ResponceMessage { get; set; } = "";

        private Task _listeningTask;
        private CancellationTokenSource _tokenSource;
        private readonly object _syncRoot = new object();

        public bool IsBusy { get; private set; }

        private TcpListener _tcpListener;
        private Encoding _encoder = Encoding.ASCII;
        private bool _disposed;

        //===================
        // Constructor
        //===================
        public TcpSocketServer()
        {
            IsBusy = false;
            LastReceiveTime = DateTime.Now;
            ReceivedSocketQueue = new ConcurrentQueue<string>();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Stop();
            _tokenSource?.Dispose();
        }

        //===================
        // Member function
        //===================
        public void Start(int port, string encodingName = "ASCII")
        {
            if (_disposed) throw new ObjectDisposedException(nameof(TcpSocketServer));

            lock (_syncRoot)
            {
               
                if (_listeningTask != null && !_listeningTask.IsCompleted)
                {
                    return;
                }

                // CTS create 
                _tokenSource?.Dispose();
                _tokenSource = new CancellationTokenSource();
                var token = _tokenSource.Token;

                Port = port;
                EncodingName = encodingName;
                _encoder = (EncodingName == "UTF8") ? Encoding.UTF8 : Encoding.ASCII;

                _listeningTask = Task.Run(() => ListeningMessageAsync(token), token);
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (_listeningTask == null || _listeningTask.IsCompleted)
                {
                    return;
                }

                try
                {
                    _tokenSource?.Cancel();

                    if (_tcpListener != null && IsBusy)
                    {
                        try
                        {
                            _tcpListener.Stop();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"{GetType().Name}::{nameof(Stop)} TcpListener.Stop error: {ex}");
                        }
                    }

                    try
                    {
                        _listeningTask.Wait();
                    }
                    catch (AggregateException ae)
                    {
                        // exception from cancel process
                        ae.Handle(e => e is OperationCanceledException || e is ObjectDisposedException || e is SocketException);
                    }
                    catch (OperationCanceledException)
                    {

                    }
                }
                finally
                {
                    _listeningTask = null;
                    IsBusy = false;
                }
            }
        }

        private async Task ListeningMessageAsync(CancellationToken token)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            _tcpListener = new TcpListener(localEndPoint);

            try
            {
                _tcpListener.Start();
                IsBusy = true;
                Debug.WriteLine($"{GetType().Name}::{nameof(ListeningMessageAsync)} started on port {Port}");

                while (!token.IsCancellationRequested)
                {
                    removeOverflowQueueFromReceivedSocketQueue();

                    TcpClient tcpClient = null;
                    try
                    {
                        // canchel check
                        var acceptTask = _tcpListener.AcceptTcpClientAsync();
                        var completed = await Task.WhenAny(acceptTask, Task.Delay(Timeout.Infinite, token)).ConfigureAwait(false);

                        if (completed != acceptTask)
                        {
                            break;
                        }

                        tcpClient = acceptTask.Result;
                        Debug.WriteLine("tcpListener.AcceptTcpClient()");

                        string receivedMessage = getReceivedMessage(tcpClient);
                        if (receivedMessage != null)
                        {
                            LastReceiveTime = DateTime.Now;
                            var line = $"{LastReceiveTime:yyyy/MM/dd HH:mm:ss.fff}\t{receivedMessage}";
                            ReceivedSocketQueue.Enqueue(line);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // cancel process
                        break;
                    }
                    catch (ObjectDisposedException)
                    {
                        // Listener Stop 
                        break;
                    }
                    catch (SocketException ex)
                    {
                        // Listener error
                        Debug.WriteLine($"{GetType().Name}::{nameof(ListeningMessageAsync)} SocketException: {ex}");
                        if (token.IsCancellationRequested)
                            break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{GetType().Name}::{nameof(ListeningMessageAsync)} Exception: {ex}");
                        // if not fatal
                        if (token.IsCancellationRequested)
                            break;
                    }
                    finally
                    {
                        tcpClient?.Close();
                        tcpClient?.Dispose();
                    }
                }
            }
            finally
            {
                try
                {
                    _tcpListener?.Stop();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{GetType().Name}::{nameof(ListeningMessageAsync)} TcpListener.Stop error in finally: {ex}");
                }

                IsBusy = false;
                Debug.WriteLine($"{GetType().Name}::{nameof(ListeningMessageAsync)} stopped");
            }
        }

        private void removeOverflowQueueFromReceivedSocketQueue()
        {
            while (ReceivedSocketQueue.Count >= ReceivedSocketQueueMaxSize)
            {
                string removed;
                if (ReceivedSocketQueue.TryDequeue(out removed))
                {
                    Debug.WriteLine($"{GetType().Name}::{nameof(removeOverflowQueueFromReceivedSocketQueue)} removed oldest item");
                }
                else
                {
                    break;
                }
            }
        }

        private string getHTTPResponceStatusAndHeader()
        {
            var lines = new List<string>
            {
                "HTTP/1.1 200 OK",
                "Server: TcpSocketServerClass",
                "Date: " + DateTime.UtcNow.ToString("R"),
                "Content-Length: " + ResponceMessage.Length,
                "Content-Type: text/xml; charset=UTF-8"
            };

            return string.Join("\r\n", lines) + "\r\n\r\n";
        }

        public string getReceivedMessage(TcpClient tcpClient)
        {
            if (tcpClient == null) return string.Empty;

            byte[] buffer = new byte[ReceiveBufferSize];
            var sb = new StringBuilder();

            try
            {
                using (NetworkStream stream = tcpClient.GetStream())
                {
                    do
                    {
                        int byteSize = stream.Read(buffer, 0, buffer.Length);
                        if (byteSize <= 0)
                        {
                            break;
                        }

                        sb.Append(_encoder.GetString(buffer, 0, byteSize));

                    } while (stream.DataAvailable);

                    string receivedMessage = sb.ToString();
                    string response = DateTime.Now.ToString("HH:mm:ss.fff") + " received : " + receivedMessage;

                    string[] cols = receivedMessage.Replace("\r\n", "\n").Split('\n');
                    if (cols.Length > 0)
                    {
                        string[] colsB = cols[0].Split(' ');
                        if (colsB.Length > 0 && colsB[colsB.Length - 1].IndexOf("HTTP", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            response = getHTTPResponceStatusAndHeader() + ResponceMessage;
                        }
                    }

                    byte[] responseBytes = _encoder.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Debug.WriteLine($"Response : {response}");

                    return receivedMessage;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{GetType().Name}::{nameof(getReceivedMessage)} Exception: {ex}");
                return string.Empty;
            }
        }
    }
}