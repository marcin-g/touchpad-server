using System.Runtime.InteropServices;

namespace touchpad_server.Controller
{
    public class KeyboardController
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public void PushKey(byte bVk, byte bScan)
        {
            keybd_event(bVk, bScan, 0, 0);
        }

        public void ReleaseKey(byte bVk, byte bScan)
        {
            keybd_event(bVk, bScan, 0x0002, 0);
        }
    }
}