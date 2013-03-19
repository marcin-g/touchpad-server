using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextBox = System.Windows.Controls.TextBox;

namespace touchpad_server.Controller
{
    public class Logger
    {
        private static TextBox TextBox=null;
        public static void SetTextBox(TextBox textBox)
        {
            TextBox = textBox;
        }
        public static void Log(String text)
        {
            MainWindow.myInstance.Dispatcher.BeginInvoke(new Action(delegate() 
                  {
                     MainWindow.myInstance.LogBox.Text+="\n"+text;
                  }));
        }
    }
}
