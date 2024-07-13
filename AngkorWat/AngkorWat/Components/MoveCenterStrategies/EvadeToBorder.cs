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
        public bool goToSafe = false;
        public int timeToSafe = 80;
        public int safeDistanceToSpots = 4;
        public int safeDistanceToEnemySquared = 36;
        // end constants
        public void AddCommand(WorldState worldState)
        {
            if (worldState.DynamicWorld.Base.Count == 0) { return; }
            BaseTile head = worldState.DynamicWorld.Base.Find(tile => tile.IsHead);
            if (head == null) { return; }

            if (worldState.DynamicWorld.Turn > timeToSafe)
            {
                goToSafe = true;
            }

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

                if (worldState.StaticWorld.ZPots.Count > 0)
                {
                    var closeZpots = worldState.StaticWorld.ZPots.Where(spot =>
                    {
                        return ((Math.Abs(spot.X - tile.X) < safeDistanceToSpots) || (Math.Abs(spot.Y - tile.Y) < safeDistanceToSpots));
                    }).ToList();
                    var closeWalls = worldState.StaticWorld.ZPots.Where(spot =>
                    {
                        return ((Math.Abs(spot.X - tile.X) < safeDistanceToSpots - 1) || (Math.Abs(spot.Y - tile.Y) < safeDistanceToSpots - 1));
                    }).ToList();

                    if (closeWalls.All(spot =>
                    {
                        return (spot.Type != "wall");
                    }))
                    {
                        if (closeZpots.Any(spot =>
                        {
                            return (spot.Type == "default");
                        }))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }).ToList();

            if (debugWrite)
            {
                Console.WriteLine("SafeFromZs:");
                tilesSafeFromZombies.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                Console.WriteLine("\n");
            }

            List<BaseTile> tilesSafeFromZombiesAndEnemy = tilesSafeFromZombies.Where(tile =>
            {
                return worldState.DynamicWorld.EnemyBases.All(enemyTile =>
                {
                    return (enemyTile.X - tile.X) * (enemyTile.X - tile.X) + (enemyTile.Y - tile.Y) * (enemyTile.Y - tile.Y) > safeDistanceToEnemySquared;
                });
            }).ToList();

            if (debugWrite)
            {
                Console.WriteLine("tilesSafeFromZombiesAndEnemy:");
                tilesSafeFromZombiesAndEnemy.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                Console.WriteLine("\n");
            }

            if (!goToSafe) { 
            }
            tilesSafeFromZombiesAndEnemy.Sort((a, b) =>
            {
                float distanceA = (a.X - centerx) * (a.X - centerx) + (a.Y - centery) * (a.Y - centery);
                float distanceB = (b.X - centerx) * (b.X - centerx) + (b.Y - centery) * (b.Y - centery);
                return (int)Math.Round(distanceB - distanceA);
            });

            if (debugWrite)
            {
                Console.WriteLine("Sorted coords:");
                tilesSafeFromZombiesAndEnemy.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                Console.WriteLine("\n");
            }

            if (tilesSafeFromZombiesAndEnemy.Count > 0)
            {
                var newHead = tilesSafeFromZombiesAndEnemy[0];
                if (goToSafe)
                {
                    newHead = tilesSafeFromZombiesAndEnemy[tilesSafeFromZombiesAndEnemy.Count - 1];
                }

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
