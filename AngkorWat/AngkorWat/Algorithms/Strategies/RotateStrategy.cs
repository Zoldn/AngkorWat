using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    internal class RotateStrategy : IShipStrategy
    {
        public Directions Direction { get; set; }
        public RotateStrategy() { }
        public void UpdateCommands(Data data, List<ShipCommand> commands)
        {
            foreach (var command in commands)
            {
                var ship = command.Ship;


            }
        }
    }
}
