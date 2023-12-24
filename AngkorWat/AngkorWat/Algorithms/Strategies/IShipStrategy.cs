using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    public interface IShipStrategy
    {
        public void UpdateCommands(Data data, List<ShipCommand> commands);

        public static List<ShipCommand> GenerateEmpty(Data data)
        {
            var commands = new List<ShipCommand>();

            foreach (var ship in data.CurrentScan.MyShips)
            {
                if (ship.HP <= 0)
                {
                    continue;
                }

                commands.Add(new ShipCommand(ship)
                {
                    ChangeSpeed = 0,
                    Rotate = null,
                    Shoot = null,
                });
            }

            return commands;
        }
    }
}
