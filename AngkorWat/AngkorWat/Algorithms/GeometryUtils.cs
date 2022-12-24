using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms
{
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

        /// <summary>
        /// Длина пересечения круга с отрезком из from в to в км
        /// </summary>
        /// <param name="snowArea"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double GetOverlapWithSnowArea(SnowArea snowArea, IPunkt from, IPunkt to)
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
    }
}
