using AngkorWat.Components;
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
        public static void Phase1Start()
        {
            string mapId = "";

            var inputContainer = IOHelper.ReadInputData<int>("phase1_input.json");

            var data = PrepairData(mapId, inputContainer);

            var outputContainer = new BaseOutputContainer(mapId);

            IOHelper.SerializeResult(outputContainer);
        }

        public static Data PrepairData(string mapId, int rawData)
        {
            var data = new Data()
            {
                MapId = mapId,
            };

            return data;
        }
    }
}
