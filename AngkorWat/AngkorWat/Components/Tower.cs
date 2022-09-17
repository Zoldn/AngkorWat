using AngkorWat.IO;
using AngkorWat.Tower;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Tower
    {
        public Dictionary<(int X, int Y, int Z), char> Points;

        public Tower()
        {
            Points = new Dictionary<(int X, int Y, int Z), char>();
        }

        public void Serialize()
        {
            var jsonPoints = new List<JsonPoint>();

            foreach (var ((x, y, z), c) in Points)
            {
                JsonPoint newOne = new()
                {
                    x = x,
                    y = y,
                    z = z,
                    name = c
                };

                jsonPoints.Add(newOne);
            }

            OutputContainer outputContainer = new OutputContainer("lesser tower")
            {
                letters = jsonPoints,
            };

            //string json = JsonConvert.SerializeObject(jsonPoints);
            string json = JsonConvert.SerializeObject(outputContainer);

            File.WriteAllText("output.json", json);

            Console.WriteLine("Collections are serialized");
        }

        public bool IsNotFalling()
        {
            int mass = Points.Count;

            double mx = Points.Sum(kv => kv.Key.X) / (double)mass;
            double my = Points.Sum(kv => kv.Key.Y) / (double)mass;

            //Points.Where(kv )

            //var minX = legs.Min(e => e);
            //var maxX = legs.Max(e => e);

            //return minX <= mx && mx <= maxX;

            return true;
        }

        public int IsNotCrumbling()
        {
            var cubesOnLevel = Points
                .GroupBy(e => e.Key.Z)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                    );

            var maxZ = Points.Max(kv => kv.Key.Z);

            List<int> failedLevels = new();

            for (int z = 0; z <= maxZ; z++)
            {
                int cubesOnCurrentLevel = cubesOnLevel[z];

                int cubesAboveCurrentLevel = cubesOnLevel
                    .Where(kv => kv.Key > z)
                    .Sum(kv => kv.Value);

                if (cubesAboveCurrentLevel > 50 * cubesOnCurrentLevel)
                {
                    failedLevels.Add(z);// return z;
                }
            }

            return failedLevels.Count;
        }
    }
}
