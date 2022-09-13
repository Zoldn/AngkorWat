using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public List<string> UsedWords { get; set; }
        public Tower()
        {
            Points = new List<Point>();
            UsedWords = new List<string>();
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
        public String ValidationInfo()
        {
            return $"Falling: {isNotFalling()} " +
                $"Crumbling: {isNotCrumbling()} " +
                $"InArea: {InArea()} " +
                $"Connected: {isConnected()}";
        }

        public Boolean isNotFalling()
        {
            Point MassCenter = new();
            Point EdgeOuter = new();
            Point EdgeInner = new();
            EdgeInner.X = 53565;
            EdgeInner.Y = 53565;


            foreach (var point in Points)
            {
                MassCenter.X += point.X;
                MassCenter.Y += point.Y;
                MassCenter.Z += point.Z;
                if (point.Z == 0 && point.X > EdgeOuter.X) EdgeOuter.X = point.X;
                if (point.Z == 0 && point.Y > EdgeOuter.Y) EdgeOuter.Y = point.Y;
                if (point.Z == 0 && point.X < EdgeInner.X) EdgeInner.X = point.X;
                if (point.Z == 0 && point.Y < EdgeInner.Y) EdgeInner.Y = point.Y;
            }

            MassCenter.X /= Points.Count;
            MassCenter.Y /= Points.Count;
            MassCenter.Z /= Points.Count;

            return MassCenter.X <= EdgeOuter.X && MassCenter.Y <= EdgeOuter.Y &&
                 MassCenter.X >= EdgeInner.X && MassCenter.Y >= EdgeInner.Y;
        }

        public int isNotCrumbling()
        {
            Dictionary<int, int> CubesOnLevel = new();

            foreach (var point in Points)
            {
                if (CubesOnLevel.ContainsKey(point.Z))
                {
                    CubesOnLevel[point.Z]++;
                } else
                {
                    CubesOnLevel[point.Z] = 0;
                }
            }

            int i = 55355;
            int Sum = 0;
            while (i >= 0)
            {
                if (!CubesOnLevel.ContainsKey(i) && Sum > 0)
                {
                    return i;
                }
                if (CubesOnLevel.ContainsKey(i))
                {
                    if (CubesOnLevel[i] * 50 < Sum)
                    {
                        return i;
                    }
                    Sum += CubesOnLevel[i];
                }
                --i;
            }
            return i;
        }

        public Boolean InArea()
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

        public Boolean isConnected()
        {
            return true;
        }
        // Validation end
    }
}
