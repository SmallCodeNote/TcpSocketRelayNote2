using System;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace tcpClient
{
    public class TcpSocketClient
    {
        public async Task<string> StartClient(string ipaddress, int port, string request, string encoding = "ASCII")
        {
            string response = string.Empty;
            Encoding enc = encoding == "UTF8" ? Encoding.UTF8 : Encoding.ASCII;

            try
            {
                using (var tcpclient = new TcpClient())
                {
                    tcpclient.SendTimeout = 1000;
                    tcpclient.ReceiveTimeout = 1000;
                    tcpclient.NoDelay = true;

                    await tcpclient.ConnectAsync(ipaddress, port).ConfigureAwait(false);
                    Debug.WriteLine("Server connected.");

                    using (var stream = tcpclient.GetStream())
                    {
                        // --- Send ---
                        byte[] writeBuffer = enc.GetBytes(request);
                        await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length).ConfigureAwait(false);
                        Debug.WriteLine($"Send [{request}] to server.");

                        // --- Recieve ---
                        var readBuffer = new byte[4096]; // 4KB
                        int length = await stream.ReadAsync(readBuffer, 0, readBuffer.Length).ConfigureAwait(false);

                        if (length > 0)
                        {
                            response = enc.GetString(readBuffer, 0, length);
                            Debug.WriteLine($"Received [{response}] from server.");
                        }
                        else
                        {
                            response = "NO_RESPONSE";
                            Debug.WriteLine("Server returned no data.");
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                response = $"SOCKET_ERROR: {ex.Message}";
                Debug.WriteLine(ex.ToString());
            }
            catch (TimeoutException ex)
            {
                response = $"TIMEOUT: {ex.Message}";
                Debug.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                response = $"ERROR: {ex.Message}";
                Debug.WriteLine(ex.ToString());
            }

            return response;
        }
    }
}
//2026.2.4