using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.MoveCenterStrategies
{
    internal class DoNothingMoveStrategy : IMoveCenterStrategy
    {
        public DoNothingMoveStrategy() { }
        public void AddCommand(WorldState worldState) { }
    }
}
