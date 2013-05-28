using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace touchpad_server.Controller
{
    public class MouseController
    {
       // InputSimulator TheInputSimulator = new InputSimulator();
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_WHEEL = 0x800;
        private const int MOUSEEVENTF_MOVE = 0x0001;

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, IntPtr dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        public void MoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void Move(int offsetX, int offsetY)
        {
            /* POINT position = new POINT(0, 0);
            GetCursorPos(out position);
            SetCursorPos(position.X + offsetX, position.Y + offsetY);*/
            // System.Windows.PointCursor.Position.X += offsetX;
            mouse_event(MOUSEEVENTF_MOVE, offsetX, offsetY, 0, IntPtr.Zero);
           // TheInputSimulator.Mouse.MoveMouseBy(offsetX, offsetY);
        }

        public Point GetPosition()
        {
            var position = new POINT(0, 0);
            GetCursorPos(out position);

            // MessageBox.Show(position.X + " " + position.Y);
            return new Point(position.X, position.Y);
        }

        public void LMBclick()
        {
            var position = new POINT(0, 0);
            GetCursorPos(out position);
            mouse_event(MOUSEEVENTF_LEFTDOWN, position.X, position.Y, 0, IntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, position.X, position.Y, 0, IntPtr.Zero);
        }

        public void Scroll(int value)
        {
            //TheInputSimulator.Mouse.VerticalScroll(value);

           /* var mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_WHEEL;
            mouseDownInput.mkhi.mi.dwExtraInfo = value;
            SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));*/
            if (value > 0)
            {
                mouse_event((int) MouseEventFlags.MOUSEEVENTF_WHEEL, 0, 0, Math.Min(value, 20) * 120, IntPtr.Zero);
            }
            else
            {
                mouse_event((int)MouseEventFlags.MOUSEEVENTF_WHEEL, 0, 0, Math.Max(value, -20) * 120, IntPtr.Zero);
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        public static void ClickLeftMouseButton()
        {
            var mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
            SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

            var mouseUpInput = new INPUT();
            mouseUpInput.type = SendInputEventType.InputMouse;
            mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
            SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
        }

        public static void ClickRightMouseButton()
        {
            var mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
            SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

            var mouseUpInput = new INPUT();
            mouseUpInput.type = SendInputEventType.InputMouse;
            mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
            SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
        }

        public static void MoveMouse(int offsetX, int offsetY)
        {
            var mouseDownInput = new INPUT();
            mouseDownInput.type = SendInputEventType.InputMouse;
            mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_MOVE;
            mouseDownInput.mkhi.mi.dx = offsetX;
            mouseDownInput.mkhi.mi.dy = offsetY;
            SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public readonly int uMsg;
            public readonly short wParamL;
            public readonly short wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public SendInputEventType type;
            public MouseKeybdhardwareInputUnion mkhi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public readonly ushort wVk;
            public readonly ushort wScan;
            public readonly uint dwFlags;
            public readonly uint time;
            public readonly IntPtr dwExtraInfo;
        }

        [Flags]
        private enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000
        }

        private struct MouseInputData
        {
            public IntPtr dwExtraInfo;
            public MouseEventFlags dwFlags;
            public int dx;
            public int dy;
            public uint mouseData;
            public uint time;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct MouseKeybdhardwareInputUnion
        {
            [FieldOffset(0)] public MouseInputData mi;

            [FieldOffset(0)] public readonly KEYBDINPUT ki;

            [FieldOffset(0)] public readonly HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private enum SendInputEventType
        {
            InputMouse,
            InputKeyboard,
            InputHardware
        }
    }
}