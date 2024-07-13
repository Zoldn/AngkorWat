using AngkorWat.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.MoveCenterStrategies
{
    internal class EvadeToBorder : IMoveCenterStrategy
    {
        public EvadeToBorder() { }
        // constants
        public bool debugWrite = false;
        // end constants
        public void AddCommand(WorldState worldState)
        {
            if (worldState.DynamicWorld.Base.Count == 0) { return; }
            BaseTile head = worldState.DynamicWorld.Base.Find(tile => tile.IsHead);
            if (head == null) { return; }

            var predictor = new ZombieTurnPredictor();
            var nextTurn = predictor.GetNextTurnWorld(worldState, worldState.DynamicWorld);

            int totalmass = worldState.DynamicWorld.Base.Count;
            int totalx = 0;
            int totaly = 0;
            worldState.DynamicWorld.Base.ForEach(tile =>
            {
                totalx += tile.X;
                totaly += tile.Y;

            });
            float centerx = (float)totalx / (float)totalmass;
            float centery = (float)totaly / (float)totalmass;

            List<BaseTile> tilesSafeFromZombies = worldState.DynamicWorld.Base.Where(tile =>
            {
                var futureTile = nextTurn.Base.Find(item => item.Id == tile.Id);
                if (futureTile == null || futureTile.Health < tile.Health)
                {
                    return false;
                }
                return true;
            }).ToList();

            List<BaseTile> tilesSafeFromZombiesAndEnemy = tilesSafeFromZombies.Where(tile =>
            {
                return worldState.DynamicWorld.EnemyBases.All(enemyTile =>
                {
                    return Math.Abs(enemyTile.X - tile.X) > 5 && Math.Abs(enemyTile.Y - tile.Y) > 5;
                });
            }).ToList();

            tilesSafeFromZombiesAndEnemy.Sort((a, b) =>
            {
                float distanceA = (a.X - centerx) * (a.X - centerx) + (a.Y - centery) * (a.Y - centery);
                float distanceB = (b.X - centerx) * (b.X - centerx) + (b.Y - centery) * (b.Y - centery);
                return (int)Math.Round(distanceB - distanceA);
            });

            if (tilesSafeFromZombiesAndEnemy.Count > 0)
            {
                var newHead = tilesSafeFromZombiesAndEnemy[0];
                int newx = newHead.X;
                int newy = newHead.Y;
                if (debugWrite)
                {
                    Console.WriteLine($"new head coords: [{newy}, {newx}]");
                    Console.WriteLine("\n");
                }

                worldState.TurnCommand.MoveCommand = new Coordinate() { X = newHead.X, Y = newHead.Y };
            } else
            {
                if (debugWrite)
                {
                    Console.WriteLine($"new head coords: none(");
                    Console.WriteLine("\n");
                }
            }


        }

        public Coordinate getMassCenter(WorldState worldState)
        {

            int totalmass = worldState.DynamicWorld.Base.Count;
            int totalx = 0;
            int totaly = 0;
            worldState.DynamicWorld.Base.ForEach(tile =>
            {
                totalx += tile.X;
                totaly += tile.Y;

            });
            float centerx = (float)totalx / (float)totalmass;
            float centery = (float)totaly / (float)totalmass;

            var massCenter = new Coordinate()
            {
                X = (int)Math.Round(centerx, 0),
                Y = (int)Math.Round(centery, 0)
            };

            return massCenter;
        }
    }
}
