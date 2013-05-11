namespace touchpad_server.DataModel
{
    public class StandardFrame
    {
        private readonly byte[] _argument;
        private readonly FrameType _type;

        public StandardFrame(FrameType type, byte[] argument)
        {
            _type = type;
            _argument = argument;
        }

        public FrameType Type
        {
            get { return _type; }
        }

        public byte[] Argument
        {
            get { return _argument; }
        }
    }
}