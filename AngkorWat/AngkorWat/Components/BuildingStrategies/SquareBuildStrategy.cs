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
        public void AddCommand(WorldState worldState)
        {
            if (worldState.DynamicWorld.Base.Count == 0) { return; }

            BaseTile head = worldState.DynamicWorld.Base.Find(tile => tile.IsHead);
            if (head == null) { return; }

            Coordinate center = new Coordinate() { X = head.X, Y = head.Y };

            List<Coordinate> newOrders = squareCommands(worldState, center);

            worldState.TurnCommand.BuildCommands.AddRange(newOrders);
        }
        public List<Coordinate> squareCommands(WorldState worldState, Coordinate center)
        {
            List<Coordinate> toBuild = new List<Coordinate>();
            bool filledToMax = false;
            int remainingGold = worldState.DynamicWorld.Player.Gold;
            int radius = 1;

            bool debugWrite = false;

            while (!filledToMax && remainingGold > 0)
            {
                // work with square with side = radius * 2 + 1 around coords
                List<Coordinate> squareCoordinates = new List<Coordinate>();
                // adding all coords from top-left going clockwise
                for (int x = center.X - radius; x <= center.X + radius; x++)
                {
                    squareCoordinates.Add(new Coordinate() { X = x, Y = center.Y + radius });
                }
                for (int y = center.Y - radius + 1; y <= center.Y + radius; y++)
                {
                    squareCoordinates.Add(new Coordinate() { X = center.X + radius, Y = y });
                }
                for (int x = center.X + radius - 1; x >= center.X - radius; x--)
                {
                    squareCoordinates.Add(new Coordinate() { X = x, Y = center.Y - radius });
                }
                for (int y = center.Y + radius - 1; y > center.Y - radius; y--)
                {
                    squareCoordinates.Add(new Coordinate() { X = center.X + radius, Y = y });
                }

                if (debugWrite)
                {
                    Console.WriteLine("Square coords:");
                    squareCoordinates.ForEach(c => Console.Write($"[{c.Y}, {c.X}]  "));
                    Console.WriteLine("\n");
                }

                // check if our base is already here
                List<Coordinate> isItBaseCoords = new List<Coordinate>();
                if (!squareCoordinates.TrueForAll((c) =>
                {
                    return whatIsHere(worldState, c) == "base";
                }))
                {
                    // check if we can add anything to a square
                    List<Coordinate> buildableCoords = new List<Coordinate>();
                    squareCoordinates.ForEach((Coordinate c) =>
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

                    if (buildableCoords.Count == 0)
                    {
                        filledToMax = true;
                        break;
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
