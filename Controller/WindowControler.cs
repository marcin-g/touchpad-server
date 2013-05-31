using System;
using System.Runtime.InteropServices;

namespace touchpad_server.Controller
{
    internal class WindowControler
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, IntPtr msg, int wParam, int lParam);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public void CloseActiveWindow()
        {
            IntPtr window = GetActiveWindow();
            if (window == new IntPtr(0))
            {
                window = GetForegroundWindow();
                Logger.Log("Foreground Window " + window);
            }
            else
            {
                Logger.Log("Active Window " + window);
                
            }
            SendMessage((int)window, (IntPtr) WM_SYSCOMMAND, SC_CLOSE, 0);
        }
    }
}