using System;
using System.Collections.Generic;
using System.Drawing;

namespace WpfApp4
{
    class Colorsdetectrer
    {
        // получаем картинку записываем цвета пискелей в двумерный лист
        private List<List<short>> list(Bitmap bit, short width, short height)
        {
            List<List<short>> list = new List<List<short>>();
            list.Add(new List<short>());
            list.Add(new List<short>());
            list.Add(new List<short>());
            list.Add(new List<short>());
            for (short i = 0; i < width; i++)
            {
                for (short j = 0; j < height; j++)
                {
                    if (bit.GetPixel(i, j).R < 254 && bit.GetPixel(i, j).B <= 254 && bit.GetPixel(i, j).G <= 254)
                    {
                        list[0].Add(bit.GetPixel(i, j).R);
                        list[1].Add(bit.GetPixel(i, j).G);
                        list[2].Add(bit.GetPixel(i, j).B);
                        list[3].Add(1);
                    }
                }
            }
            return list;
        }


        // узнаем самый популярный цвет 
        private List<List<short>> Fam(List<List<short>> list)
        {
            short k = 0;
            for (short i = 0; i < list[0].ToArray().Length; i++)
            {
                for (short j = 0; j < list[0].ToArray().Length; j++)
                {
                    if ((i != j) && x(list[0][i], list[0][j], list[1][i], list[1][j], list[2][i], list[2][j]))
                    {
                        k++;
                    }
                }
                list[3][i] = k;
                k = 0;
            }
            return list;
        }

        // возвращаем цвета с наибольшей популярностью 
        public short[] rgb(Bitmap bit, short width, short height)
        {
            List<List<short>> fam = new List<List<short>>();
            fam = Fam(list(bit, width, height));
            short max = 0;
            short z = 0;
            for (short i = 0; i < fam[0].ToArray().Length; i++)
            {
                if (max < fam[3][i])
                {
                    max = fam[3][i];
                    z = i;
                }
            }
            short[] rgb = new short[3];
            rgb[0] = fam[0][z];
            rgb[1] = fam[1][z];
            rgb[2] = fam[2][z];
            fam = null;
            return rgb;
        }


        //вспомогательный метод - узнаем похож ли пиксель на другие
        private bool x(short a, short b, short c, short d, short e, short f)
        {
            bool x = false;
            if ((Math.Abs(a - b) < 5) && (Math.Abs(c - d) < 5) && (Math.Abs(e - f) < 5)) { x = true; }
            return x;
        }
    }
}
