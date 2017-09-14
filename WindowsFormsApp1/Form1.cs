using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = bmp;

        }
        ArrayList alist = new ArrayList();
        Bitmap bmp = new Bitmap(640, 480);

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            alist.Add(p);
            redraw();

        }


        void redraw()
        {

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            int lens = alist.Count;
            Pen pen = new Pen(Brushes.Red,5);

            for(int i = 0; i < lens; i++)
            {
                Point p = (Point) alist[i];
                Point q = (Point)alist[(i+1)%lens];

                g.DrawLine(pen, p, q);


            }

            pictureBox1.Invalidate();




        }

        class myvector
        {
            float x;
            float y;
            float z;

           public  myvector(float a, float b, float c)
            {
                x = a;
                y = b;
                z = c;

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  Graphics g = Graphics.FromImage(bmp);

            int i = 320;
            int j = 240;
            //for (int j = 0; j < 480; j++)
             //   for ( int i = 0; i < 640; i++)
                {
                    float value=0.0f;
                    myvector dir=new myvector(0.0f,0.0f,0.0f)  ;
                    mytest(i, j,  ref value, ref dir);

                    Color c = Color.FromArgb((int)value, (int)value, (int)value);
            
                    
                }



            pictureBox1.Invalidate();
        }

        void mytest(int x,int y , ref float  val, ref myvector dir)
        {
            double dx=0, dy=0;

            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Brushes.Green, 1);

            for (int k = 0; k < 360; k = k + 10)
            {
                dx = Math.Cos(k*Math.PI/180);
                dy = Math.Sin(k*Math.PI/180);



                double A = dy;
                double B = -dx;
                double C = A * x + B * y;
                g.DrawLine(pen, new Point(x, y), new Point((int)(x + 50 * dx), (int)(y + 50 * dy)));
            }
            val = (x + y) % 255;

        }


    }
}
