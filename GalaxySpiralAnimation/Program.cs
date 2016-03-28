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
            Vidja.Vidja.Generate(new GalaxySpiral());
        }
    }

    internal class GalaxySpiral : IVidja
    {

        public int Width { get; } = 1920;
        public int Height { get; } = 1080;
        public int Fps { get; } = 30;
        public double Duration { get; } = 60;

        private const int StarSpacing = 15;
        private const float Tau = (float) (2*Math.PI);
        private const int MinR = 100;
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


            var scene01Time = CalcSceneTime(0, 10, t);
            var scene02Time = CalcSceneTime(10, 10, t);
            var scene03Time = CalcSceneTime(20, 10, t);
            var scene04Time = CalcSceneTime(30, 10, t);
            var size = (int) (_halfHeight*0.83);
            var rangeR = size - MinR;

            FadeInOutText(0, "Here is an unperturbed galaxy.",ypos:990);
            FadeInOutText(3, "All the stars go around in more or less a circular orbit.",ypos:990);
            FadeInOutText(6, "If another galaxy is nearby it can distort those orbits.", ypos: 990);
            FadeInOutText(10, "The simplest kind of distortion is that they get stretched a bit.", ypos: 990);
            FadeInOutText(20, "The second thing that can happen is that the orbits can then get pulled around.", ypos: 990, duration:5);
            FadeInOutText(30, "Just those two things can produce a two arm spiral.", ypos: 990, duration:5);
            /*
The stars travelling around on their orbits are essentially undisturbed.
Sometimes lots of the orbits come quite close together where the spiral arms are.*/

            FadeInOutText(35, "The stars travelling around on their orbits are essentially undisturbed.", ypos: 990, duration: 5);
            FadeInOutText(40, "Sometimes orbits come close together creating the spiral arms.", ypos: 990, duration: 5);

            for (int r = MinR; r < size; r+=StarSpacing)
            {
                var majorAxis = CalcMajorAxis(r,scene02Time);

                float minorAxis = r;
                var circumference = Tau*r;
                var starsPerOrbit = (int)circumference/StarSpacing;

                var deltaTheta = Tau/starsPerOrbit;
                var orbitalVelocity = 5*1/Math.Sqrt(r);

                var foo =(double) (size - (r-MinR) - MinR)/rangeR;
                foo = SCurve(foo, 1);
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

                    _graphics.FillCircle(Brushes.White,x,y,3);
                }
            }

            _graphics.DrawString("@kevpluck", new Font("Arial", 16), new SolidBrush(Color.White), 1800f, 1050f);
            return bmp;
        }

        private static double CalcSceneTime(double start, double duration, double t)
        {
            if (t < start) return 0;

            var sceneTime = 0d;
            var end = start + duration;

            if (t < end) sceneTime = t - start;
            if (t >= end) sceneTime = duration;

            return sceneTime;
        }

        private static float CalcMajorAxis(int r, double t)
        {
            var proportion = SCurve(t, 10);
            var maxR = r*(16f/9f);
            var diffR = maxR - r;
            return (float) (r + diffR*proportion);
        }

        private static double SCurve(double t, double range)
        {
            var proportion = 1 - (t/range);
            var cosArg = proportion*Math.PI;
            var cosValue = Math.Cos(cosArg);
            return (1 + cosValue)/2;
        }

        private void FadeInOutText(int start, string text, int duration = 3, int fontsize = 35, int ypos = 0)
        {
            if (NotNow(start, duration)) return;

            var t = _t - start;

            var colour = VidjaEasing.FadeInOutColour(t, duration, Color.FromArgb(255,255,255));
            var brush = new SolidBrush(colour);
            var font = new Font("Arial", fontsize);

            var layoutRectangle = new Rectangle((int)(Width * .05), 0, (int)(Width * .9), Height);
            if (ypos != 0)
            {
                var layoutSize = _graphics.MeasureString(text, font);
                layoutRectangle.Y = ypos;
                layoutRectangle.Height = (int)layoutSize.Height;
            }
            else
                _graphics.FillRectangle(new SolidBrush(Color.FromArgb(colour.A / 2, 255, 255, 255)), 0, 0, Width, Height);

            _graphics.DrawString(text, font, brush, layoutRectangle, _centeredText);
        }

        private readonly StringFormat _centeredText = new StringFormat
        {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
        };

        private bool NotNow(double start, double duration)
        {
            return _t < start || _t > start + duration;
        }
    }



    internal class VidjaEasing
    {
        /// <summary>
        /// Fades in for .5 sec and fades out .5 sec before duration completes
        /// </summary>
        /// <param name="t">0 &lt; t &lt; duration</param>
        /// <param name="duration">duration &gt; 1</param>
        public static Color FadeInOutColour(double t, double duration, Color color)
        {
            if (t < 0.5)
            {
                var alpha = (byte)(255 * t * 2);
                return Color.FromArgb(alpha, color.R, color.G, color.B);
            }
            if (t > duration - 0.5)
            {
                var tt = t - duration + 0.5;
                var alpha = (int)(255 - (255 * tt * 2));
                return Color.FromArgb(alpha, color.R, color.G, color.B);
            }
            return Color.FromArgb(255, color.R, color.G, color.B);
        }
    }
}
