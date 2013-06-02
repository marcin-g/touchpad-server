using System;
using System.Windows;
using touchpad_server.IO;

namespace touchpad_server
{
    /// <summary>
    /// Interaction logic for PortWindow.xaml
    /// </summary>
    public partial class PortWindow : Window
    {
        public PortWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int i = 0;
            if (Int32.TryParse(PortText.Text, out i))
            {
                SocketConnection.ConnectionPort = i;
                this.Close();
            }
        }
    }
}
