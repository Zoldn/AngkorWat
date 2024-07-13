using AngkorWat.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.MoveCenterStrategies
{
    internal class MassCenter : IMoveCenterStrategy
    {
        public MassCenter() { }
        // constants
        public bool debugWrite = true;
        // end constants
        public void AddCommand(WorldState worldState)
        {

            int totalmass = worldState.DynamicWorld.Base.Count;
            int totalx = 0;
            int totaly = 0;
            worldState.DynamicWorld.Base.ForEach(tile =>
            {
                totalx += tile.X;
                totaly += tile.Y;

            });
            float x = (float)totalx / (float)totalmass;
            float y = (float)totaly / (float)totalmass;
            worldState.TurnCommand.MoveCommand = new Coordinate() { X = (int)Math.Round(x, 0), Y = (int)Math.Round(y, 0) };

            if (debugWrite)
            {
                Console.WriteLine($"new center coords: [{(int)Math.Round(y, 0)}, {(int)Math.Round(x, 0)}]");
                Console.WriteLine("\n");
            }
        }
    }
}
