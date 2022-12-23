using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.DistSolver
{
    internal class DistanceSolver
    {
        private static readonly double AIR_SPEED = 70.0d;
        private static readonly double SNOW_SPEED = 10.0d;

        private readonly AllData allData;

        public DistanceSolver(AllData allData)
        {
            this.allData = allData;
        }

        public DistanceSolution Solve()
        {
            var solution = new DistanceSolution();

            var santa = new Santa();

            var locations = allData.Children
                .Select(c => c as ILocation)
                .Append(santa);

            foreach (var from in locations)
            {
                foreach (var to in locations)
                {
                    if (from.LocationId >= to.LocationId)
                    {
                        continue;
                    }

                    var route = new Route(from, to);

                    CalculateRouteStraightDistance(route);

                    solution.Routes.Add((from, to), route);

                    solution.Routes.Add((to, from), route.AsReverse());
                }
            }

            return solution;
        }

        private void CalculateRouteStraightDistance(Route route)
        {
            route.TravelTime = 0.0d;

            var totalLenght = GetDistance(route.From, route.To);
            var snowLength = 0.0d;

            foreach (var snowArea in allData.SnowAreas)
            {
                snowLength += GetOverlapWithSnowArea(snowArea, route.From, route.To);
            }

            var cleanLength = totalLenght - snowLength;

            route.TravelTime = cleanLength / AIR_SPEED + snowLength / SNOW_SPEED;
        }

        public static double GetDistance(IPunkt from, IPunkt to)
        {
            return Math.Sqrt((from.X - to.X) * (from.X - to.X) + (from.Y - to.Y) * (from.Y - to.Y));
        }

        public static double GetOverlapWithSnowArea(SnowArea snowArea, IPunkt from, IPunkt to)
        {
            var alpha = to.X - from.X;
            var beta = to.Y - from.Y;

            var a = (alpha * alpha + beta * beta);
            var b = 2 * alpha * (from.X - snowArea.X) + 2 * beta * (from.Y - snowArea.Y);
            var c = Math.Pow(from.X - snowArea.X, 2) + Math.Pow(from.Y - snowArea.Y, 2) -
                snowArea.R * snowArea.R;

            var D = b * b - 4 * a * c;

            if (D <= 0)
            {
                return 0.0d;
            }

            double t1 = (-b - Math.Sqrt(D)) / 2 / a;
            double t2 = (-b + Math.Sqrt(D)) / 2 / a;

            var clampedT1 = Math.Clamp(t1, 0.0d, 1.0d);
            var clampedT2 = Math.Clamp(t2, 0.0d, 1.0d);

            return (clampedT2 - clampedT1) * GetDistance(from, to);
        }

        public static void TestOverlap()
        {
            var snowArea = new SnowArea(new IO.RawSnowArea() { x = 1, y = 1, r = 1});

            var from = new DerPunkt() { X = 0, Y = 0 };
            var to = new DerPunkt() { X = -2, Y = 2 };

            var dist = GetOverlapWithSnowArea(snowArea, from, to);
        }
    }
}
