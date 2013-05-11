namespace touchpad_server.DataModel
{
    public enum FrameType
    {
        CLICK = 1,
        MOVE = 2,
        SCROLL = 3,
        MUTE = 4,
        VOLUME_UP = 5,
        VOLUME_DOWN = 6,
        ZOOM = 7,
        CLOSE = 8,
        SWITCH = 9
    }

    public static class FrameTypeExtension
    {
        public static int GetSize(this FrameType frame)
        {
            switch (frame)
            {
                case FrameType.MOVE:
                    return 9;
                case FrameType.SCROLL:
                case FrameType.ZOOM:
                case FrameType.SWITCH:
                    return 5;
                default:
                    return 1;
            }
        }
    }
}