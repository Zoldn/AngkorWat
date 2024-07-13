using Google.OrTools.Sat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    /// <summary>
    /// Внутреннее представление входных данных
    /// </summary>
    public class WorldState
    {
        [JsonProperty("staticWorld")]
        public StaticWorld StaticWorld { get; set; } = new();
        [JsonProperty("dynamicWorld")]
        public DynamicWorld DynamicWorld { get; set; } = new();
        public TurnCommand TurnCommand { get; set; } = new();
        public WorldState()
        {

        }

        internal void SetStaticData(StaticWorld output)
        {
            StaticWorld = output;

            StaticWorld.FillNullLists();
            StaticWorld.FillDict();

            foreach (var zpot in StaticWorld.ZPots)
            {
                zpot.ParseType();
            }
        }

        internal void SetDynamicData(DynamicWorld dynamicData)
        {
            DynamicWorld = dynamicData;
            DynamicWorld.FillNullLists();
            DynamicWorld.FillDicts();

            foreach (var zombie in DynamicWorld.Zombies)
            {
                zombie.ParseTypes();
            }
        }
    }
}
