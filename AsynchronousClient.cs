using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VSSimulator
{
    // VS 2019 - 4024
    // VS 2017 - 4022
    // VS 2015 - 4020

    public class AsynchronousClient : IDisposable
    {
        TcpClient tcpClient;
        readonly int vsPort;
        readonly IPAddress vsIPAddress;
        readonly VSSim vsSim;

        NetworkStream networkStream;

        public AsynchronousClient(VSSim visualStudioSim, int visualStudioPort) 
        {
            vsPort = visualStudioPort;
            vsSim = visualStudioSim;

            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            vsIPAddress = ipHostInfo.AddressList[1];

            tcpClient = null;
        }

        public async void StartWorking()
        {
            vsSim.AddStatusText($"Start Working - Ready to attempt connection{Environment.NewLine}");
            while(true)
            {
                if(tcpClient == null)
                {
                    // Must use same protocol as server
                    tcpClient = new TcpClient(AddressFamily.InterNetworkV6);
                }

                try
                {
                    if(!tcpClient.Connected)
                    {
                        await tcpClient.ConnectAsync(IPAddress.IPv6Loopback, vsPort).ConfigureAwait(false);
                    
                        vsSim.AddStatusText($"Connection made to server{Environment.NewLine}");

                        // Get the stream
                        networkStream = tcpClient.GetStream();
                    }
                    else
                    {
                        Thread.Sleep(0);    // place for break point
                    }

                    // Start receiving
                    await Task.Run(() => ReceiveMonoData(networkStream)).ConfigureAwait(false);
                }
                catch (SocketException sockex)
                {
                    if (sockex.SocketErrorCode == SocketError.IsConnected)
                    {
                        vsSim.AddStatusText($"SocketException:IsConnected,CLI must have stopped. Must restart.{Environment.NewLine}");
                        tcpClient.Close();
                        tcpClient = null;
                    }
                    else if (sockex.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        vsSim.AddStatusText($"SocketException:Connection refused, CLI not running{Environment.NewLine}");
                        // no one's listening
                    }
                    else
                    {
                        vsSim.AddStatusText($"SocketException:{sockex}{Environment.NewLine}");
                    }
                    Thread.Sleep(5 * 1000);
                }
                catch (Exception ex)
                {
                    vsSim.AddStatusText($"Exception: {ex}{Environment.NewLine}");
                    Thread.Sleep(5 * 1000);
                }
            }
            vsSim.AddStatusText($"Dropped out of connect -> receive and send loop{Environment.NewLine}");
        }

        private async Task ReceiveMonoData(NetworkStream netStream)
        {
            byte[] recvBuffer = new byte[498];
            try
            {
                vsSim.AddStatusText($"Beginning receive{Environment.NewLine}");
                while(true)
                {
                    int bytesRead = await netStream.ReadAsync(recvBuffer, 0, recvBuffer.Length).ConfigureAwait(false);
                    if(bytesRead == 0)
                    {
                        vsSim.AddStatusText($"Server has disconnected{Environment.NewLine}");
                        Thread.Sleep(500);
                    }
                    else
                    {
                        vsSim.AddStatusText($"Received {bytesRead} bytes{Environment.NewLine}");
                        byte[] recvData = new byte[bytesRead];
                        Array.Copy(recvBuffer, 0, recvData, 0, bytesRead);
                        vsSim.ReceivedData(recvData);
                    }
                }
            }
            catch(System.IO.IOException ioe)
            {
                vsSim.AddStatusText($"IO.Exception: {ioe}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                vsSim.AddStatusText($"Exception: {ex}{Environment.NewLine}");
            }
        }

        public async Task SendVSData(byte[] dataToSend, int dataLength)
        {
            try
            {
                Debug.Assert(dataToSend != null);
                if (dataToSend == null || dataLength == 0)
                    return;

                vsSim.AddStatusText($"Sending {dataLength} bytes{Environment.NewLine}");
                await networkStream.WriteAsync(dataToSend, 0, dataLength).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                vsSim.AddStatusText($"Exception: {ex}{Environment.NewLine}");
            }
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ((IDisposable)networkStream).Dispose();
            tcpClient.Dispose();
        }
    }
}

