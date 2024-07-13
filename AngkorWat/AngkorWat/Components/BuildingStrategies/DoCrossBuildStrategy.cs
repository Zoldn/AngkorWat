using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.BuildingStrategies
{
    internal class DoCrossBuildStrategy : IBuildStrategy
    {
        public DoCrossBuildStrategy() { }
        public void AddCommand(WorldState worldState)
        {
            if (worldState.DynamicWorld.Base.Count == 0) { return; }

            BaseTile? head = worldState.DynamicWorld.Base.FirstOrDefault(tile => tile.IsHead);

            if (head == null) { return; }

            int maxX = worldState.DynamicWorld.Base.Max(r => r.X);
            int maxY = worldState.DynamicWorld.Base.Max(r => r.Y);
            int minX = worldState.DynamicWorld.Base.Min(r => r.X);
            int minY = worldState.DynamicWorld.Base.Min(r => r.Y);

            if (worldState.DynamicWorld.Player.Gold >= 4)
            {
                worldState.TurnCommand.BuildCommands.Add(new Coordinate() { X = maxX + 1, Y = head.Y });
                worldState.TurnCommand.BuildCommands.Add(new Coordinate() { X = minX - 1, Y = head.Y });
                worldState.TurnCommand.BuildCommands.Add(new Coordinate() { X = head.X, Y = minY - 1 });
                worldState.TurnCommand.BuildCommands.Add(new Coordinate() { X = head.X, Y = maxY + 1 });
            }
        }
    }
}
