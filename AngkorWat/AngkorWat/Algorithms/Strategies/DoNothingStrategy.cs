using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    public class DoNothingStrategy : IShipStrategy
    {
        public DoNothingStrategy() { }
        public void UpdateCommands(Data data, List<ShipCommand> commands) { }
    }
}
