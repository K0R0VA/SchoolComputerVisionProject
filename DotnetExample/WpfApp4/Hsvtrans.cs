using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace WpfApp4
{
    class Hsvtrans
    {
        // rgb -----> hsv получаем rgb возвращаем hsv (трогать не надо)
        short[] HsvArray(float r, float g, float b)
        {
            // получа
            float[] a = new float[3];
            float R = r / 255;
            float G = g / 255;
            float B = b / 255;
            // выясняем какой из цветов босс качалки
            a = Sravn(R, G, B);
            // выясняем какой из цветов босс качалки
            float h = 0, s, v = a[0] * 100;
            if (a[0] == 0)
            {
                s = 0;
            }
            else s = ((a[0] - a[1]) / a[0]) * 100;
            if (a[1] != a[0])
            {
                if (a[0] == R && G >= B)
                {
                    h = 60 * ((G - B) / (a[0] - a[1]));
                }
                if (a[0] == R && G < B)
                {
                    h = 60 * ((G - B) / (a[0] - a[1])) + 360;
                }
                if (a[0] == G)
                {
                    h = 60 * ((B - R) / (a[0] - a[1]) + 2);
                }
                if (a[0] == B)
                {
                    h = 60 * ((R - G) / (a[0] - a[1]) + 4);
                }
            }
            short[] hsvArray = new short[3] { (short)h, (short)s, (short)v };
            return hsvArray;
        }
        // hsv -----> rgb получаем hsv возвращаем rgb (трогать не надо)
        float[] rgbrad(short H, short s, short v)
        {
            float[] rgb = new float[3];
            float Vmin = ((100 - s) * v) / 100;
            float a = (v - Vmin) * (H % 60) / 60;
            float Vinc = (Vmin + a) * 2.55F;
            float Vdec = (v - a) * 2.55F;
            Vmin = Vmin * 2.55F;

            int h = (H / 60) % 6;
            float V = v * 2.55F;
            a = a * 2.55F;

            switch (h)
            {
                case 0:
                    rgb[0] = V;
                    rgb[1] = Vinc;
                    rgb[2] = Vmin;
                    break;
                case 1:
                    rgb[0] = Vdec;
                    rgb[1] = V;
                    rgb[2] = Vmin;
                    break;
                case 2:
                    rgb[0] = Vmin;
                    rgb[1] = V;
                    rgb[2] = Vinc;
                    break;
                case 3:
                    rgb[0] = Vmin;
                    rgb[1] = Vdec;
                    rgb[2] = V;
                    break;
                case 4:
                    rgb[0] = Vinc;
                    rgb[1] = Vmin;
                    rgb[2] = V;
                    break;
                case 5:
                    rgb[0] = V;
                    rgb[1] = Vmin;
                    rgb[2] = Vdec;
                    break;
            }
            return rgb;
        }
        //Здесь задается диапазон (можно трогать)
        float[] Low(short r, short g, short b)
        {
            short[] hsv = new short[3];
            //получаем rgb 
            hsv = HsvArray(r, g, b);
            //возвращаем hsv


            //задаем диапозон 

            // диапозон по h
            if (hsv[0] - 15 > 0) { hsv[0] = (short)(hsv[0] - 15); }
            else hsv[0] = (short)(360 - hsv[0] - 15);//

            // диапозон по s
            if (hsv[1] - 15 > 0) { hsv[1] = (short)(hsv[1] - 15); }
            else hsv[1] = 0;//

            // диапозон по v
            if (hsv[2] - 15 > 0) { hsv[2] = (short)(hsv[2] - 15); }
            else hsv[2] = 0;//
            float[] rgb = new float[3];


            // отдаем измененные hsv и получаем rgb
            rgb = rgbrad(hsv[0], hsv[1], hsv[2]);
            float[] Low = new float[3] { rgb[2], rgb[1], rgb[0] };
            return Low;
        }
        //Здесь задается диапазон  идентично лоу
        float[] High(short r, short g, short b)
        {
            short[] hsv = new short[3];
            hsv = HsvArray(r, g, b);
            if (hsv[0] + 15 < 360) { hsv[0] = (short)(hsv[0] + 15); }
            else hsv[0] = (short)(360 - hsv[0] + 15);
            if (hsv[1] + 15 < 100) { hsv[1] = (short)(hsv[1] + 15); }
            else hsv[1] = 100;
            if (hsv[2] + 15 < 0) { hsv[2] = (short)(hsv[2] + 15); }
            else hsv[2] = 100;
            float[] rgb = new float[3];
            rgb = rgbrad(hsv[0], hsv[1], hsv[2]).ToArray();
            float[] High = new float[3] { rgb[2], rgb[1], rgb[0] };
            return High;
        }
        // Нижняя граница 
        public byte[] LOW(short r, short g, short b)
        {
            float[] Lrgb = new float[3];

            // получем диапозон по rgb, в котором все уменьшали на 15 
            Lrgb = Low(r, g, b);

            float[] Hrgb = new float[3];


            // получем диапозон по rgb, в котором все уменьшали на 15 
            Hrgb = High(r, g, b);
            byte[] LOW = new byte[3];


            //  так как при создании диапозонов могло получиться так что лоу > хай делаем проверку 
            if (Hrgb[0] < Lrgb[0]) { LOW[0] = (byte)Hrgb[0]; } else LOW[0] = (byte)Lrgb[0];
            if (Hrgb[1] < Lrgb[1]) { LOW[1] = (byte)Hrgb[1]; } else LOW[1] = (byte)Lrgb[1];
            if (Hrgb[2] < Lrgb[2]) { LOW[2] = (byte)Hrgb[2]; } else LOW[2] = (byte)Lrgb[2];
            return LOW;
        }
        // Верхняя граница идентично лоу
        public byte[] HIGH(short r, short g, short b)
        {
            float[] Lrgb = new float[3];
            Lrgb = Low(r, g, b);
            float[] Hrgb = new float[3];
            Hrgb = High(r, g, b);
            byte[] HIGH = new byte[3];
            if (Hrgb[0] > Lrgb[0]) { HIGH[0] = (byte)Hrgb[0]; } else HIGH[0] = (byte)Lrgb[0];
            if (Hrgb[1] > Lrgb[1]) { HIGH[1] = (byte)Hrgb[1]; } else HIGH[1] = (byte)Lrgb[1];
            if (Hrgb[2] > Lrgb[2]) { HIGH[2] = (byte)Hrgb[2]; } else HIGH[2] = (byte)Lrgb[2];
            return HIGH;
        }
        // узнаем основной цвет входящего изображения 
        float[] Sravn(float x, float y, float z)
        {
            float[] a = new float[3] { x, y, z };
            float min = a[0], max = 0;
            for (int i = 0; i < 3; i++)
            {
                if (a[i] > max) { max = a[i]; }
                if (a[i] < min) { min = a[i]; }
            }
            float[] Spavn = new float[2] { max, min };
            return Spavn;
        }
    }
}
