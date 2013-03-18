namespace touchpad_server.DataModel
{
    public class StandardFrame
    {
        private readonly byte _id;
        private readonly FrameType _type;
        private readonly byte[] _argument;

        public FrameType Type
        {
            get { return _type; }
        }

        public byte[] Argument
        {
            get { return _argument; }
        }

        public byte Id
        {
            get { return _id; }
        }
        public StandardFrame(byte id, FrameType type, byte[] argument)
        {
            _id = id;
            _type = type;
            _argument = argument;
        }
    }
}
