using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using touchpad_server.DataModel;

namespace touchpad_server.Controller
{
    public class FrameInterpreter
    {
        private static readonly List<StandardFrame> _frameBuffor = new List<StandardFrame>();
        private static readonly object _bufforLock = 1;
        private static readonly Command _command = new Command();
        private static int counter;
        private Thread _mainThread;
        private bool _process;
        Stopwatch sw=new Stopwatch();

        public FrameInterpreter()
        {
            _process = true;
        }

        public static void AddFrame(StandardFrame frame)
        {
            //  ProcessFrame(frame);

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
            _mainThread = new Thread(ProccessingThread);
            _mainThread.Priority = ThreadPriority.Highest;
            _mainThread.IsBackground = true;
            _mainThread.Start();
        }

        private void ProccessingThread()
        {
            try
            {
                while (true)
                {
                    sw.Reset();
                    sw.Start();
                    StandardFrame tmpFrame = null;
                    Monitor.Enter(_bufforLock);
                    try
                    {
                        if (_frameBuffor.Count > 0)
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
                            sw.Stop();
                            Logger.LogTime("BEGIN_PROCESS " + sw.ElapsedTicks);
                            ProcessFrame(tmpFrame);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
            }
        }

        public void EndProcessing()
        {
            _mainThread.Abort();
        }

        private static void ProcessFrame(StandardFrame frame)
        {
            Logger.Log(frame.Type.ToString() + " " + counter++);
            switch (frame.Type)
            {
                case FrameType.CLICK:
                    _command.LeftClick();
                    break;
                case FrameType.MOVE:
                    //_mouseController.Move(ConvertBytes(frame.Argument,0),ConvertBytes(frame.Argument,4));
                    _command.MoveCursor(ConvertBytes(frame.Argument, 0), ConvertBytes(frame.Argument, 4));

                    break;
                case FrameType.SCROLL:
                    _command.Scroll(ConvertBytes(frame.Argument, 0));
                    break;
                case FrameType.MUTE:
                    _command.Mute();
                    break;
                case FrameType.VOLUME_DOWN:
                    _command.VolumeDown();
                    break;
                case FrameType.VOLUME_UP:
                    _command.VolumeUp();
                    break;
                case FrameType.ZOOM:
                    _command.Zoom(ConvertBytes(frame.Argument, 0));
                    break;
                case FrameType.CLOSE:
                    _command.CloseWindow();
                    break;
                case FrameType.SWITCH:
                    _command.Switch(ConvertBytes(frame.Argument, 0));
                    break;
            }
        }

        private static int ConvertBytes(byte[] data, int index)
        {
            byte[] array = {data[index], data[index + 1], data[index + 2], data[index + 3]};
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array);
            int val = BitConverter.ToInt32(array, 0);
            Logger.Log("arg " + val);

            return val;
        }
    }
}