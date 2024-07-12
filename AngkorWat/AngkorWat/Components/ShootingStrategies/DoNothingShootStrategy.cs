using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.ShootingStrategies
{
    internal class DoNothingShootStrategy : IShootStrategy
    {
        public DoNothingShootStrategy() { }
        public void AddCommand(WorldState worldState) { }
    }
}
