using AngkorWat.Algorithms;
using AngkorWat.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal static class Phase2
    {
        public static void TestRun()
        {
            var json = File.ReadAllText(@"C:\Users\User\Desktop\Хакатон\scrap.json");

            var scrap = JsonConvert.DeserializeObject<TravelResponse>(json) ?? throw new IOException();

            var items = scrap
                .PlanetGarbage
                .Select(g => new GarbageItem(g.Key, g.Value.Select(e => (e[0], e[1])).ToList()))
                .ToList();

            var solver = new PackingSolver();

            solver.Solve(8, 11, items);

            Console.WriteLine("");
        }
    }
}
