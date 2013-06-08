using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using touchpad_server.Controller;
using touchpad_server.DataModel;

namespace touchpad_server.IO
{
    public class SocketConnection
    {
        private readonly ManualResetEvent allDone = new ManualResetEvent(false);
        private readonly IPEndPoint localEP;
        private Socket Listener;
        private static Dictionary<SocketConnection,Thread> connections = new Dictionary<SocketConnection,Thread> ();
        private static int _connectionPort=11000;
        private static readonly ManualResetEvent connectedLock = new ManualResetEvent(false);
        private static bool connectionsCreated = false;
        private static int clients = 0;

        public SocketConnection(IPAddress address, int port)
        {
            localEP = new IPEndPoint(address, port);
        }

        public static int ConnectionPort
        {
            get { return _connectionPort; }
            set { _connectionPort = value; }
        }

        public static string CreateConnection()
        {
            connectionsCreated = true;
            connectedLock.Reset();
            List<IPAddress> ips = new List<IPAddress>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in nic.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ips.Add(ip.Address);
                        }
                    }
                }
            }

            string result = "";
            foreach (var ipAddress in ips)
            {
                SocketConnection tmpSocket = new SocketConnection(ipAddress, ConnectionPort);
                Thread tmpThread = new Thread(delegate() {
                    try
                    {
                        tmpSocket.StartListening();
                    }
                    catch (SocketException)
                    {
                        Logger.Log("zamknieto watek");
                    }
                                                             
                });
                tmpThread.Start();
                connections.Add(tmpSocket,tmpThread);
                result += ipAddress.ToString() +":"+ConnectionPort+ ";";
            }
            result=result.Remove(result.LastIndexOf(";"));
            return result;
        }
        public static void CloseOtherConnections(SocketConnection notClosed)
        {
            if (!connectionsCreated)
            {
                return;
            }
            Thread notClosedThread = null;
            if (notClosed != null)
            {
                notClosedThread = connections[notClosed];
            }
            foreach (var item in connections)
            {
                if (item.Key != notClosed)
                {
                    try
                    {
                        item.Key.CloseNotBinded();
                        Logger.Log("zamknieto polaczenie w CloseOtherConnections");
                    }
                    catch (Exception)
                    {
                        Logger.Log("zamknieto polaczenie w CloseOtherConnections");
                    }
                }
            }
          //  clients = connections.Count - 1;
            if (notClosed == null)
            {
                clients = 0;
            }
            connections.Clear();
            if (notClosed != null)
            {
                connections.Add(notClosed, notClosedThread);
            }
            connectionsCreated = false;
            connectedLock.Set();
        }

        public static SocketConnection GetConnectedSocket()
        {
            connectedLock.WaitOne();
            if (connections.Count != 0)
            {
            
            IEnumerator enumerator = connections.Keys.GetEnumerator();
            enumerator.MoveNext();
            return (SocketConnection) enumerator.Current;
            }
            return null;

        }

        public void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());

            Logger.Log("Local address and port : " + localEP);
            //Console.WriteLine("Local address and port : {0}", localEP.ToString());

            Listener = new Socket(localEP.Address.AddressFamily,
                                      SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Listener.Bind(localEP);
                Listener.Listen(10);

                while (true)
                {
                    allDone.Reset();
                    Logger.Log("Waiting for a connection...");
                    //Console.WriteLine("Waiting for a connection...");
                    Listener.BeginAccept(
                        acceptCallback,
                        Listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Logger.Log("Closing the listener...");
                Logger.Log(e.ToString());
                throw new SocketException();
                //Console.WriteLine(e.ToString());
            }

            //Console.WriteLine("Closing the listener...");
        }

        private void acceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            try
            {
                var listener = (Socket) ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                Logger.Log("Accept Connection");
                SocketConnection.CloseOtherConnections(this);
                clients++;
                MainWindow.ClientsNumber(clients);
                //Console.WriteLine("Accept Connection");
                // Signal the main thread to continue.
                allDone.Set();
                // Create the state object.
                var client = new SocketClient();
                client.WorkSocket = handler;
                handler.BeginReceive(client.Buffer, 0, SocketClient.BufferSize, 0,
                                     readCallback, client);
            }
            catch (Exception e)
            {
                Logger.Log("Closing the listener...");
                Logger.Log(e.ToString());
                MainWindow.ClientsNumber(clients);
            }
        }

        private void readCallback(IAsyncResult ar)
        {
            var state = (SocketClient) ar.AsyncState;
            Socket handler = state.WorkSocket;


            int read = 0;
            try
            {
                read = handler.EndReceive(ar);
            }
            catch (SocketException)
            {
                allDone.Set();
                clients--;
                Logger.Log("Zamknieto połączenie");
                MainWindow.ClientsNumber(clients);
                handler.Close();
            }


            // Data was read from the client socket.
            if (read > 0)
            {
                int startIndex = 0;
                if (state.BrokenFrame != null && state.BrokenFrame.Length > 0)
                {
                    var type = (FrameType) (state.BrokenFrame[0]);
                    var tmp = new byte[type.GetSize() - 1];
                    for (int i = 0; i < state.BrokenFrame.Length - 1; i++)
                    {
                        tmp[i] = state.BrokenFrame[i + 1];
                    }
                    for (int i = 0, j = state.BrokenFrame.Length - 1; j < type.GetSize() - 1; i++,j++,startIndex++)
                    {
                        tmp[j] = state.Buffer[i];
                    }

                    state.BrokenFrame = null;
                    FrameInterpreter.AddFrame(new StandardFrame(type, tmp));

                }

                for (int i = startIndex; i < read;)
                {
                    //Logger.Log((string)read.ToString("G"));
                    StandardFrame frame = null;
                    var type = (FrameType) (state.Buffer[i]);
                    if (type.GetSize() - 1 > 0)
                    {
                        if (i + type.GetSize() > read)
                        {
                            var tmp = new byte[read - i];
                            Array.Copy(state.Buffer, i, tmp, 0, read - i);
                            state.BrokenFrame = tmp;
                        }
                        else
                        {
                            var tmp = new byte[type.GetSize() - 1];
                            Array.Copy(state.Buffer, i + 1, tmp, 0, type.GetSize() - 1);
                            frame = new StandardFrame(type, tmp);

                            FrameInterpreter.AddFrame(frame);
                        }
                    }
                    else
                    {
                        frame = new StandardFrame(type, null);
                        FrameInterpreter.AddFrame(frame);
                    }

                    i += type.GetSize();
                }
                //  state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                handler.BeginReceive(state.Buffer, 0, SocketClient.BufferSize, 0,
                                     readCallback, state);
            }
            else
            {
                Logger.Log("Zamknieto połączenie");
                
                /* if (state.sb.Length > 1)
                {
                    // All the data has been read from the client;
                    // display it on the console.
                    string content = state.sb.ToString();
                    Console.WriteLine("Read {0} bytes from socket.\n Data : {1}",
                       content.Length, content);
                }*/
                handler.Close();
            }
        }

        //CloseNotBinded not binded socket
        public void CloseNotBinded()
        {
            //Listener.Shutdown(SocketShutdown.Receive);
            //Listener.Disconnect(false);
            Listener.Close();
            allDone.Set();
        }
        //CloseNotBinded not binded socket
        public void CloseBinded()
        {
            Listener.Shutdown(SocketShutdown.Receive);
            Listener.Close();
        }
    }
    
}