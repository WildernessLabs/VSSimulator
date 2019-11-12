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
                        // no ones listening
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

//{
//    // State object for receiving data from remote device.  
//    public class RecvStateObj
//    {
//        // Client socket.  
//        public Socket recvSocket = null;
//        // Size of receive buffer.  
//        public const int recvBufSize = 600;
//        // Receive buffer.  
//        public byte[] recvBuffer = new byte[recvBufSize];

//        public int validDataLen;
//    }

//    public class SendStateObj
//    {
//        // Client socket.  
//        public Socket sendSocket = null;
//        // Size of receive buffer.  
//        public const int BufferSize = 600;
//        // Receive buffer.  
//        public byte[] sendBuffer = new byte[BufferSize];
//        public int validDataLen;
//    }

//    public class AsynchronousClient : IDisposable
//    {
//        // VS 2019 - 4024
//        // VS 2017 - 4022
//        // VS 2015 - 4020
//        int vsPort;
//        Socket serverSocket = null;
//        private VSSim vsSim;
//        RecvStateObj recvStateObj = new RecvStateObj();
//        SendStateObj sendStateObj = new SendStateObj();

//        public AsynchronousClient(int visualStudioPort, VSSim vSSim)
//        {
//            vsPort = visualStudioPort;
//            this.vsSim = vSSim;
//        }

//        // ManualResetEventSlim instances signal completion.  
//        private ManualResetEventSlim connectValid = new ManualResetEventSlim(false);
//        private ManualResetEventSlim needConnection = new ManualResetEventSlim(true);
//        private ManualResetEventSlim sendDone = new ManualResetEventSlim(false);
//        private ManualResetEventSlim receiveDone = new ManualResetEventSlim(false);

//        public bool EchoMode { get; set; } = false;
//        public bool ShutDown { get; set; } = false;
//        public VSSim mainDialogBox { get; set; } = null;

//        public void StartClient()
//        {
//            // Establish the remote endpoint for the socket.  
//            // The name of the   
//            // remote device is "host.contoso.com".  
//            //IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");
//            //IPAddress ipAddress = ipHostInfo.AddressList[0];
//            //IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
//            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");
//            IPAddress ipAddress = ipHostInfo.AddressList[0];
//            IPEndPoint remoteEP = new IPEndPoint(ipAddress, vsPort);

//            // Connect to a remote device.  
//            while (!ShutDown)
//            {
//                try
//                {
//                    needConnection.Wait();
//                    needConnection.Reset();

//                    if (ShutDown)
//                        break;

//                    // Create a TCP/IP socket.  
//                    serverSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

//                    // Connect to the remote endpoint.  
//                    serverSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), serverSocket);

//                    // Wait for connection
//                    connectValid.Wait();
//                    connectValid.Reset();
//                }
//                catch (Exception e)
//                {
//                    vsSim.AddStatusText(e.ToString());

//                    if (!needConnection.IsSet)
//                    {
//                        serverSocket.Shutdown(SocketShutdown.Both);
//                        serverSocket.Close();
//                        serverSocket = null;
//                        connectValid.Reset();
//                    }

//                    // Allow another connection to be established
//                    needConnection.Set();
//                }
//            }

//            // Release the socket.  
//            serverSocket.Shutdown(SocketShutdown.Both);
//            serverSocket.Close();
//        }

//        private void ConnectCallback(IAsyncResult ar)
//        {
//            try
//            {
//                // Retrieve the socket from the state object.  
//                Socket client = (Socket)ar.AsyncState;
//                Debug.Assert(serverSocket == client);

//                // Complete the connection.  
//                client.EndConnect(ar);
//                Console.WriteLine($"Socket connected to {client.RemoteEndPoint}");

//                // Signal that the connection has been made.  
//                connectValid.Set();
//            }
//            catch (Exception e)
//            {
//                vsSim.AddStatusText(e.ToString());
//            }
//        }

//        //----------------------------------------------------------------------
//        // Receive the response from the remote device.
//        public void InitializeReceiving()
//        {
//            recvStateObj.recvSocket = serverSocket;
//            Task.Run(async () =>
//            {
//                await Receive(recvStateObj).ConfigureAwait(false);
//            });
//        }

//        public async void Receive(RecvStateObj recvStateObj)
//        {
//            try
//            {
//                recvStateObj.recvSocket = serverSocket;

//                while(!ShutDown)
//                {
//                    ArraySegment<byte> recvArray = new ArraySegment<byte>(recvStateObj.recvBuffer);
//                    int bytesRead = await serverSocket.ReceiveAsync(recvArray, SocketFlags.None);
//                    if (bytesRead > 0)
//                    {
//                        //receiveDone.Set();

//                        //// Get the rest of the data.  
//                        //client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
//                        //    new AsyncCallback(ReceiveCallback), state);
//                    }
//                    else
//                    {
//                        // Report that 0 bytes received
//                    }

//                    byte[] validData = new byte[recvStateObj.validDataLen];
//                    Array.Copy(recvStateObj.recvArray, 0, validData, 0, recvStateObj.validDataLen);
//                    string validDataString = "hi";
//                    vsSim.AddReceivedData(validDataString);
//                }

//                if (EchoMode)
//                {
//                    Send();
//                }
//            }
//            catch (Exception e)
//            {
//                vsSim.AddStatusText(e.ToString());
//            }
//        }

//        private void ReceiveCallback(IAsyncResult ar)
//        {
//            try
//            {
//                // Retrieve the state object and the client socket   
//                // from the asynchronous state object.  
//                RecvStateObj recvStateObj = (RecvStateObj)ar.AsyncState;
//                Socket client = recvStateObj.recvSocket;

