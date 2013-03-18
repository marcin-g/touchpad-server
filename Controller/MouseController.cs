using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace touchpad_server.Controller
{
    public class MouseController
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x800;

        public void MoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void Move(int offsetX, int offsetY )
        {
            POINT position = new POINT(0, 0);
            GetCursorPos(out position);
            SetCursorPos(position.X + offsetX, position.Y + offsetY);
        }

        public Point GetPosition()
        {
            POINT position = new POINT(0, 0);
            GetCursorPos(out position);

            // MessageBox.Show(position.X + " " + position.Y);
            return new Point(position.X, position.Y);
        }

        public void LMBclick()
        {
            POINT position = new POINT(0, 0); 
            GetCursorPos(out position);
            mouse_event(MOUSEEVENTF_LEFTDOWN, position.X, position.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, position.X, position.Y, 0, 0);
        }
        public void Scroll(int value)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, value, 0);
        }
    }
}
