using AngkorWat.Algorithms;
using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AngkorWat.Phases
{
    internal static class Phase1
    {
        public async static Task Phase1Start()
        {
            var universeState = await HttpHelper.Get<UniverseState>(
                "https://datsedenspace.datsteam.dev/player/universe") ?? throw new Exception();

            var data = new Data();

            InitializePlanets(data, universeState);
            InitializeRoutes(data, universeState);
            InitializeShip(data, universeState);

            HashSet<Planet> bannedPlanets = new();
            HashSet<Planet> alwaysBannedPlanets = new() 
            {
                data.Planets["Earth"],
                data.Planets["Eden"],
            };

            var solver = new PackingSolver();

            while (true)
            {
                if (!TryGetClosestPlanets(data, bannedPlanets, alwaysBannedPlanets, out var nextPlanet))
                {
                    break;
                }

                var travel = new Travel();
                travel.Route.Clear();
                travel.Route.Add(nextPlanet.Name);

                var travelResponse = await HttpHelper.Post<Travel, TravelResponse>(
                    "https://datsedenspace.datsteam.dev/player/travel", travel) ?? 
                    throw new Exception();

                Thread.Sleep(250);

                if (travelResponse.ShipGarbage is not null)
                {
                    data.Ship.Garbage = travelResponse.ShipGarbage
                        .ToDictionary(
                            f => f.Key,
                            f => f.Value.Select(e => new List<int>() { e[0], e[1] }).ToList()
                        );
                }
                else
                {
                    data.Ship.Garbage.Clear();
                }

                Dictionary<string, List<List<int>>> planetGarbage;

                if (travelResponse.PlanetGarbage is not null)
                {
                    planetGarbage = travelResponse.PlanetGarbage
                        .ToDictionary(
                            f => f.Key,
                            f => f.Value.Select(e => new List<int>() { e[0], e[1] }).ToList()
                        );
                }
                else
                {
                    planetGarbage = new();
                }
                
                var totalGarbage = data.Ship.Garbage
                    .Concat(planetGarbage)
                    .Select(e => new GarbageItem(e.Key, e.Value.Select(e => (e[0], e[1])).ToList()))
                    .ToList();

                int minLimit = GetMinLimit(data);

                var solution = solver.Solve(data.Ship.CapacityX, data.Ship.CapacityY, 
                    totalGarbage, minLimit);

                if (solution.GarbageItems.Any())
                {
                    var collectData = MakeCollectData(solution);

                    var collectRet = await HttpHelper.Post<CollectGarbage, CollectGarbageResponse>(
                        "https://datsedenspace.datsteam.dev/player/collect", collectData);

                    Thread.Sleep(250);

                    ////////////////////////
                }
                else
                {
                    bannedPlanets.Add(nextPlanet);
                }

                if (solution.GarbageItems.All(g => g.IsTaken))
                {
                    bannedPlanets.Add(nextPlanet);
                }

                data.Ship.Planet = nextPlanet;
            }
        }

        public static CollectGarbage MakeCollectData(PackingSolution solution)
        {
            var ret = new CollectGarbage();

            ret.ShipGarbage = solution
                .GarbageItems
                .Where(e => e.IsTaken)
                .ToDictionary(
                    e => e.Name,
                    e => e.Form.Select(f => new List<int>() { f.X + e.X, f.Y + e.Y }).ToList()
                );

            //if (ret.ShipGarbage.Count == 0)
            //{
            //    ret.ShipGarbage = null;
            //}

            return ret;
        }

        public static int GetMinLimit(Data data)
        {
            var currentScrap = data.Ship.Garbage.Sum(kv => kv.Value.Count);
            var maxSize = data.Ship.CapacityX * data.Ship.CapacityY;
            var delta = (int)Math.Ceiling(0.05d * maxSize);

            if (currentScrap == 0)
            {
                return (int)Math.Ceiling(0.3d * maxSize);
            }
            else
            {
                return currentScrap + delta;
            }
        }

        public static void InitializeShip(Data data, UniverseState universeState)
        {
            data.Ship = new Ship();

            var currentPlanet = data.Planets[universeState.Ship.Planet.Name];

            data.Ship.Planet = currentPlanet;
            data.Ship.CapacityX = universeState.Ship.CapacityX;
            data.Ship.CapacityY = universeState.Ship.CapacityY;

            data.Ship.Garbage = universeState.Ship.Garbage;
        }

        public static void InitializePlanets(Data data, UniverseState universeState)
        {
            data.Planets = universeState
                .Routes
                .SelectMany(r => r.Take(2))
                .Distinct()
                .Select(e => new Planet()
                {
                    Name = e,
                })
                .ToDictionary(
                    e => e.Name,
                    e => e
                    );
        }

        public static void InitializeRoutes(Data data, UniverseState universeState)
        {
            data.Routes = universeState
                .Routes
                .Select(r => new Route(data.Planets[r[0]], data.Planets[r[1]], int.Parse(r[2])))
                .ToList();

            data.RoutesFrom = data.Routes
                .ToLookup(r => r.LocationFrom);

            data.RoutesTo = data.Routes
                .ToLookup(r => r.LocationTo);
        }

        public static bool TryGetClosestPlanets(Data data, HashSet<Planet> bannedPlanets,
            HashSet<Planet> alwaysBannedPlanets,
            [MaybeNullWhen(false)][NotNullWhen(true)] out Planet? nextPlanet)
        {
            Planet currentPlanet = data.Ship.Planet;

            var routesFromHere = data.RoutesFrom[currentPlanet]
                .Where(r => !bannedPlanets.Contains(r.LocationTo)
                    && !alwaysBannedPlanets.Contains(r.LocationTo)
                )
                .OrderBy(r => r.Cost)
                .ToList();

            nextPlanet = routesFromHere.FirstOrDefault()?.LocationTo;

            return nextPlanet is not null;
        }
    }
}
