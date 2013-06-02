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
    public partial class MainWindow : Window
    {
        public static MainWindow myInstance;
        private readonly FrameInterpreter interpreter = new FrameInterpreter();
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private SocketConnection connection;
        private string qrString;
        private object workerLock=new object();
        QRWindow window=new QRWindow();

        public MainWindow()
        {
            InitializeComponent();
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
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            interpreter.BeginProccessing();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkCombo.Items.Add(nic.Name);
            }
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Log("Koniec workera");
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                qrString = SocketConnection.CreateConnection();

                myInstance.Dispatcher.Invoke(new Action(delegate()
                    {
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


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int i = 0;
            if (Int32.TryParse(PortText.Text, out i) && AddressCombo.SelectedValue != null)
            {
                connection = new SocketConnection((IPAddress) AddressCombo.SelectedValue, Int32.Parse(PortText.Text));
                worker.RunWorkerAsync();
            }
        }

        private void NetworkCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddressCombo.Items.Clear();
            if (NetworkCombo.SelectedIndex >= 0)
            {
                foreach (
                    IPAddressInformation unicast in
                        NetworkInterface.GetAllNetworkInterfaces()[NetworkCombo.SelectedIndex].GetIPProperties()
                                                                                              .UnicastAddresses)
                {
                    AddressCombo.Items.Add(unicast.Address);
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            connection.CloseNotBinded();
            Logger.CloseLogFile();
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
            Logger.CloseLogFile();
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
           // base.OnStateChanged(e);
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
    }
}