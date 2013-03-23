using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace touchpad_server.IO
{
    class SocketClient
    {
        private Socket _workSocket;
        private byte[] _buffer = new byte[BufferSize];
        public const int BufferSize = 512;

        public Socket WorkSocket
        {
            get { return _workSocket; }
            set { _workSocket = value; }
        }

        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }
    }
}
