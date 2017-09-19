using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
            for ( int i = 0; i < 640*480; i++)
               dir[i] = new myvector(0, 0);

        }
        ArrayList alist = new ArrayList();

        Bitmap bmp = new Bitmap(640, 480);
      
         myvector [] dir = new myvector[640*480];
        float[] values = new float[640 * 480];
        const double maxdis = Double.MaxValue;
        const float SCALE = 30;

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

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = data.Stride;
           unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                // Check this is not a null area
                //if (!areaToPaint.IsEmpty)
                {
                    int ind = 0;
                    // Go through the draw area and set the pixels as they should be
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            if (values[ind] >= 0)
                            {

                                Color c = generateRGB(values[ind]);
                                // layer.GetBitmap().SetPixel(x, y, m_colour);
                                ptr[(x * 3) + y * stride] = c.B;
                                ptr[(x * 3) + y * stride + 1] = c.G;
                                ptr[(x * 3) + y * stride + 2] = c.R;
                            }
                            ind++;
                        }
                    }
                }
            }
            bmp.UnlockBits(data);



            pictureBox1.Invalidate();




        }

        class myvector
        {
           public float x;
           public float y;
        //   public float z;

           public  myvector(float a, float b)
            {
                x = a;
                y = b;
           //     z = c;

            }

        }

        private Color generateRGB(double X)
        {
            Color color;

            int red;
            int green;
            int blue;
            TransColor.HsvToRgb(X * 360, 1, 1, out red, out green, out blue);

            color = Color.FromArgb(red, green, blue);

            return color;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  Graphics g = Graphics.FromImage(bmp);

            //   int i = 270;
            //    int j = 140;
            float myv = -1;

            //  myvector dir = new myvector(0.0f, 0.0f);

            DateTime t1 = DateTime.Now;

            int idx;

            for (int j = 0; j < 480; j++)
                for ( int i = 0; i < 640; i++)
                {
                  //  float value=0.0f;

                    idx = j * 640 + i;

                    mytest(i, j,  ref values[idx], ref dir[idx]);

            //        if ( values[idx] > 0 )
            //        {

            //            myv = values[idx];
            //            Color c = generateRGB(myv);
            ////            bmp.SetPixel(i, j, c);
            //           // Debug.WriteLine(myv);
            //        }

                    //Color c = Color.FromArgb((int)value, (int)value, (int)value);



                  //  Application.DoEvents();
                  //  pictureBox1.Invalidate();

                }


            DateTime t2 = DateTime.Now;
            TimeSpan span = t2 - t1;
            Debug.WriteLine("finish in " + span.ToString());

            redraw();

            for (int j = 0; j < 10; j++)
                for (int i = 0; i < 100; i++)
                {
                    Color c = generateRGB(i / 100.0f);
                    bmp.SetPixel(i, j, c);

                }


//            Debug.WriteLine(myv);
        }
       
        void mytest(int x,int y , ref float  val, ref myvector dir)
        {
            double dx=0, dy=0;

            Graphics g = Graphics.FromImage(bmp);
           
            int lens = alist.Count;

            bool allhave = true;

            float  retx = 0;
            float rety = 0;


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

               // bool bfind = false;
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
                    double tsx = 1;
                    double tsy = 1;


                    if (Math.Abs(q.X - p.X) > 0)
                        tx = (nx - p.X) / (q.X - p.X);

                    if (Math.Abs(q.Y - p.Y) > 0)
                        ty = (ny - p.Y) / (q.Y - p.Y);

                    if ( Math.Abs(dx) >0.00001)
                    tsx = (nx - x) / dx;
                    if ( Math.Abs(dy)>0.00001)
                    tsy = (ny - y) / dy;


                    if (( tx >=0 && tx <=1) && ( ty >=0 && ty <=1) && tsx >=0 && tsy >=0)
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
                    //    bfind = true;
                    if (Math.Abs(dx) > 0.0001)
                        retx +=(float) (dx / shortest);

                    if (Math.Abs(dy) > 0.0001)
                        rety += (float)(dy / shortest);


                    //    g.DrawLine(pen, new Point(x, y), new Point((int)sx, (int)sy));
                    // break;
                }
                else //都沒有找到交點,只要找到一個即是外部點
                {
                    allhave = false;
                    break;

                }

            }

            if (!allhave)
            {
                dir.x = 0;
                dir.y = 0;
                val = -1;
    //           bmp.SetPixel(x, y, Color.White);
            }else
            {
                val =(float) Math.Sqrt(retx * retx + rety * rety);

                dir.x = retx/val;
                dir.y = rety/val;

     //           Debug.WriteLine(val);
                val = val * 10;
                if (val > SCALE)
                    val = SCALE;
                val = val / SCALE;

            }
            //val = (x + y) % 255;

        }

        private void button2_Click(object sender, EventArgs e)
        {

            System.IO.StreamWriter sw = new System.IO.StreamWriter("out.txt");

            foreach (Point p in alist)
                sw.WriteLine(p);
            
            sw.Close();

            System.IO.StreamWriter sw2 = new System.IO.StreamWriter("vector.txt");

            foreach (myvector pv in dir)
                sw2.WriteLine("{0} {1}",pv.x,  pv.y );

            sw2.Close();


            System.IO.StreamWriter sw3 = new System.IO.StreamWriter("values.txt");

            foreach (float v in values)
                sw3.WriteLine( v);

            sw3.Close();

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


            System.IO.StreamReader sw2 = new System.IO.StreamReader("vector.txt");

             str = sw2.ReadLine();
            int ind = 0;
            while (!String.IsNullOrEmpty(str))
            {
                
                String [] sub=str.Split(' ');
                dir[ind].x=float.Parse(sub[0]);
                dir[ind].y = float.Parse(sub[1]);

                ind++;
                str = sw2.ReadLine();

            }
            sw2.Close();


            System.IO.StreamReader sw3 = new System.IO.StreamReader("values.txt");

            str = sw3.ReadLine();
            ind = 0;
            while (!String.IsNullOrEmpty(str))
            {
                values[ind] = float.Parse(str);
                ind++;
                str = sw3.ReadLine();

            }

            sw3.Close();



            redraw();

        }

        private void button4_Click(object sender, EventArgs e)
        {


        }
    }
}
