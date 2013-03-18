using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using touchpad_server.DataModel;

namespace touchpad_server.Controller
{
    public class FrameInterpreter
    {
        private static readonly List<StandardFrame> _frameBuffor = new List<StandardFrame>();
        private static object _bufforLock=1;
        private bool _process;
        private Thread _mainThread;
        private MouseController _controller;
        
        public FrameInterpreter()
        {
            _process = true;
            _controller=new MouseController();
        }

        public static void AddFrame(StandardFrame frame)
        {
            Monitor.Enter(_bufforLock);
            try
            {
                _frameBuffor.Add(frame);
                Monitor.Pulse(_bufforLock);
            }
            finally
            {
                Monitor.Exit(_bufforLock);
            }
        }

        public void BeginProccessing()
        {
            _frameBuffor.Clear();
            _process = true;
            _mainThread=new Thread(ProccessingThread);
            _mainThread.IsBackground = true;
            _mainThread.Start();
        }
        private void ProccessingThread()
        {
            while (true)
            {
                StandardFrame tmpFrame = null;
                Monitor.Enter(_bufforLock);
                try
                {
                   if (_frameBuffor.Count>0)
                    {
                        tmpFrame = _frameBuffor[0];
                        _frameBuffor.Remove(tmpFrame);
                    }
                    if (tmpFrame == null)
                    {
                        Monitor.Wait(_bufforLock);
                    } 
                }
                finally
                {
                    Monitor.Exit(_bufforLock);
                    if (tmpFrame != null)
                    {
                        ProcessFrame(tmpFrame);
                    }
                }
            }

        }
        public void EndProcessing()
        {
            _mainThread.Abort();
        }

        private void ProcessFrame(StandardFrame frame)
        {
            switch (frame.Type)
            {
                case FrameType.CLICK:
                    _controller.LMBclick();
                    break;
                case FrameType.MOVE:
                    int offX=ConvertBytes(frame.Argument[0],frame.Argument[1],frame.Argument[2],frame.Argument[3]);
                    int offY=ConvertBytes(frame.Argument[4],frame.Argument[5],frame.Argument[6],frame.Argument[7]);
                    _controller.Move(offX,offY);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private int ConvertBytes(byte first, byte second, byte third, byte fourth)
        {
            byte[] array = {first, second, third, fourth};
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);

            return BitConverter.ToInt32(array, 0);
        }
    }
}
