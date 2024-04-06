using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal static class Phase3
    {
        public async static Task Run()
        {
            var universeState = await HttpHelper.Get<UniverseState>(
                "https://datsedenspace.datsteam.dev/player/universe") ?? throw new Exception();

            var ret = Dijkstra.Run(universeState, "Earth", "Eden");

            Console.WriteLine(string.Join("->", ret));
        }
    }
}