//                // Read data from the remote device.  
//                int bytesRead = client.EndReceive(ar);

//                if (bytesRead > 0)
//                {
//                    //receiveDone.Set();

//                    // Get the rest of the data.  
//                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
//                        new AsyncCallback(ReceiveCallback), state);
//                }
//                else
//                {
//                    // Report that 0 bytes received
//                }
//            }
//            catch (Exception e)
//            {
//                vsSim.AddStatusText(e.ToString());
//            }
//        }

//        //----------------------------------------------------------------------


//        private void Send(Socket client, String data)
//        {
//            // Convert the string data to byte data using ASCII encoding.  
//            byte[] byteData = Encoding.ASCII.GetBytes(data);

//            // Begin sending the data to the remote device.  
//            client.BeginSend(byteData, 0, byteData.Length, 0,
//                new AsyncCallback(SendCallback), client);
//        }

//        private void SendCallback(IAsyncResult ar)
//        {
//            try
//            {
//                // Retrieve the socket from the state object.  
//                Socket client = (Socket)ar.AsyncState;

//                // Complete sending the data to the remote device.  
//                int bytesSent = client.EndSend(ar);
//                vsSim.AddStatusText($"Sent {bytesSent} bytes to server.");

//                // Signal that all bytes have been sent.  
//                sendDone.Set();
//            }
//            catch (Exception e)
//            {
//                vsSim.AddStatusText(e.ToString());
//            }
//        }

//        public void Dispose()
//        {
//            if (!needConnection.IsSet)
//            {
//                serverSocket.Shutdown(SocketShutdown.Both);
//                serverSocket.Close();
//            }
//        }
//    }
//}

/*
using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using System.Text;  
  
// State object for receiving data from remote device.  
public class StateObject {  
    // Client socket.  
    public Socket workSocket = null;  
    // Size of receive buffer.  
    public const int BufferSize = 256;  
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];  
    // Received data string.  
    public StringBuilder sb = new StringBuilder();  
}  
  
public class AsynchronousClient {  
    // The port number for the remote device.  
    private const int port = 11000;  
  
    // ManualResetEventSlim instances signal completion.  
    private static ManualResetEventSlim connectDone =   
        new ManualResetEventSlim(false);  
    private static ManualResetEventSlim sendDone =   
        new ManualResetEventSlim(false);  
    private static ManualResetEventSlim receiveDone =   
        new ManualResetEventSlim(false);  
  
    // The response from the remote device.  
    private static String response = String.Empty;  
  
    private static void StartClient() {  
        // Connect to a remote device.  
        try {  
            // Establish the remote endpoint for the socket.  
            // The name of the   
            // remote device is "host.contoso.com".  
            IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);  
  
            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily,  
                SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect to the remote endpoint.  
            client.BeginConnect( remoteEP,   
                new AsyncCallback(ConnectCallback), client);  
            connectDone.WaitOne();  
  
            // Send test data to the remote device.  
            Send(client,"This is a test<EOF>");  
            sendDone.WaitOne();  
  
            // Receive the response from the remote device.  
            Receive(client);  
            receiveDone.WaitOne();  
  
            // Write the response to the console.  
            Console.WriteLine("Response received : {0}", response);  
  
            // Release the socket.  
            client.Shutdown(SocketShutdown.Both);  
            client.Close();  
  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    private static void ConnectCallback(IAsyncResult ar) {  
        try {  
            // Retrieve the socket from the state object.  
            Socket client = (Socket) ar.AsyncState;  
  
            // Complete the connection.  
            client.EndConnect(ar);  
  
            Console.WriteLine("Socket connected to {0}",  
                client.RemoteEndPoint.ToString());  
  
            // Signal that the connection has been made.  
            connectDone.Set();  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    private static void Receive(Socket client) {  
        try {  
            // Create the state object.  
            StateObject state = new StateObject();  
            state.workSocket = client;  
  
            // Begin receiving the data from the remote device.  
            client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                new AsyncCallback(ReceiveCallback), state);  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    private static void ReceiveCallback( IAsyncResult ar )
    {  
        try
        {  
            // Retrieve the state object and the client socket   
            // from the asynchronous state object.  
            StateObject state = (StateObject) ar.AsyncState;  
            Socket client = state.workSocket;  
  
            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);  
  
            if (bytesRead > 0) {  
                // There might be more data, so store the data received so far.  
            state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));  
  
                // Get the rest of the data.  
                client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,  
                    new AsyncCallback(ReceiveCallback), state);  
            } else {  
                // All the data has arrived; put it in response.  
                if (state.sb.Length > 1) {  
                    response = state.sb.ToString();  
                }  
                // Signal that all bytes have been received.  
                receiveDone.Set();  
            }  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    private static void Send(Socket client, String data) {  
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);  
  
        // Begin sending the data to the remote device.  
        client.BeginSend(byteData, 0, byteData.Length, 0,  
            new AsyncCallback(SendCallback), client);  
    }  
  
    private static void SendCallback(IAsyncResult ar) {  
        try {  
            // Retrieve the socket from the state object.  
            Socket client = (Socket) ar.AsyncState;  
  
            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);  
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);  
  
            // Signal that all bytes have been sent.  
            sendDone.Set();  
        } catch (Exception e) {  
            Console.WriteLine(e.ToString());  
        }  
    }  
  
    public static int Main(String[] args) {  
        StartClient();  
        return 0;  
    }  
}
*/
