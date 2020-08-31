using System;
using System.Collections.Generic;
using System.Drawing;

namespace WpfApp4
{
    class Bezie
    {
        int Fuctorial(int n) // Функция вычисления факториала
        {
            int res = 1;
            for (int i = 1; i <= n; i++)
                res *= i;
            return res;
        }
        float Polinom(int i, int n, float t)// Функция вычисления полинома Бернштейна
        {
            return Fuctorial(n) / (Fuctorial(i) * Fuctorial(n - i)) * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, n - i);
        }


        // принимаем лист поинтов, изменяем его с шагом в три точки если пользователь нажал чекбокс место 3 точек становиться сто точек
        public List<Point> Rezult(List<Point> ishod, bool x)
        {
            byte div = 1;
            List<Point> rezult = new List<Point>();
            if (ishod.Count > 2)
            {
                for (short j = 2; j < ishod.Count; j += 3)
                { //начинаем пробег по листу с шагом в три точки
                    if (x) { div = 100; }
                    // эти три точки передаем в лист трип
                    List<Point> triple = new List<Point>() { ishod[j - 2], ishod[j - 1], ishod[j] };
                    List<Point> rez = new List<Point>();
                    // преобразуем 3 точки в сто
                    for (float t = 0; t < 1; t += 1F / div)
                    {
                        float ytmp = 0;
                        float xtmp = 0;
                        for (int i = 0; i < triple.Count; i++)
                        { // проходим по каждой точке
                            float b = Polinom(i, triple.Count - 1, t); // вычисляем наш полином Бернштейна
                            xtmp += triple[i].X * b; // записываем и прибавляем результат
                            ytmp += triple[i].Y * b;
                        }
                        rez.Add(new Point((short)xtmp, (short)ytmp));
                    }
                    rezult.AddRange(rez);
                }
                return rezult;
            }
            else return ishod;
        }
    }
}
