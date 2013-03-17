using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            Console.WriteLine("Accept Connection2");
            // Create the state object.
            StateObject state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(SocketConnection.readCallback), state);
        }
        public static void readCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            // Read data from the client socket.
            int read = handler.EndReceive(ar);

            Console.WriteLine("Read");
            // Data was read from the client socket.
            if (read > 0)
            {
                Console.WriteLine("Read1");
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, read));
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(readCallback), state);
                Console.WriteLine("Read {0} bytes from socket.\n Data : {1}",
                      state.sb.Length, state.sb);
            }
            else
            {

                Console.WriteLine("Read2");
                if (state.sb.Length > 1)
                {
                    // All the data has been read from the client;
                    // display it on the console.
                    string content = state.sb.ToString();
                    Console.WriteLine("Read {0} bytes from socket.\n Data : {1}",
                       content.Length, content);
                }
                handler.Close();
            }
        }



    }

}
