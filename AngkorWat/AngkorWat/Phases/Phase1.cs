using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal static class Phase1
    {
        public async static Task Phase1Start()
        {
            var universeState = await HttpHelper.Get<UniverseState>(
                "https://datsedenspace.datsteam.dev/player/universe");
        }
    }
}
