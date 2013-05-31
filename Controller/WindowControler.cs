using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace touchpad_server.Controller
{
    internal class WindowControler
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, IntPtr msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.Dll")]
        public static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);
        [DllImport("user32.Dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc callback, IntPtr extraData);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern string GetClassName(IntPtr hwnd);

        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        private const UInt32 WM_CLOSE = 0x0010;

        public void CloseActiveWindow()
        {
            IntPtr window= GetForegroundWindow();
            if (window != null)
            {
                uint i = 0;
                GetWindowThreadProcessId(window, out i);
                Process p=Process.GetProcessById((int)i);
                if (p != null)
                {
                    if (p.ProcessName == "explorer")
                    {
                        SendKeys.SendWait("%{F4}");
                    }
                    else
                    {
                       p.CloseMainWindow();
                    }
                }
            }
          /*  IntPtr window = GetActiveWindow();
            if (window == new IntPtr(0))
            {
                window = GetForegroundWindow();
                Logger.Log("Foreground Window " + window);
            }
            else
            {
                Logger.Log("Active Window " + window);
                
            }*=
            SendMessage((int)window, (IntPtr) WM_SYSCOMMAND, SC_CLOSE, 0);*/
        }

        public enum DesktopWindow
        {
            ProgMan,
            SHELLDLL_DefViewParent,
            SHELLDLL_DefView,
            SysListView32
        }

        public static IntPtr GetDesktopWindow(DesktopWindow desktopWindow)
        {
            IntPtr _ProgMan = GetShellWindow();
            IntPtr _SHELLDLL_DefViewParent = _ProgMan;
            IntPtr _SHELLDLL_DefView = FindWindowEx(_ProgMan, IntPtr.Zero, "SHELLDLL_DefView", null);
            IntPtr _SysListView32 = FindWindowEx(_SHELLDLL_DefView, IntPtr.Zero, "SysListView32", "FolderView");

            if (_SHELLDLL_DefView == IntPtr.Zero)
            {
                EnumWindows((hwnd, lParam) =>
                {
                    if (GetClassName(hwnd) == "WorkerW")
                    {
                        IntPtr child = FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                        if (child != IntPtr.Zero)
                        {
                            _SHELLDLL_DefViewParent = hwnd;
                            _SHELLDLL_DefView = child;
                            _SysListView32 = FindWindowEx(child, IntPtr.Zero, "SysListView32", "FolderView"); ;
                            return false;
                        }
                    }
                    return true;
                }, IntPtr.Zero);
            }

            switch (desktopWindow)
            {
                case DesktopWindow.ProgMan:
                    return _ProgMan;
                case DesktopWindow.SHELLDLL_DefViewParent:
                    return _SHELLDLL_DefViewParent;
                case DesktopWindow.SHELLDLL_DefView:
                    return _SHELLDLL_DefView;
                case DesktopWindow.SysListView32:
                    return _SysListView32;
                default:
                    return IntPtr.Zero;
            }
        }
    }
}