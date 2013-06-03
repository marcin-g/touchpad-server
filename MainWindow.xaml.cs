using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using touchpad_server.Controller;
using touchpad_server.IO;

namespace touchpad_server
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow myInstance;
        private readonly FrameInterpreter interpreter = new FrameInterpreter();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private SocketConnection connection;
        private string qrString;
        QRWindow window;
        private PortWindow portWindow = new PortWindow();
        private bool started = false;

        public MainWindow()
        {
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new System.Drawing.Icon("Tray.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
            myInstance = this;
            InitializeComponent();
            window=new QRWindow();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Log("Koniec workera");
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                interpreter.BeginProccessing();
                qrString = SocketConnection.CreateConnection();

                myInstance.Dispatcher.Invoke(new Action(delegate()
                {
                    window = new QRWindow();
                        window.qrControl.Text = qrString;
                        window.Show();
                    }));
                connection = SocketConnection.GetConnectedSocket();
                myInstance.Dispatcher.Invoke(new Action(delegate() { window.Close();}));
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return;
            }
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (!started)
            {
                started = true;
                worker.RunWorkerAsync();
                StartButton.Content = "Stop";
            }
            else
            {
                if (connection != null)
                {
                    try
                    {
                        connection.CloseNotBinded();
                        connection.CloseBinded(); 
                    }
                    catch (Exception)
                    {
                    }

                }
                else
                {
                    SocketConnection.CloseOtherConnections(null);
                }
                Logger.CloseLogFile();
                StartButton.Content = "Start";
                QrButton.IsEnabled = false;
                started = false;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (connection != null)
            {
                try
                {
                    connection.CloseNotBinded();
                    connection.CloseBinded();
                }
                catch (Exception)
                {
                }

            }
            else
            {
                SocketConnection.CloseOtherConnections(null);
            }
            if (interpreter != null)
            {
                interpreter.EndProcessing();
            }
            window.Close();
                //portWindow.Close();
            portWindow.Close();
            Application.Current.Shutdown();
        }

        private void QrCode_Click(object sender, RoutedEventArgs e)
        {
            window=new QRWindow();
            myInstance.Dispatcher.Invoke(new Action(delegate()
            {
                window.qrControl.Text = qrString;
                window.Show();
            }));
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized && IsVisible)
            {
                this.Hide();
            }
            else
            {
                base.OnStateChanged(e);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            portWindow=new PortWindow();
            portWindow.ShowDialog();
        }
       /* protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...

            handled = true;
            return IntPtr.Zero;
        }*/

        public static void ClientsNumber(int clients)
        {
            myInstance.Dispatcher.Invoke(new Action(delegate
                {
                    myInstance.ClientsLabel.Content = clients;
                }));
        }
    }
}