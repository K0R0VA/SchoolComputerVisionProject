using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using Emgu.CV.UI;
using DirectShowLib;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace WpfApp4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public class Myimagebox : ImageBox
    {
        public byte r_high { get; set; }
        public byte g_high { get; set; }
        public byte b_high { get; set; }


        public byte r_low { get; set; }
        public byte g_low { get; set; }
        public byte b_low { get; set; }



        public byte r_high_2 { get; set; }
        public byte g_high_2 { get; set; }
        public byte b_high_2 { get; set; }


        public byte r_low_2 { get; set; }
        public byte g_low_2 { get; set; }
        public byte b_low_2 { get; set; }



        public System.Drawing.Rectangle Cutrec { get; set; }
        public System.Drawing.Image OrImg { get; set; }
        public System.Drawing.Point center { get; set; }
        public bool cen { get; set; }
        public bool DrRec { get; set; }
        public System.Drawing.Color col { get; set; }

        private System.Drawing.Rectangle cutrec = new System.Drawing.Rectangle();
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                cutrec.X = e.X;
                cutrec.Y = e.Y;
                col = Image.Bitmap.GetPixel(e.X, e.Y);
            }
            // правая кнопка мы создаем центр осей координат 
            if (e.Button == MouseButtons.Right)
            {
                center = new System.Drawing.Point(e.X, e.Y);
                cen = true;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (OrImg != null) { e.Graphics.DrawImage(OrImg, new System.Drawing.Point()); }
            if (DrRec) { Cutrec = cutrec; e.Graphics.DrawRectangle(Pens.Red, Cutrec); }
            if (cen) { e.Graphics.DrawEllipse(new System.Drawing.Pen(System.Drawing.Color.AliceBlue, 10), new System.Drawing.Rectangle(center, new System.Drawing.Size(5, 5))); }
        }


        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                DrRec = false;
                cutrec.Width = -cutrec.X + e.X;
                cutrec.Height = -cutrec.Y + e.Y;
                Invalidate();
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }
            if (e.Button != MouseButtons.Right)
            {
                DrRec = true;
                cutrec.Width = -cutrec.X + e.X;
                cutrec.Height = -cutrec.Y + e.Y;
                Invalidate();
            }
        }

        Colorsdetectrer colorsdetectrer = new Colorsdetectrer();

        public short[] rgb(Myimagebox myimagebox)
        {
            // здесь определяем rgb от которого нужно найти диапазон
            short[] rgb = new short[3];

            // если пользователь выделел область
            if (Cutrec.Width > 50 && Cutrec.Height > 50)
            {
                mypicture picture = new mypicture(myimagebox, Cutrec.Size);
                OrImg = picture.Image; Bitmap bit = new Bitmap(OrImg, Cutrec.Size);
                rgb = colorsdetectrer.rgb(bit, (short)myimagebox.Cutrec.Width, (short)myimagebox.Cutrec.Height);
                picture.Dispose();
                bit.Dispose();
            }
            // если просто кликул или немного выделил область 
            else
            {
                rgb[0] = col.R; rgb[1] = col.G; rgb[2] = col.B;
            }
            return rgb;
        }
    }







    /// 

    public partial class MainWindow : Window
    {
        
        VideoCapture Cam1;
        VideoCapture Cam2;
        bool f = false;
        public MainWindow()
        {
        }
        Myimagebox Cam2Box = new Myimagebox();
        Myimagebox Cam1Box = new Myimagebox();


        byte blur=1;
        Objectselect objectselect = new Objectselect();
        Colorsdetectrer colorsdetectrer = new Colorsdetectrer();
        bool c = false;
        bool d = false;
        short[] rgb = new short[3];
        short[] RgB = new short[3];
        List<System.Drawing.Point> points = new List<System.Drawing.Point>();
        List<System.Drawing.Point> points2 = new List<System.Drawing.Point>();
        List<Pointintime> pointintimes = new List<Pointintime>();
        List<Pointintime> pointintime = new List<Pointintime>();
        List<System.Drawing.Point> points3 = new List<System.Drawing.Point>();
        List<System.Drawing.Point> points4 = new List<System.Drawing.Point>();

  Timer timer = new Timer();
        private  void ProcessFrame(object sender,EventArgs e)
        {
                        Image<Bgr, byte> Imageframe = Cam1.QuerySmallFrame().ToImage<Bgr, byte>();
                        Cam1Box.Image = Imageframe;

                        // записываем в лист изображение кадры, чтобы потом можно было произвести их в 

                        // валфилтер для первого объекта
                        Image<Gray, byte> valfilter = Imageframe.Convert<Gray, Byte>();

                        // валфилтер для первого объекта
                        Image<Gray, byte> Valfilter1 = Imageframe.Convert<Gray, Byte>();

                        // передаем в orimg кадр с имеджбокса, именно с orimg мы берем пиксели, которые потом анализируем и находим нужный диапазон
                        Cam1Box.OrImg = Cam1Box.Image.Bitmap;

                     objectselect.imagechanger(Cam1Box, valfilter, Valfilter1, Imageframe, blur, c, d, points, points2, false);
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

      

        private void @return(object sender, RoutedEventArgs e)
        {

        }

        bool x = false;


        private void Playvid(object sender, RoutedEventArgs e)
        {

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Cam1Box.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            Cam1Box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

            
            myimg1.Child = Cam1Box;


            Cam2Box.FunctionalMode = Emgu.CV.UI.ImageBox.FunctionalModeOption.Minimum;
            Cam2Box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

            myimg2.Child = Cam2Box;
            new Thread(() =>
            {
                timer.Tick += ProcessFrame;
                timer.Interval = 1;
            })
            { IsBackground = true,
            Priority = ThreadPriority.Highest}.Start();
          
           timer.Enabled = false;
            

            DsDevice[] _SystemCamereas = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);
            GetCameras getCameras = new GetCameras();
            List<KeyValuePair<int, string>> CamList = new List<KeyValuePair<int, string>>();
            CamList = getCameras.CameraList();
            foreach (KeyValuePair<int, string> Camera in CamList)
            {
                ComboBoxCameraList.Items.Add(Camera);
            }
            foreach (KeyValuePair<int, string> Camera in CamList)
            {
                ComboBoxCameraList2.Items.Add(Camera);
            }

            ComboBoxCameraList.Text = ComboBoxCameraList.Items[0].ToString();


        }

        private void AddCamera_Click(object sender, RoutedEventArgs e)
        {
            if (f == false)
            {
                SelectCameras2.Visibility = System.Windows.Visibility.Visible;
                AddCamera.Visibility = System.Windows.Visibility.Hidden;
                Cam2Box.Visible = true;
                f = !f;

            }
            else
            {
                SelectCameras2.Visibility = System.Windows.Visibility.Collapsed;
                AddCamera.Visibility = System.Windows.Visibility.Visible;

                f = !f;
            }
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer.Enabled = !timer.Enabled;
        }


        private void ComboBoxCameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Cam1 = new VideoCapture(Convert.ToByte(ComboBoxCameraList.SelectedIndex));
        }

        private void ComboBoxCameraList2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ComboBoxCameraList2.SelectedIndex > -1)
            {
                
                Cam2 = new VideoCapture(Convert.ToByte(ComboBoxCameraList2.SelectedIndex));
            }
            
            
        }


        private void AddCamera2_Click(object sender, RoutedEventArgs e)
        {
            SelectCameras2.Visibility = System.Windows.Visibility.Collapsed;
            AddCamera.Visibility = System.Windows.Visibility.Visible;
            Cam2 = null;
            Cam2Box.Visible = false;
        }

        private void first_object(object sender, RoutedEventArgs e)
        {
            c = true;
            rgb = Cam1Box.rgb(Cam1Box);
            track(rgb);
        }
         void track(short[] rgb)
        {
            Hsvtrans hsvtrans = new Hsvtrans();
            byte[] high = hsvtrans.HIGH(rgb[0], rgb[1], rgb[2]);
            
            byte[] low = hsvtrans.LOW(rgb[0], rgb[1], rgb[2]);

            Cam2Box.r_high = Cam1Box.r_high = high[0];
            Cam2Box.r_high = Cam1Box.g_high = high[1];
            Cam2Box.b_high = Cam1Box.b_high = high[2];
            Cam2Box.r_low = Cam1Box.r_low = low[0];
            Cam2Box.g_low = Cam1Box.g_low = low[1];
            Cam2Box.b_low = Cam1Box.b_low = low[2];
        }
    }

}
