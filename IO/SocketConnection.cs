using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using touchpad_server.Controller;
using touchpad_server.DataModel;

namespace touchpad_server.IO
{
    public class SocketConnection
    {
        private static readonly ManualResetEvent allDone = new ManualResetEvent(false);
        private readonly IPEndPoint localEP;
        private Socket Listener;
        public SocketConnection(IPAddress address, int port)
        {
            localEP = new IPEndPoint(address, port);
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
                Logger.Log(e.ToString());
                throw e;
                //Console.WriteLine(e.ToString());
            }

            Logger.Log("Closing the listener...");
            //Console.WriteLine("Closing the listener...");
        }

        public static void acceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            try
            {
                var listener = (Socket) ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                Logger.Log("Accept Connection");
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
                Logger.Log(e.ToString());
            }
        }

        public static void readCallback(IAsyncResult ar)
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
                Logger.Log("Zamknieto połączenie");
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
                    state.sw.Stop();
                    Logger.LogTime("BEFORE_ADDFRAME " + state.sw.ElapsedTicks);
                    state.sw.Reset();
                    state.sw.Start();
                    FrameInterpreter.AddFrame(new StandardFrame(type, tmp));
                    state.sw.Stop();
                    Logger.LogTime("ADDFRAME " + state.sw.ElapsedTicks);

                }

                for (int i = startIndex; i < read;)
                {
                    state.sw.Reset();
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

                            Logger.LogTime("BEFORE_ADDFRAME " + state.sw.ElapsedTicks);
                            state.sw.Reset();
                            state.sw.Start();
                            FrameInterpreter.AddFrame(frame);
                            state.sw.Stop();
                            Logger.LogTime("ADDFRAME " + state.sw.ElapsedTicks);
                        }
                    }
                    else
                    {
                        frame = new StandardFrame(type, null);
                        Logger.LogTime("BEFORE_ADDFRAME " + state.sw.ElapsedTicks);
                        state.sw.Reset();
                        state.sw.Start();
                        FrameInterpreter.AddFrame(frame);
                        state.sw.Stop();
                        Logger.LogTime("ADDFRAME " + state.sw.ElapsedTicks);
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
        public void Stop()
        {
            Listener.Close();
        }
    }
    
}