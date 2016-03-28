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
        public double Duration { get; } = 40;

        private const int StarSpacing = 20;
        private const float Tau = (float) (2*Math.PI);
        private const int MinR = 100;
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


            var scene01Time = CalcSceneTime(0, 10, t);
            var scene02Time = CalcSceneTime(10, 10, t);
            var scene03Time = CalcSceneTime(20, 10, t);
            var scene04Time = CalcSceneTime(30, 10, t);
            var size = (int) (_halfHeight*0.83);
            var rangeR = size - MinR;

            for (int r = MinR; r < size; r+=StarSpacing)
            {
                var majorAxis = CalcMajorAxis(r,scene02Time);

                float minorAxis = r;
                var circumference = Tau*r;
                var starsPerOrbit = (int)circumference/StarSpacing;

                var deltaTheta = Tau/starsPerOrbit;
                var orbitalVelocity = 5*1/Math.Sqrt(r);

                var foo =(double) (size - (r-MinR) - MinR)/rangeR;
                var angle = foo*scene03Time*0.3;

                for (double theta = 0; theta < Tau; theta+=deltaTheta)
                {
                    var thetaT = theta +t* orbitalVelocity;
                    // x' = a*cos(t)*cos(theta) - b*sin(t)*sin(theta)  
                    var x = (float) (majorAxis*Math.Cos(thetaT)*Math.Cos(angle) - minorAxis*Math.Sin(thetaT)*Math.Sin(angle));

                    // y' = a*cos(t)*sin(theta) + b*sin(t)*cos(theta)
                    var y = (float) (majorAxis*Math.Cos(thetaT)*Math.Sin(angle) + minorAxis*Math.Sin(thetaT)*Math.Cos(angle));

                    x = x + _halfWidth;
                    y = y + _halfHeight;

                    graphics.FillCircle(Brushes.White,x,y,5);
                }
            }

            graphics.DrawString("@kevpluck", new Font("Arial", 16), new SolidBrush(Color.White), 1800f, 1050f);
            return bmp;
        }

        private static double CalcSceneTime(double start, double duration, double t)
        {
            var end = start + duration;
            var sceneTime = 0d;

            if (t >= start)
            {
                if (t < end) sceneTime = t - start;
                if (t >= end) sceneTime = duration;
            }

            return sceneTime;
        }

        private float CalcMajorAxis(int r, double t)
        {
            var maxR = r*(16f/9f);
            var diffR = maxR - r;
            return (float) (r + diffR*(t/10));
        }
    }
}
