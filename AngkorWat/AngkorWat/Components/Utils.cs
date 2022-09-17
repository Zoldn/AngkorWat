using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Point2D
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    internal class Plank
    {
        public Point2D From { get; set; }
        public Point2D To { get; set; }
        public bool IsXDirection { get; set; }
        public int PlankId { get; set; }
        public Plank(Point2D from, Point2D to)
        {
            From = from;   
            To = to;
            PlankId = 0;
        }
        public int Length
        {
            get
            {
                if (IsXDirection)
                {
                    return To.X - From.X + 1;
                }
                else
                {
                    return To.Y - From.Y + 1;
                }
            }
        }
    }

    internal static class Utils
    {
        public static List<Plank> Initialize(int shiftX, int shiftY, int N)
        {
            var planks = new List<Plank>();

            var firstPoint = new Point2D()
            {
                X = 0,
                Y = 0,
            };

            for (int i = 0; i < N; i++)
            {
                var secondPoint = new Point2D() { 
                    X = firstPoint.X, 
                    Y = firstPoint.Y, 
                };

                if (i % 2 == 0)
                {
                    secondPoint.X += shiftX;
                }
                else
                {
                    secondPoint.Y += shiftY;
                }

                planks.Add(new Plank(firstPoint, secondPoint)
                {
                    IsXDirection = i % 2 == 0,
                    PlankId = i,
                });

                firstPoint = secondPoint;
            }

            return planks;
        }

        public static HashSet<char> CalculateCommonChars(HashSet<string> words)
        {
            var letters = words
                .SelectMany(w => w.ToCharArray())
                .GroupBy(g => g)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                    );

            return letters
                .OrderByDescending(kv => kv.Value)
                .Take(15)
                .Select(kv => kv.Key)
                .ToHashSet();
        }
    }
}
