﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            g.Clear(Color.Yellow);

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

        //   int i = 500;
        //    int j = 10;
           for (int j = 0; j < 480; j++)
                for ( int i = 0; i < 640; i++)
                {
                    float value=0.0f;
                    myvector dir=new myvector(0.0f,0.0f,0.0f)  ;
                    mytest(i, j,  ref value, ref dir);

                    Color c = Color.FromArgb((int)value, (int)value, (int)value);

                    Application.DoEvents();
                    pictureBox1.Invalidate();

                }



        }
        const double maxdis = Double.MaxValue;

        void mytest(int x,int y , ref float  val, ref myvector dir)
        {
            double dx=0, dy=0;

            Graphics g = Graphics.FromImage(bmp);
           
            int lens = alist.Count;

            bool allhave = true;


            for (int k = 0; k < 360; k = k + 10)
            {
                Pen pen = new Pen(new SolidBrush(Color.FromArgb(128, k% 255,0)), 1);
                Pen inpen = new Pen(new SolidBrush(Color.FromArgb(0, k % 255, 255)), 1);



                dx = Math.Cos(k*Math.PI/180);
                dy = Math.Sin(k*Math.PI/180);

                //https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines


                double A1 = dy;
                double B1 = -dx;
                double C1 = A1 * x + B1 * y;

                bool bfind = false;
                double nx=0, ny=0;

                double shortest = maxdis;
                double sx=x;
                double sy=y;

                for (int i = 0; i < lens; i++)
                {
                    Point p = (Point)alist[i];
                    Point q = (Point)alist[(i + 1) % lens];

                    double A2 = q.Y - p.Y;
                    double B2 = p.X - q.X;
                    double C2 = A2 * p.X + B2 * p.Y;

                    double  delta = A1 * B2 - A2 * B1;
                    if (delta == 0)
                        throw new ArgumentException("Lines are parallel");

                      nx = (B2 * C1 - B1 * C2) / delta;
                      ny = (A1 * C2 - A2 * C1) / delta;

                    double tx = 1;
                    double ty = 1;

                    if (Math.Abs(q.X - p.X) > 0)
                        tx = (nx - p.X) / (q.X - p.X);

                    if (Math.Abs(q.Y - p.Y) > 0)
                        ty = (ny - p.Y) / (q.Y - p.Y);


                    if (( tx >=0 && tx <=1) && ( ty >=0 && ty <=1))
                    {


                        double dis = Math.Sqrt((nx - x) * (nx - x) + (ny - y) * (ny - y));

                        if (dis < shortest)
                        {
                            shortest = dis;
                            sx = nx;
                            sy = ny;
                        }
                    }
                 
                }


                //與某線段找到交點
                if (shortest < maxdis)
                {
                    bfind = true;
                    g.DrawLine(pen, new Point(x, y), new Point((int)sx, (int)sy));
                   // break;
                }else //都沒有找到交點
                {
                    allhave = false;

                }

            }

            if (!allhave)
                bmp.SetPixel(x, y, Color.Red);
            //val = (x + y) % 255;

        }

        private void button2_Click(object sender, EventArgs e)
        {

            System.IO.StreamWriter sw = new System.IO.StreamWriter("out.txt");

            foreach (Point p in alist)
                sw.WriteLine(p);

            sw.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.IO.StreamReader sr= new System.IO.StreamReader("out.txt");

            alist.Clear();

            string str = sr.ReadLine();
            while(!String.IsNullOrEmpty(str) )
            {
                var g = Regex.Replace(str, @"[\{\}a-zA-Z=]", "").Split(',');

                Point pointResult = new Point(
                                  int.Parse(g[0]),
                                  int.Parse(g[1]));
                alist.Add(pointResult);
                str = sr.ReadLine();
            }

            sr.Close();

            redraw();

        }
    }
}
