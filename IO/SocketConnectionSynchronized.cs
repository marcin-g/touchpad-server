using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using touchpad_server.Controller;
using touchpad_server.DataModel;

namespace touchpad_server.IO
{
    public class SocketConnectionSynchronized
    {

        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private IPEndPoint localEP;
        public SocketConnectionSynchronized(IPAddress address, int port)
        {
            localEP = new IPEndPoint(address, port);
        }
        public void StartListening()
        {
            
          
            
            Logger.Log("Local address and port : " + localEP.ToString());
            //Console.WriteLine("Local address and port : {0}", localEP.ToString());

            Socket listener = new Socket(localEP.Address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEP);
                listener.Listen(10);

                while (true)
                {
                    while (true)
                    {
                        Console.WriteLine("Waiting for a connection...");
                        // Program is suspended while waiting for an incoming connection.
                        Socket handler = listener.Accept();

                        // An incoming connection needs to be processed.
                        while (true)
                        {
                            if (!readCallback(handler))
                            {
                                break;
                            }
                            
                            
                        }

                        // Show the data on the console.

                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }

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
        
        public static bool readCallback(Socket handler)
        {
            
            byte[] bytes=new byte[9];
            int read = 0; 

            try
            {

                read = handler.Receive(bytes);

            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
            // Data was read from the client socket.
            if (read > 0)
            {
                read = Math.Min(read, bytes.Length);
                for (int i = 0; i < read; )
                {
                    
                    //Logger.Log((string)read.ToString("G"));
                    StandardFrame frame = null;
                    FrameType type = (FrameType)((int)bytes[i]);
                    if (type.GetSize() - 1 > 0)
                    {
                        byte[] tmp = new byte[type.GetSize() - 1];
                        Array.Copy(bytes, i + 1, tmp, 0, type.GetSize() - 1);
                        frame = new StandardFrame(type, tmp);
                    }
                    else
                    {
                        frame = new StandardFrame(type, null);
                    }
                    FrameInterpreter.AddFrame(frame);
                    i += type.GetSize();

                }
                Logger.Log("KONIEC");
              //  state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
            }
            else
            {

                Logger.Log("Zamknieto połączenie");
                return false;
               /* if (state.sb.Length > 1)
                {
                    // All the data has been read from the client;
                    // display it on the console.
                    string content = state.sb.ToString();
                    Console.WriteLine("Read {0} bytes from socket.\n Data : {1}",
                       content.Length, content);
                }*/
            }
            return true;
        }



    }

}
