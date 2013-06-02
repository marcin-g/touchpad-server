using System;
using System.Windows;



namespace touchpad_server
{
    /// <summary>
    /// Interaction logic for QRWindow.xaml
    /// </summary>
    public partial class QRWindow : Window
    {
        public QRWindow()
        {
           // qrControl.Text = "1";
            try
            {

                InitializeComponent();
            }
            catch (Exception)
            {
            }
        }

        private void QRGrid_OnLoaded(object sender, RoutedEventArgs e)
        {/*
            qrControl.Text = "QrCode.Net";

            BitMatrix qrMatrix = qrControl.GetQrMatrix();    //Qr Bit matrix for input string "QrCode.Net"

            
            qrControl.Lock();
            qrControl.Freeze();
            qrControl.LightColor = Colors.Yellow;  //It won't redraw. 
            qrControl.Text = "Lock & Freeze Test";  //It won't re-encode
            qrControl.Unlock();    //Unlock class, re-encode but won't redraw
            qrControl.UnFreeze();   //unfreeze and redraw image*/

            //Unlock before UnFreeze. Else it will redraw image twice. 


           // BitMatrix qrMatrix = QrControl.GetQrMatrix(); //Qr bit matrix for input string "QrCode.Net".
            /*
            QrEncoder encoder = new QrEncoder();
            QrCode qrCode;
            encoder.TryEncode("Test", out qrCode);
            QrControl.Freeze(); //Freeze class.
            QrControl.Text = "freeze test"; //Control will re-encode, but won't recreate image right away. 
            QrControl.UnFreeze(); //Recreate Image

            QrControl.Lock();  //Lock class.
            QrControl.ErrorCorrectLevel = ErrorCorrectionLevel.M;  //It won't encode and recreate image.
            QrControl.Text = "next test";
            qrMatrix = QrControl.GetQrMatrix(); //Qr bit matrix for input string "QrCode.Net".
            QrControl.QuietZoneModule = QuietZoneModules.Zero;  //Control will recreate image, but Bitmatrix is still for "QrCode.Net" input string. 
            QrControl.Unlock(); //Unlock class, re-encode and repaint. 
            */
            /*
            QrControl.Lock();
            QrControl.Freeze();
            QrControl.LightColor = Colors.White;  //It won't recreate image.
            QrControl.DarkColor = Colors.Black;
            QrControl.Text = "Lock and Freeze test";  //Control won't re-encode.
            qrMatrix = QrControl.GetQrMatrix();  //QrCode matrix is still for string "Freeze test"
            QrControl.Unlock();    //Unlock class, re-encode QrCode but won't recreate image
            QrControl.UnFreeze();  //UnFreeze class, recreate image.*/
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {/*
            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.L);
            QrCode qrCode;
            encoder.TryEncode(SocketConnection.CreateConnection(), out qrCode);

            WriteableBitmapRenderer wRenderer = new WriteableBitmapRenderer(
                new FixedModuleSize(2, QuietZoneModules.Two),
                Colors.Black, Colors.White);
            Console.WriteLine(qrCode.Matrix.Width+" "+qrCode.Matrix.Height);
            WriteableBitmap wBitmap = new WriteableBitmap(100,100, 192, 192, PixelFormats.Gray8, null);
            wRenderer.Draw(wBitmap, qrCode.Matrix);
            mainImage.Source = wBitmap;*/
            /*
            MemoryStream ms = new MemoryStream();
            wRenderer.WriteToStream(qrCode.Matrix, ImageFormatEnum.BMP, ms);
            
            //You can also use above wBitmap to encode to image file on your own. 

            FileStream wms = File.Create(@"code.png");
           // MemoryStream wms = new Me();
            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Interlace = PngInterlaceOption.On;
            pngEncoder.Frames.Add(BitmapFrame.Create(wBitmap));
            pngEncoder.Save(wms);
            wms.CloseNotBinded();*/

        }
    }
}
