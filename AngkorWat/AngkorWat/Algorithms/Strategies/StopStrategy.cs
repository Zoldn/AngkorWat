using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    public class StopStrategy : IShipStrategy
    {
        public void UpdateCommands(Data data, List<ShipCommand> commands)
        {
            if (!commands.Any(c => c.Ship.HP > 0))
            {
                return;
            }

            int minAccelerate = commands
                .Where(c => c.Ship.HP > 0)
                .Select(c => c.Ship.MaxChangeSpeed)
                .Min();

            foreach (var command in commands)
            {
                if (command.Ship.HP == 0)
                {
                    continue;
                }

                if (command.Ship.Speed == -1)
                {
                    command.ChangeSpeed = 1;
                }
                else if (command.Ship.Speed > 0)
                {
                    if (command.Ship.Speed > minAccelerate)
                    {
                        command.ChangeSpeed = -minAccelerate;
                    }
                    else
                    {
                        command.ChangeSpeed = -command.Ship.Speed;
                    }
                }
                else
                {
                    /// Do nothing
                }
            }
        }
    }
}
