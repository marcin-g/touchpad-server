using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPEndPoint localEP = new IPEndPoint(ipHostInfo.AddressList[0], 11000);

            Console.WriteLine("Local address and port : {0}", localEP.ToString());

            Socket listener = new Socket(localEP.Address.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEP);
                listener.Listen(10);

                while (true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(SocketConnection.acceptCallback),
                        listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Closing the listener...");
        }
        public static void acceptCallback(IAsyncResult ar)
        {
            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            Console.WriteLine("Accept Connection");
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

            // Read data from the client socket.
            int read = handler.EndReceive(ar);
            // Data was read from the client socket.
            if (read > 0)
            {
                byte[] tmp=new byte[read-2];
                Array.Copy(state.Buffer,2,tmp,0,read-2);
                StandardFrame frame=new StandardFrame(state.Buffer[0],(FrameType)((int) state.Buffer[1]),tmp);
                FrameInterpreter.AddFrame(frame);
              //  state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
             /*   handler.BeginReceive(state.Buffer, 0, SocketClient.BufferSize, 0,
                    new AsyncCallback(readCallback), state);*/
            }
            else
            {
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
