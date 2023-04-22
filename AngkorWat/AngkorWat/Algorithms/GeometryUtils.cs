using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms
{
    internal interface ICircle
    {
        public double X { get; }
        public double Y { get; }
        public double R { get; }
    }

    internal interface IPunkt
    {
        public double X { get; }
        public double Y { get; }
    }

    internal static class GeometryUtils
    {
        /// <summary>
        /// Расстояние между двумя абстрактными точками в км
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double GetDistance(IPunkt from, IPunkt to)
        {
            return Math.Sqrt((from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y));
        }

        public static double GetDistance(double x0, double y0, double x1, double y1)
        {
            return Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1));
        }

        /// <summary>
        /// Длина пересечения круга с отрезком из from в to в км
        /// </summary>
        /// <param name="snowArea"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double GetOverlapWithSnowArea(ICircle snowArea, IPunkt from, IPunkt to)
        {
            var alpha = to.X - from.X;
            var beta = to.Y - from.Y;

            var a = alpha * alpha + beta * beta;
            var b = 2.0d * alpha * (from.X - snowArea.X) + 2.0d * beta * (from.Y - snowArea.Y);
            var c = Math.Pow(from.X - snowArea.X, 2) + Math.Pow(from.Y - snowArea.Y, 2) -
                snowArea.R * snowArea.R;

            var D = b * b - 4 * a * c;

            if (D <= 0)
            {
                return 0.0d;
            }

            double t1 = (-b - Math.Sqrt(D)) / 2.0d / a;
            double t2 = (-b + Math.Sqrt(D)) / 2.0d / a;

            var clampedT1 = Math.Clamp(t1, 0.0d, 1.0d);
            var clampedT2 = Math.Clamp(t2, 0.0d, 1.0d);

            return (clampedT2 - clampedT1) * GetDistance(from, to);
        }

        /// <summary>
        /// Длина пересечения круга с отрезком из from в to в км
        /// </summary>
        /// <param name="snowArea"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double GetOverlapWithSnowArea(ICircle snowArea, IPunkt from, IPunkt to,
            out double outT1, out double outT2)
        {
            outT1 = 0.0d;
            outT2 = 0.0d;

            var alpha = to.X - from.X;
            var beta = to.Y - from.Y;

            var a = alpha * alpha + beta * beta;
            var b = 2.0d * alpha * (from.X - snowArea.X) + 2.0d * beta * (from.Y - snowArea.Y);
            var c = Math.Pow(from.X - snowArea.X, 2) + Math.Pow(from.Y - snowArea.Y, 2) -
                snowArea.R * snowArea.R;

            var D = b * b - 4 * a * c;

            if (D <= 0)
            {
                return 0.0d;
            }

            double t1 = (-b - Math.Sqrt(D)) / 2.0d / a;
            double t2 = (-b + Math.Sqrt(D)) / 2.0d / a;

            var clampedT1 = Math.Clamp(t1, 0.0d, 1.0d);
            var clampedT2 = Math.Clamp(t2, 0.0d, 1.0d);

            outT1 = clampedT1;
            outT2 = clampedT2;

            return (clampedT2 - clampedT1) * GetDistance(from, to);
        }

        public static bool IsPunktInSnowArea(ICircle snowArea, IPunkt punkt)
        {
            return (snowArea.X - punkt.X) * (snowArea.X - punkt.X) +
                (snowArea.Y - punkt.Y) * (snowArea.Y - punkt.Y) <= snowArea.R * snowArea.R;
        }

        /// <summary>
        /// Возвращает угол в полярных координтах точки to относительно from [-pi, pi]
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double GetPolarAngle(IPunkt from, IPunkt to)
        {
            var dx = to.X - from.X;
            var dy = to.Y - from.Y;

            return Math.Atan2(dy, dx);
        }

        public static double SumPolarAngles(double phi1, double phi2)
        {
            if (Math.Abs(phi1) > Math.PI
                || Math.Abs(phi2) > Math.PI
                )
            {
                throw new ArgumentOutOfRangeException();
            }

            double phi = phi1 + phi2;

            if (phi < -Math.PI)
            {
                phi += 2 * Math.PI;
            }

            if (phi > Math.PI)
            {
                phi -= 2 * Math.PI;
            }

            return phi;
        }

        public static double TotalLengthOfPath(List<IPunkt> punkts)
        {
            var sum = 0.0d;

            for (int i = 0; i < punkts.Count - 1; i++)
            {
                sum += GetDistance(punkts[i + 1], punkts[i]);
            }

            return sum;
        }

        internal static double AngleDistance(double phiFrom, double phiTo)
        {
            var dphi = Math.Abs(phiTo - phiFrom);

            if (dphi > Math.PI)
            {
                dphi = 2 * Math.PI - dphi;
            }

            return dphi;
        }

        internal static bool IsPointInArea(IPunkt punkt, double size)
        {
            return punkt.X >= 0 && punkt.X <= size
                && punkt.Y >= 0 && punkt.Y <= size;
        }
    }
}
