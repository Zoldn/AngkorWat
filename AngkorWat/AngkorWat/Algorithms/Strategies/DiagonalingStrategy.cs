using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    public class DiagonalingStrategy : IShipStrategy
    {
        public int PivotPointX { get; set; }
        public int PivotPointY { get; set; }
        public int B => PivotPointY - PivotPointX;
        public bool IsOk { get; set; }
        public Directions Direction { get; set; }

        public DiagonalingStrategy(Data data) 
        {
            var commands = IShipStrategy.GenerateEmpty(data);

            IsOk = true;

            PivotPointX = (int)Math.Round(commands.Select(e => e.Ship.X).Average());
            PivotPointY = (int)Math.Round(commands.Select(e => e.Ship.Y).Average());

            if (commands.Select(e => e.Ship.Direction).Distinct().Count() != 1)
            {
                IsOk = false;
            }
            else
            {
                Direction = commands[0].Ship.Direction;
            }

            if (!commands.All(e => e.Ship.Speed == 0))
            {
                IsOk = false;
            }
        }
        public void UpdateCommands(Data data, List<ShipCommand> commands)
        {
            if (!IsOk)
            {
                return;
            }

            foreach (var command in commands)
            {
                if (Direction == Directions.NORTH
                    || Direction == Directions.SOUTH)
                {
                    int mult = Direction == Directions.NORTH ? 1 : -1;

                    if (command.Ship.Y < command.Ship.X + B)
                    {
                        if (command.Ship.Speed == 0)
                        {
                            command.ChangeSpeed = -mult;
                        }
                    }
                    else if (command.Ship.Y == command.Ship.X + B)
                    {
                        command.ChangeSpeed = -command.Ship.Speed;
                    }
                    else
                    {
                        if (command.Ship.Speed == 0)
                        {
                            command.ChangeSpeed = +mult;
                        }
                    }
                }

                if (Direction == Directions.WEST
                    || Direction == Directions.EAST)
                {
                    int mult = Direction == Directions.EAST ? 1 : -1;

                    if (command.Ship.X < command.Ship.Y - B)
                    {
                        if (command.Ship.Speed == 0)
                        {
                            command.ChangeSpeed = -mult;
                        }
                    }
                    else if (command.Ship.Y == command.Ship.X + B)
                    {
                        command.ChangeSpeed = -command.Ship.Speed;
                    }
                    else
                    {
                        if (command.Ship.Speed == 0)
                        {
                            command.ChangeSpeed = +mult;
                        }
                    }
                }
            }
        }
    }
}
