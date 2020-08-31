using System;
using System.Collections.Generic;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Windows.Forms;

namespace WpfApp4
{
    class Objectselect
    {
        Bezie bezie = new Bezie();
        Hsvtrans hsvtrans = new Hsvtrans();



        // находим maxind contour and center of object
        object[] Select(Image<Gray, byte> valfilter)
        {
            Point xy = new Point();
            VectorOfVectorOfPoint contour = new VectorOfVectorOfPoint();
            Mat contourmatrix = new Mat();
            object[] objec = new object[3];
            CvInvoke.FindContours(valfilter, contour, contourmatrix, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            double maxperimetr = -1;
            short maxind = -1;

            for (short i = 0; i < contour.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(contour[i], true);
                if (perimeter > maxperimetr)
                {
                    maxperimetr = perimeter;
                    maxind = i;
                }
            }
            if (maxind >= 0)
            {
                var dot = CvInvoke.Moments(contour[maxind]);
                if (dot.M00 != 0)
                {
                    xy = new Point((int)(dot.M10 / dot.M00), (int)(dot.M01 / dot.M00));
                }
            }
            objec[0] = maxind;
            objec[1] = contour;
            objec[2] = xy;
            return objec;
        }  



        // основная обработка объекта
        public void imagechanger(Myimagebox myimagebox,Image<Gray, byte> valfilter, Image<Gray, byte> valfilter1, Image<Bgr, byte> Imageframe, byte blur,  bool c, bool d, List<Point> points, List<Point> points2, bool check, DataGridView datagridview = null, DataGridView datagridview2 = null)
        {

            // если выбран первый объект
            if (c == true)
            {
               // устанавливаем верхнюю и нижнюю границу myimagebox.g_hihg и тд - свойства myimagebox присваеваем им значения когда кликаем 1 объект
               CvInvoke.InRange(Imageframe, new ScalarArray( new MCvScalar(myimagebox.r_low, myimagebox.g_low, myimagebox.b_low)), 
               new ScalarArray(new MCvScalar(myimagebox.r_high, myimagebox.g_high, myimagebox.b_high)), valfilter);
                CvInvoke.MedianBlur(valfilter, valfilter, blur);

                // создаем массив объектов 0 - maxind, 1 - контур, 2 - центр выделенного объекта
                object[] o = Select(valfilter);
                //=============================//
                Point xy = new Point();
                xy = (Point)o[2];
                if (xy.X > 0 && xy.Y > 0)
                {
                    // добавлеяем в структуру "точка во времени" точку и время(которое сейчас на компуктере) 
                    points.Add(xy);
                   // datagridview.Rows.Add(new object[] { xy.X, xy.Y });
                }

                // рисуем линию по массиву точек - либо по безъе, если был нажат чекбокс, либо просто по точкам
                Imageframe.DrawPolyline(bezie.Rezult(points, check).ToArray(), false, new Bgr(255, 0, 0));
                
                //делаем  контур
                CvInvoke.DrawContours(Imageframe, (VectorOfVectorOfPoint)o[1], (short)o[0], new MCvScalar(255, 0, 0), 2);

                
            }


            // все тоже самое для второго выделенного объекта
            if (d == true)
            {
               

                CvInvoke.InRange(Imageframe, new ScalarArray(new MCvScalar(myimagebox.r_low_2, myimagebox.g_low_2, myimagebox.b_low_2)),
                new ScalarArray(new MCvScalar(myimagebox.r_high_2, myimagebox.g_high_2, myimagebox.b_high_2)), valfilter1);
                object[] os = Select(valfilter1);
                CvInvoke.MedianBlur(valfilter1, valfilter1, blur);
                //=============================//
                Point xy = new Point();
                xy = (Point)os[2];
                if (xy.X > 0 && xy.Y > 0)
                {
                    points2.Add(xy);

                    //datagridview2.Rows.Add(new object[] { xy.X, xy.Y });
                }
                //=============================//
                Imageframe.DrawPolyline(bezie.Rezult(points2, check).ToArray(), false, new Bgr(0, 0, 255));
                CvInvoke.DrawContours(Imageframe, (VectorOfVectorOfPoint)os[1], (short)os[0], new MCvScalar(0, 0, 255), 2);
               
            }
        }
    }
}
