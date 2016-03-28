using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Vidja;

namespace GalaxySpiralAnimation
{
    class Program
    {
        static void Main(string[] args)
        {
            Vidja.Vidja.Generate(new GalaxySpiral());
        }
    }

    internal class GalaxySpiral : IVidja
    {

        public int Width { get; } = 1920;
        public int Height { get; } = 1080;
        public int Fps { get; } = 30;
        public double Duration { get; } = 10;

        private const int StarSpacing = 35;
        private const float Tau = (float) (2*Math.PI);
        private readonly int _halfHeight;
        private readonly int _halfWidth;

        public GalaxySpiral()
        {
            _halfHeight = Height/2;
            _halfWidth = Width/2;
        }

        public Bitmap RenderFrame(double t)
        {
            var bmp = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            var graphics = Graphics.FromImage(bmp);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;


            for (int r = 100; r < _halfHeight; r+=StarSpacing)
            {
                var majorAxis = CalcMajorAxis(r,t);

                float minorAxis = r;
                var circumference = Tau*r;
                var starsPerOrbit = (int)circumference/StarSpacing;

                var deltaTheta = Tau/starsPerOrbit;
                var orbitalVelocity = 5*1/Math.Sqrt(r);

                for (double theta = 0; theta < Tau; theta+=deltaTheta)
                {
                    var angle = 20*t/r;
                    var thetaT = theta +t* orbitalVelocity;
                    // x' = a*cos(t)*cos(theta) - b*sin(t)*sin(theta)  
                    var x = (float) (majorAxis*Math.Cos(thetaT)*Math.Cos(angle) - minorAxis*Math.Sin(thetaT)*Math.Sin(angle));

                    // y' = a*cos(t)*sin(theta) + b*sin(t)*cos(theta)
                    var y = (float) (majorAxis*Math.Cos(thetaT)*Math.Sin(angle) + minorAxis*Math.Sin(thetaT)*Math.Cos(angle));

                    x = x + _halfWidth;
                    y = y + _halfHeight;

                    graphics.FillCircle(Brushes.White,x,y,10);
                }
            }

            return bmp;
        }

        private float CalcMajorAxis(int r, double t)
        {
            var maxR = r*(16f/9f);
            var diffR = maxR - r;
            return (float) (r + diffR*(t/10));
        }
    }
}
