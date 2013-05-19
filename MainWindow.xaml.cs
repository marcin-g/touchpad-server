using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows;
using System.Windows.Controls;
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

        public MainWindow()
        {
            InitializeComponent();
            myInstance = this;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            interpreter.BeginProccessing();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                NetworkCombo.Items.Add(nic.Name);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Log("Koniec workera");
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (true)
                    connection.StartListening();
            }
            catch (Exception ex)
            {
                worker.Dispose();
                Logger.Log(ex.ToString());
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
            connection.Stop();
            Logger.CloseLogFile();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Logger.CloseLogFile();
        }
    }
}