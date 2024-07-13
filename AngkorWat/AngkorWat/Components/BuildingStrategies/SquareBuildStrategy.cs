﻿using AngkorWat.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.BuildingStrategies
{
    internal class SquareBuildStrategy : IBuildStrategy
    {
        public SquareBuildStrategy() { }
        // constants
        public bool todayIsSafe = true;
        public bool debugWrite = false;
        public int minDistanceToSpawns = 6;
        public int minDistanceToEnemys = 5;
        public int setMinDistanceToOneAfterTurn = 130;
        // end constants
        public void AddCommand(WorldState worldState)
        {
            if (worldState.DynamicWorld.Base.Count == 0) { return; }


            BaseTile head = worldState.DynamicWorld.Base.Find(tile => tile.IsHead);
            if (head == null) { return; }


            Coordinate center = getMassCenter(worldState);
            if (worldState.DynamicWorld.Player.Gold > 30 && worldState.DynamicWorld.Turn > setMinDistanceToOneAfterTurn && minDistanceToSpawns > 2)
            {
                minDistanceToSpawns = 1;
            }


            List<Coordinate> newOrders = squareCommands(worldState, center);
            while ((minDistanceToSpawns > 1) && (worldState.DynamicWorld.Player.Gold > 16) && (newOrders.Count < 4))
            {
                minDistanceToSpawns--;
                newOrders = squareCommands(worldState, center);
            }


            worldState.TurnCommand.BuildCommands.AddRange(newOrders);
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
        public List<Coordinate> squareCommands(WorldState worldState, Coordinate center)
        {

            List<Coordinate> toBuild = new List<Coordinate>();
            bool filledToMax = false;
            int remainingGold = worldState.DynamicWorld.Player.Gold;
            int radius = 1;
            int lastBaseRadius = 0;

            while (!filledToMax && remainingGold > 0)
            {
                // work with square with side = radius * 2 + 1 around coords
                List<Coordinate> squareCoordinates = new List<Coordinate>();
                // adding all coords from top-left going clockwise
                for (int x = center.X - radius; x < center.X + radius; x++)
                {
                    squareCoordinates.Add(new Coordinate() { X = x, Y = center.Y - radius });
                }
                for (int y = center.Y - radius; y < center.Y + radius; y++)
                {
                    squareCoordinates.Add(new Coordinate() { X = center.X + radius, Y = y });
                }
                for (int x = center.X + radius; x > center.X - radius; x--)
                {
                    squareCoordinates.Add(new Coordinate() { X = x, Y = center.Y + radius });
                }
                for (int y = center.Y + radius; y > center.Y - radius; y--)
                {
                    squareCoordinates.Add(new Coordinate() { X = center.X - radius, Y = y });
                }

                if (debugWrite)
                {
                    Console.WriteLine("Square coords:");
                    squareCoordinates.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                    Console.WriteLine("\n");
                }


                // if it is safe this turn - on every 2nd radius we will skip half of the nodes
                if (todayIsSafe)
                {
                    squareCoordinates = squareCoordinates.Where(c =>
                    {
                        if (worldState.DynamicWorld.EnemyBases.Count > 0)
                        {
                            var minDistToEnemy = worldState.DynamicWorld.EnemyBases.Min(enemyTile =>
                            {
                                return (enemyTile.X - c.X) * (enemyTile.X - c.X) + (enemyTile.Y - c.Y) * (enemyTile.Y - c.Y);
                            });

                            if (minDistToEnemy <= minDistanceToEnemys * minDistanceToEnemys) {
                                return true;
                            };
                        }

                        if (c.X % 4 == 0 && c.Y % 4 == 1)
                        {
                            return false;
                        }
                        if (c.X % 4 == 1 && c.Y % 4 == 3)
                        {
                            return false;
                        }
                        if (c.X % 4 == 2 && c.Y % 4 == 0)
                        {
                            return false;
                        }
                        if (c.X % 4 == 3 && c.Y % 4 == 2)
                        {
                            return false;
                        }
                        return true;
                    }).ToList();

                    if (debugWrite)
                    {
                        Console.WriteLine("Square coords hexed:");
                        squareCoordinates.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                        Console.WriteLine("\n");
                    }
                }

                // remove coords which are too close to spawns
                List<Coordinate> antiSpawnCoords = squareCoordinates;
                if ((minDistanceToSpawns > 1) && (radius > 2))
                {
                    for (int i = minDistanceToSpawns; i > 1; --i)
                    {
                        antiSpawnCoords = squareCoordinates.Where(c => coordIsFarFromSpawn(worldState, c, i)).ToList();
                        if (antiSpawnCoords.Count > 4)
                        {
                            break;
                        }
                    }
                }

                // if almost all coords are forbidden - ignore this rule
                if (antiSpawnCoords.Count < 4)
                {
                    antiSpawnCoords = squareCoordinates;
                }

                // check if our base is already in some slots for this radius
                List<Coordinate> isItBaseCoords = new List<Coordinate>();
                if (!antiSpawnCoords.TrueForAll((c) =>
                {
                    return whatIsHere(worldState, c) != "base";
                }))
                {
                    lastBaseRadius = radius;
                }

                // if there were no base tile on this and previous radius - break
                if (radius > lastBaseRadius + 1)
                {
                    break;
                }

                if (!antiSpawnCoords.TrueForAll((c) =>
                {
                    return whatIsHere(worldState, c) == "base" || !canBuildHere(worldState, c);
                }))
                {
                    // check if we can add anything to a square
                    List<Coordinate> buildableCoords = new List<Coordinate>();
                    antiSpawnCoords.ForEach((Coordinate c) =>
                    {
                        if (whatIsHere(worldState, c) == "nothing" && canBuildHere(worldState, c))
                        {
                            buildableCoords.Add(c);
                        }
                    });

                    if (debugWrite)
                    {
                        Console.WriteLine("Buildable square coords:");
                        buildableCoords.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                        Console.WriteLine("\n");
                    }

                    if ((buildableCoords.Count > remainingGold) && worldState.DynamicWorld.EnemyBases.Count > 0)
                    {
                        buildableCoords = buildableCoords.OrderBy(tile =>
                        {
                            return worldState.DynamicWorld.EnemyBases.Max(enemyTile =>
                            {
                                return (enemyTile.X - tile.X) * (enemyTile.X - tile.X) + (enemyTile.Y - tile.Y) * (enemyTile.Y - tile.Y);
                            });
                        }).ToList();
                    }


                    // filling commands from buildableCoords limiting by gold
                    int toImport = Math.Min(remainingGold, buildableCoords.Count);
                    toBuild.AddRange(buildableCoords.GetRange(0, toImport));

                    // removing spend gold
                    remainingGold = remainingGold - toImport;
                }


                // make i + 1
                ++radius;
            }

            if (debugWrite)
            {
                Console.WriteLine("tobuild square coords:");
                toBuild.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                Console.WriteLine("\n");
            }

            return toBuild;
        }
        public bool coordIsFarFromSpawn(WorldState worldState, Coordinate coords, int minDistance)
        {
            List<Coordinate> coordsToCheck = new List<Coordinate>();
            for (int i = -minDistance; i <= minDistance; ++i)
            {
                coordsToCheck.Add(new Coordinate() { X = coords.X + i, Y = coords.Y });
                coordsToCheck.Add(new Coordinate() { X = coords.X, Y = coords.Y + i });
            }

            return coordsToCheck.TrueForAll(c =>
            {
                string here = whatIsHere(worldState, c);
                return here != "default";
            }) || !coordsToCheck.TrueForAll(c =>
            {
                string here = whatIsHere(worldState, c);
                return here != "wall";
            });
        }
        public string whatIsHere(WorldState worldState, Coordinate coords)
        {
            if (worldState == null)
            {
                return "nothing";
            }

            // checking enemy zpots
            if (worldState.StaticWorld.ZPots != null)
            {
                ZombieSpawn foundZpot = worldState.StaticWorld.ZPots.Find(x => x.X == coords.X && x.Y == coords.Y);
                if (foundZpot != null)
                {
                    return foundZpot.Type;
                }
            }

            // checking enemy bases
            if (worldState.DynamicWorld.EnemyBases != null)
            {
                EnemyBaseTile foundEnemyBase = worldState.DynamicWorld.EnemyBases.Find(x => x.X == coords.X && x.Y == coords.Y);
                if (foundEnemyBase != null)
                {
                    return "enemy";
                }
            }

            // checking own base
            if (worldState.DynamicWorld.Base != null)
            {
                BaseTile foundOwnBase = worldState.DynamicWorld.Base.Find(x => x.X == coords.X && x.Y == coords.Y);
                if (foundOwnBase != null)
                {
                    return "base";
                }
            }

            return "nothing";
        }
        public bool canBuildHere(WorldState worldState, Coordinate coords)
        {
            bool isFree = this.whatIsHere(worldState, coords) == "nothing";
            List<Coordinate> neighbourTiles = new List<Coordinate>
            {
                new Coordinate() { X = coords.X + 1, Y = coords.Y },
                new Coordinate() { X = coords.X - 1, Y = coords.Y },
                new Coordinate() { X = coords.X, Y = coords.Y + 1 },
                new Coordinate() { X = coords.X, Y = coords.Y - 1 }
            };

            // check if at least one neighbourTiles is a base
            bool isCloseToBase = !neighbourTiles.TrueForAll((coords) =>
            {
                string typeOfLand = whatIsHere(worldState, coords);
                return typeOfLand != "base";
            });

            // check if all neighbour tiles are either base or nothing
            bool isBuildable = neighbourTiles.TrueForAll((coords) =>
            {
                string typeOfLand = whatIsHere(worldState, coords);
                return typeOfLand == "base" || typeOfLand == "nothing";
            });

            return isFree && isCloseToBase && isBuildable;
        }
    }
}
