using System;
using System.IO;
using System.Windows.Controls;

namespace touchpad_server.Controller
{
    public class Logger
    {
        private static TextBox TextBox;

        private static StreamWriter _stream=new StreamWriter(@"log.txt") ;

        public static void SetTextBox(TextBox textBox)
        {
            TextBox = textBox;
        }

        public static void Log(String text)
        {
            MainWindow.myInstance.Dispatcher.BeginInvoke(
                new Action(delegate { MainWindow.myInstance.LogBox.Text += "\n" + text; }));
        }

        public static void LogTime(String text)
        {
            _stream.WriteLine(text);
        }

        public static void CloseLogFile()
        {
            _stream.Close();
        }

    }
}