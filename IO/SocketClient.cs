using System.Diagnostics;
using System.Net.Sockets;

namespace touchpad_server.IO
{
    internal class SocketClient
    {
        public const int BufferSize = 10;
        private byte[] _buffer = new byte[BufferSize];

        public Socket WorkSocket { get; set; }

        public byte[] Buffer
        {
            get { return _buffer; }
            set { _buffer = value; }
        }

        public byte[] BrokenFrame { get; set; }
    }
}