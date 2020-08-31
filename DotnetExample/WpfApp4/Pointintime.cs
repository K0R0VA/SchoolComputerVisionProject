using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;


namespace WpfApp4

{
    struct Pointintime
    {
        public Pointintime(Point point, DateTime date)
        {
            this.date = date;
            this.point = point;
            p3 = new Point3D();
            dis = false;
        }
        public DateTime date { get; set; }
        public Point point { get; set; }
        public Point3D  p3 {get;set;}
        public bool dis { get; set; }
          



        // создаем 3д поинт, если поинты одного объекта с разных камер находились в одной секунде
        public void intersect(DateTime date, DateTime time, Point one, Point sec,Point p,Point m)
        {
            if (date.Second == time.Second)
            {
                Point3D point = new Point3D(one.X-p.X,p.Y-one.Y, sec.X-m.X);
                p3 = point;
                dis = true;
            }
            else
            {
                dis = false;
            }
        }
    }
}
