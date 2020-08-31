using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp4
{
    class mypicture:PictureBox
    {
       public mypicture(WpfApp4.Myimagebox myimagebox,Size cut)
        {
            Size = cut;
            Image = new Bitmap(@"..\..\res\white");
            SizeMode = PictureBoxSizeMode.StretchImage;
            draw(myimagebox);
        }

        private void draw(Myimagebox myimagebox)
        {
            Visible = false;
            Size = myimagebox.Cutrec.Size;
            if (myimagebox.OrImg != null)
            {
                using (Graphics gr = Graphics.FromImage(Image))
                {
                    gr.DrawImage(myimagebox.OrImg, 0, 0, myimagebox.Cutrec, GraphicsUnit.Pixel);
                }
            }
           
        }
    }
}
