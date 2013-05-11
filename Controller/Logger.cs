using System;
using System.Windows.Controls;

namespace touchpad_server.Controller
{
    public class Logger
    {
        private static TextBox TextBox;

        public static void SetTextBox(TextBox textBox)
        {
            TextBox = textBox;
        }

        public static void Log(String text)
        {
            MainWindow.myInstance.Dispatcher.BeginInvoke(
                new Action(delegate { MainWindow.myInstance.LogBox.Text += "\n" + text; }));
        }
    }
}