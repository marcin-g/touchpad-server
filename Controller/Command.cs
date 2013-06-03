using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;

namespace touchpad_server.Controller
{
    public class Command
    {
        private readonly AudioController _audioController = new AudioController();
        private readonly KeyboardController _keyBoardController = new KeyboardController();
        private readonly MouseController _mouseController = new MouseController();
        private readonly WindowControler _windowControler = new WindowControler();

        public void MoveCursor(int offsetX, int offsetY)
        {
            _mouseController.Move(offsetX, offsetY);
        }

        public void LeftClick()
        {
            _mouseController.LMBclick();
        }

        public void Scroll(int scroll)
        {
            _mouseController.Scroll(scroll);
        }

        public void Mute()
        {
            _audioController.Mute();
        }

        public void VolumeUp()
        {
            _audioController.VolumeUp();
        }

        public void VolumeDown()
        {
            _audioController.VolumeDown();
        }

        public void Zoom(int value)
        {
           // System.Windows.Threading.Dispatcher.Run();
                    
            _mouseController.Zoom(value);
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {

                    
                    /*
                    SendKeys.SendWait("CTRL down");
                    //_keyBoardController.PushKey(0x11, 0);
                    _mouseController.Scroll(value);

                    SendKeys.SendWait("CTRL up");
                    //_keyBoardController.ReleaseKey(0x11, 0);*/
                }));

        }

        private void StartSwitch()
        {
           // _keyBoardController.PushKey(0x11, 0x14); //CTRL
            _keyBoardController.PushKey(0x12, 0); //ALT
            _keyBoardController.PushKey(0x09, 0); //TAB
         //   _keyBoardController.ReleaseKey(0x11, 0x14);
            _keyBoardController.ReleaseKey(0x09, 0);
            
         //   SendKeys.SendWait("%TAB");
        }

        public void Switch(int val)
        {
            if (val == 0)
            {

                StartSwitch();
            //    SendKeys.SendWait("TAB");
            }
            else if (val == 1)
            {
                _keyBoardController.PushKey(0x09, 0); //TAB
                _keyBoardController.ReleaseKey(0x09, 0);
            }
            else if (val == 2)
            {
                _keyBoardController.PushKey(0xA0, 0);
                _keyBoardController.PushKey(0x09, 0); //TAB
                _keyBoardController.ReleaseKey(0x09, 0);
                _keyBoardController.ReleaseKey(0xA0, 0);
            }
            else
            {
                EndSwitch();
            }
        }

        private void EndSwitch()
        {
            //_keyBoardController.PushKey(0x0D, 0x5a); //ENTER
          //  _keyBoardController.ReleaseKey(0x0D, 0x5a);
           _keyBoardController.ReleaseKey(0x12, 0);
         //   SendKeys.SendWait("{Alt up}");
        }
        public void CloseWindow()
        {
            _windowControler.CloseActiveWindow();
        }
    }
}