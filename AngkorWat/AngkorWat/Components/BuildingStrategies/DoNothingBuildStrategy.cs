using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.BuildingStrategies
{
    internal class DoNothingBuildStrategy : IBuildStrategy
    {
        public DoNothingBuildStrategy() { }
        public void AddCommand(WorldState worldState) { }
    }
}
