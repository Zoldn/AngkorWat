using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Tower
{
    internal class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public char C { get; set; }
        public Point()
        {
            X = 0;
            Y = 0;
            Z = 0;
            C = ' ';
        }
    }

    internal class Tower
    {
        public List<Point> Points { get; set; }
        public Dictionary<(int, int, int), char> NewPoints { get; set; }
        public List<string> UsedWords { get; set; }
        public Tower()
        {
            Points = new List<Point>();
            UsedWords = new List<string>();

            NewPoints = new Dictionary<(int, int, int), char>();
        }

        public void UpdatePoints()
        {
            Points = NewPoints
                .Select(kv => new Point()
                {
                    X = kv.Key.Item1,
                    Y = kv.Key.Item2,
                    Z = kv.Key.Item3,
                    C = kv.Value,
                })
                .ToList();
        }
    }
}
