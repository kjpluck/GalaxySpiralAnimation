using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Vidja;

namespace GalaxySpiralAnimation
{
    class Program
    {
        static void Main(string[] args)
        {
            Vidja.Vidja.Generate(new GalaxySpiral(),"spiralgalaxy.gif",30);
        }
    }

    internal class GalaxySpiral : IVidja
    {

        public int Width { get; } = 960;
        public int Height { get; } = 540;
        public int Fps { get; } = 25;
        public double Duration { get; } = 30.96;

        private const int StarSpacing = 7;
        private const float Tau = (float) (2*Math.PI);
        private const int MinR = 50;
        private readonly int _halfHeight;
        private readonly int _halfWidth;
        private double _t;
        private Graphics _graphics;

        public GalaxySpiral()
        {
            _halfHeight = Height/2;
            _halfWidth = Width/2;
        }

        public Bitmap RenderFrame(double t)
        {
            _t = t;
            var bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            _graphics = Graphics.FromImage(bmp);
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;


            var size = (int) (_halfHeight*0.83);
            var rangeR = size - MinR;
            
            for (int r = MinR; r < size; r+=StarSpacing)
            {
                var majorAxis = CalcMajorAxis(r,10);

                float minorAxis = r;
                var starsPerOrbit = 100;

                var deltaTheta = Tau/starsPerOrbit;
                var orbitalVelocity = 0.13;

                var foo =(double) (size - (r-MinR) - MinR)/rangeR;
                //foo = SCurve(foo, 1);
                var angle = foo*10*0.3;

                for (double theta = 0; theta < Tau; theta+=deltaTheta)
                {
                    var thetaT = theta +t* orbitalVelocity;
                    // x' = a*cos(t)*cos(theta) - b*sin(t)*sin(theta)  
                    var x = (float) (majorAxis*Math.Cos(thetaT)*Math.Cos(angle) - minorAxis*Math.Sin(thetaT)*Math.Sin(angle));

                    // y' = a*cos(t)*sin(theta) + b*sin(t)*cos(theta)
                    var y = (float) (majorAxis*Math.Cos(thetaT)*Math.Sin(angle) + minorAxis*Math.Sin(thetaT)*Math.Cos(angle));

                    x = x + _halfWidth;
                    y = y + _halfHeight;

                    _graphics.FillCircle(Brushes.White,x,y,2);
                }
            }

            _graphics.DrawString("@kevpluck", new Font("Arial", 16), new SolidBrush(Color.White), 650f, 20f);
            return bmp;
        }

        private static float CalcMajorAxis(int r, double t)
        {
            var maxR = r*(16f/9f);
            var diffR = maxR - r;
            return (float) (r + diffR);
        }
        
    }

    
}
