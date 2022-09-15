using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngkorWat.IO;
using Newtonsoft.Json;

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

    internal class JsonPoint
    {
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public char name { get; set; }
        public JsonPoint()
        {
            x = 0;
            y = 0;
            z = 0;
            name = ' ';
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

        // Serializer start
        public void Serialize()
        {
            var JsonPoints = new List<JsonPoint>();
            foreach (var point in Points)
            {
                JsonPoint newOne = new();
                newOne.x = point.X;
                newOne.y = point.Y;
                newOne.z = point.Z;
                newOne.name = point.C;

                JsonPoints.Add(newOne);
            }
            string json = JsonConvert.SerializeObject(JsonPoints);

            File.WriteAllText("output.json", json);

            Console.WriteLine("Collections are serialized");
        }
        // Serializer end

        // Validation start
        //public string ValidationInfo()
        //{
        //    return $"Falling: {isNotFalling()} " +
        //        $"Crumbling: {isNotCrumbling()} " +
        //        $"InArea: {InArea()} " +
        //        $"Connected: {isConnected()}";
        //}

        public void Print()
        {
            var minx = NewPoints.Min(kv => kv.Key.Item1);
            var maxx = NewPoints.Max(kv => kv.Key.Item1);

            var minz = NewPoints.Min(kv => kv.Key.Item3);
            var maxz = NewPoints.Max(kv => kv.Key.Item3);

            for (int z = maxz; z >= minz; z--)
            {
                for (int x = minx; x <= maxx; x++)
                {
                    if (NewPoints.TryGetValue((x, 0, z), out var c))
                    {
                        Console.Write(c);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();
            }
        }

        public bool IsNotFalling()
        {
            int mass = NewPoints.Count;

            double mx = NewPoints
                .Sum(kv => kv.Key.Item1) / (double)mass;

            var legs = NewPoints
                .Where(kv => kv.Key.Item3 == 0)
                .Select(kv => kv.Key.Item1)
                .ToList();

            var minX = legs.Min(e => e);
            var maxX = legs.Max(e => e);

            return minX <= mx && mx <= maxX;


            //Point MassCenter = new();
            //Point EdgeOuter = new();
            //Point EdgeInner = new();
            //EdgeInner.X = 53565;
            //EdgeInner.Y = 53565;


            //foreach (var point in Points)
            //{
            //    MassCenter.X += point.X;
            //    MassCenter.Y += point.Y;
            //    MassCenter.Z += point.Z;
            //    if (point.Z == 0 && point.X > EdgeOuter.X) EdgeOuter.X = point.X;
            //    if (point.Z == 0 && point.Y > EdgeOuter.Y) EdgeOuter.Y = point.Y;
            //    if (point.Z == 0 && point.X < EdgeInner.X) EdgeInner.X = point.X;
            //    if (point.Z == 0 && point.Y < EdgeInner.Y) EdgeInner.Y = point.Y;
            //}

            //MassCenter.X /= Points.Count;
            //MassCenter.Y /= Points.Count;
            //MassCenter.Z /= Points.Count;

            //return MassCenter.X <= EdgeOuter.X && MassCenter.Y <= EdgeOuter.Y &&
            //     MassCenter.X >= EdgeInner.X && MassCenter.Y >= EdgeInner.Y;
        }

        public int IsNotCrumbling()
        {
            var cubesOnLevel = NewPoints
                .GroupBy(e => e.Key.Item3)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Count()
                    );

            var maxZ = NewPoints.Max(kv => kv.Key.Item3);

            for (int z = 0; z <= maxZ; z++)
            {
                int cubesOnCurrentLevel = cubesOnLevel[z];

                int cubesAboveCurrentLevel = cubesOnLevel
                    .Where(kv => kv.Key > z)
                    .Sum(kv => kv.Value);

                if (cubesAboveCurrentLevel > 50 * cubesOnCurrentLevel)
                {
                    return z;
                }
            }

            return -1;

            //int i = 55355;
            //int Sum = 0;
            //while (i >= 0)
            //{
            //    if (!CubesOnLevel.ContainsKey(i) && Sum > 0)
            //    {
            //        return i;
            //    }
            //    if (CubesOnLevel.ContainsKey(i))
            //    {
            //        if (CubesOnLevel[i] * 50 < Sum)
            //        {
            //            return i;
            //        }
            //        Sum += CubesOnLevel[i];
            //    }
            //    --i;
            //}
            //return i;
        }

        public bool InArea()
        {
            var NotGood = new List<Point>();
            foreach (var point in Points)
            {
                if (point.X < 0 || point.Y < 0 || point.Z < 0)
                {
                    NotGood.Add(point);
                }
            }
            return NotGood.Count == 0;
        }

        public bool isConnected()
        {
            return true;
        }
        // Validation end
    }
}
