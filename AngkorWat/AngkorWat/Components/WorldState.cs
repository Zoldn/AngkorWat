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

        static WorldState stateNextTurnRevursive (int turns, WorldState fromState)
        {
            if (turns == 0) return fromState;

            WorldState ret = new() { StaticWorld = fromState.StaticWorld, TurnCommand = fromState.TurnCommand };
            DynamicWorld newDynamicWorld = new DynamicWorld() { }



            return ret;
        }
        public WorldState stateNextTurn (int turns = 1)
        {
            return stateNextTurnRevursive(turns, this);
        }
    }
}
