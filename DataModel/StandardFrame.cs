namespace touchpad_server.DataModel
{
    public class StandardFrame
    {
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

        public StandardFrame(FrameType type, byte[] argument)
        {
            _type = type;
            _argument = argument;
        }
    }
}
