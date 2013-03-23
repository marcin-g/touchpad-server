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
    public class SocketConnection
    {

        private static ManualResetEvent allDone = new ManualResetEvent(false);
        private IPEndPoint localEP;
        public SocketConnection(IPAddress address, int port)
        {
            localEP = new IPEndPoint(address, port);
        }
        public void StartListening()
        {
            
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            
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
                    allDone.Reset();
                    Logger.Log("Waiting for a connection...");
                    //Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(SocketConnection.acceptCallback),
                        listener);

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
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            Logger.Log("Accept Connection");
            //Console.WriteLine("Accept Connection");
            // Signal the main thread to continue.
            allDone.Set();
            // Create the state object.
            SocketClient client = new SocketClient();
            client.WorkSocket = handler;
            handler.BeginReceive(client.Buffer, 0, SocketClient.BufferSize, 0,
                new AsyncCallback(SocketConnection.readCallback), client);
        }
        public static void readCallback(IAsyncResult ar)
        {
            SocketClient state = (SocketClient)ar.AsyncState;
            Socket handler = state.WorkSocket;


            int read = 0;
            try
            {

                read = handler.EndReceive(ar);

            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
            // Data was read from the client socket.
            if (read > 0)
            {

                for (int i = 0; i < read; )
                {
                    
                    //Logger.Log((string)read.ToString("G"));
                    StandardFrame frame = null;
                    FrameType type= (FrameType) ((int) state.Buffer[i]);
                    if (type.GetSize() - 1 > 0)
                    {
                        byte[] tmp = new byte[type.GetSize() - 1];
                        Array.Copy(state.Buffer, i + 1, tmp, 0, type.GetSize() - 1);
                        frame = new StandardFrame(type, tmp);
                    }
                    else
                    {
                        frame = new StandardFrame(type, null);
                    }
                    FrameInterpreter.AddFrame(frame);
                    i += type.GetSize();

                }
              //  state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                handler.BeginReceive(state.Buffer, 0, SocketClient.BufferSize, 0,
                    new AsyncCallback(readCallback), state);
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



    }

}
