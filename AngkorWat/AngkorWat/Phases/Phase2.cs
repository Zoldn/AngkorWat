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
            var json = File.ReadAllText(@"C:\Users\User\Desktop\Хакатон\message (1).json");

            var scrap = JsonConvert.DeserializeObject<TravelResponse>(json) ?? throw new IOException();

            var items = scrap
                .PlanetGarbage
                .Select(g => new GarbageItem(g.Key, g.Value.Select(e => (e[0], e[1])).ToList()))
                .ToList();

            while (items.Any())
            {
                var solver = new PackingSolver();
                var ret = solver.Solve(8, 11, items, doRotate: false, minLimit: 60);

                HashSet<string> removedNames = ret.GarbageItems
                    .Where(e => e.IsTaken)
                    .Select(e => e.Name)
                    .ToHashSet();

                //foreach (var item in ret.GarbageItems.Where(r => r.IsTaken))
                //{
                //    //items.Remove(item);

                //}

                //items = items.Where(i => !removedNames.Contains(i.Name)).ToList();

                items.RemoveAll(i => removedNames.Contains(i.Name));
            }

            Console.WriteLine("");
        }
    }
}
