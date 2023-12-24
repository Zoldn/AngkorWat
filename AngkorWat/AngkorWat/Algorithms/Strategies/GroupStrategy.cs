using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    public class GroupStrategy : IShipStrategy
    {
        public Directions Direction { get; set; } = Directions.NORTH;
        public int TargetSpeed { get; set; } = 0;
        public GroupStrategy() 
        {

        }
        public void UpdateCommands(Data data, List<ShipCommand> commands)
        {
            Rotate(commands);
            ChangeSpeed(commands);
        }

        private void ChangeSpeed(List<ShipCommand> commands)
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
                if (Math.Abs(command.Ship.Speed - TargetSpeed) >= minAccelerate)
                {
                    if (command.Ship.Speed > TargetSpeed)
                    {
                        command.ChangeSpeed = -minAccelerate;
                    }
                    else
                    {
                        command.ChangeSpeed = +minAccelerate;
                    }
                }
                else
                {
                    command.ChangeSpeed = TargetSpeed - command.Ship.Speed;

                    //if (command.Ship.Speed > TargetSpeed)
                    //{
                    //    command.ChangeSpeed = TargetSpeed - command.Ship.Speed;
                    //}
                    //else
                    //{
                    //    command.ChangeSpeed = TargetSpeed - command.Ship.Speed;
                    //}
                }
            }
        }

        private void Rotate(List<ShipCommand> commands)
        {
            foreach (var command in commands)
            {
                var ship = command.Ship;

                if (ship.HP == 0)
                {
                    continue;
                }

                if (ship.Direction == Direction)
                {
                    continue;
                }

                if (((int)ship.Direction + 1) % 4 == (int)Direction)
                {
                    command.Rotate = 90;
                    continue;
                }

                if (((int)ship.Direction + 3) % 4 == (int)Direction)
                {
                    command.Rotate = -90;
                    continue;
                }

                if (((int)ship.Direction + 2) % 4 == (int)Direction)
                {
                    command.Rotate = -90;
                    continue;
                }
            }
        }
    }
}
