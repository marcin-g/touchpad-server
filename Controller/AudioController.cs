using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace touchpad_server.Controller
{
    public class AudioController
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
                                                 IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /*
        private void btnMute_Click(object sender, EventArgs e)
        {
            SendMessageW(MainWindow.myInstance, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }
 
        private void btnDecVol_Click(object sender, EventArgs e)
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }
 
        private void btnIncVol_Click(object sender, EventArgs e)
        {
            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }*/

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);


        /*private void trackWave_Scroll(object sender, EventArgs e)
      {
         // Calculate the volume that's being set
         int NewVolume = ((ushort.MaxValue / 10) * trackWave.Value);
         // Set the same volume for both the left and the right channels
         uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
         // Set the volume
         waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
      }*/

        /* public void Mute()
        {
            waveOutSetVolume(IntPtr.Zero, 0);
        }*/

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public void VolumeUp()
        {
            MainWindow.myInstance.Dispatcher.BeginInvoke(new Action(delegate
                {
                    var windowInteropHelper = new WindowInteropHelper(MainWindow.myInstance);
                    SendMessageW(windowInteropHelper.Handle, WM_APPCOMMAND, windowInteropHelper.Handle,
                                 (IntPtr) APPCOMMAND_VOLUME_UP);
                }));
            //keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
        }

        public void VolumeDown()
        {
            MainWindow.myInstance.Dispatcher.BeginInvoke(new Action(delegate
                {
                    var windowInteropHelper = new WindowInteropHelper(MainWindow.myInstance);
                    SendMessageW(windowInteropHelper.Handle, WM_APPCOMMAND, windowInteropHelper.Handle,
                                 (IntPtr) APPCOMMAND_VOLUME_DOWN);
                }));
            // keybd_event((byte)Keys.VolumeDown, 0, 0, 0);
        }

        public void Mute()
        {
            MainWindow.myInstance.Dispatcher.BeginInvoke(new Action(delegate
                {
                    var windowInteropHelper = new WindowInteropHelper(MainWindow.myInstance);
                    SendMessageW(windowInteropHelper.Handle, WM_APPCOMMAND, windowInteropHelper.Handle,
                                 (IntPtr) APPCOMMAND_VOLUME_MUTE);
                }));
            //keybd_event((byte)Keys.VolumeMute, 0, 0, 0);
        }

        /*   public void ZoomIn(int val)
        {
            //DTM_ZOOMLEVEL
            keybd_event(0x11, 0x9d,0 , 0);
            MouseController.Scroll(val);
            keybd_event(0x11,0x9d,0x0002,0);
        }
      /*  public void ZoomOut()
        {
            keybd_event((byte)Keys.Pa, 0, 0, 0);
        }*/
        /* public void SetVolume(int volume)
        {
            int NewVolume = ((ushort.MaxValue / 10) * volume);
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }*/
    }
}