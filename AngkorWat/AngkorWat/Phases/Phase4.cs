using AngkorWat.Algorithms;
using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal static class Phase4
    {
        private readonly static int WaitTime = 300;

        internal async static Task Run()
        {
            var universeState = await HttpHelper.Get<UniverseState>(
                "https://datsedenspace.datsteam.dev/player/universe") ?? throw new Exception();

            var data = new Data();

            Phase1.InitializePlanets(data, universeState);
            Phase1.InitializeRoutes(data, universeState);
            Phase1.InitializeShip(data, universeState);

            Thread.Sleep(WaitTime);

            INextPlanetSelector nextPlanetSelector = new ClosestToEdenPlanetSelector();
            INextPlanetSelector closestPlanetSelector = new ClosestPlanetSelector ();

            int iteration = 0;

            int threshold = (int)Math.Ceiling(0.5d * data.Ship.CapacityX * data.Ship.CapacityY);

            while (true) 
            {
                iteration++;

                universeState = await HttpHelper.Get<UniverseState>(
                    "https://datsedenspace.datsteam.dev/player/universe") ?? throw new Exception();

                Thread.Sleep(WaitTime);

                Phase1.InitializePlanets(data, universeState);
                Phase1.InitializeRoutes(data, universeState);
                Phase1.InitializeShip(data, universeState);

                Planet? nextPlanet = null;
                List<string> routeSteps = new List<string>();

                if (data.Ship.Planet.Name == "Earth"
                    || data.Ship.Planet.Name == "Eden"
                    || data.Ship.Garbage.Sum(kv => kv.Value.Count) < threshold)
                {
                    if (!closestPlanetSelector.TryGetNextPlanet(data, out nextPlanet))
                    {
                        Console.WriteLine($"Iteration={iteration}: Nowhere to fly");
                        threshold = 0;
                        data.BannedPlanets.Clear();
                        continue;
                    }

                    routeSteps = Dijkstra.Run(universeState, data.Ship.Planet.Name, nextPlanet.Name);
                }
                else
                {
                    if (!nextPlanetSelector.TryGetNextPlanet(data, out nextPlanet))
                    {
                        Console.WriteLine($"Iteration={iteration}: Nowhere to fly");
                        threshold = 0;
                        data.BannedPlanets.Clear();
                        continue;
                    }

                    var toEden = Dijkstra.Run(universeState, data.Ship.Planet.Name, "Eden");
                    var fromEden = Dijkstra.Run(universeState, "Eden", nextPlanet.Name);

                    routeSteps.Clear();
                    routeSteps.AddRange(toEden);
                    routeSteps.AddRange(fromEden);
                }

                var travel = new Travel();
                travel.Route.Clear();
                travel.Route.AddRange(routeSteps);

                var travelResponse = await HttpHelper.Post<Travel, TravelResponse>(
                    "https://datsedenspace.datsteam.dev/player/travel", travel) ??
                    throw new Exception();

                Thread.Sleep(WaitTime);

                data.Ship.Planet = nextPlanet;

                /// foreach travel route increase by 10 

                Console.WriteLine($"Iteration={iteration}: Traveled to planet {nextPlanet.Name}");

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

                if (planetGarbage.Count == 0)
                {
                    data.BannedPlanets.Add(nextPlanet.Name);
                }

                var totalGarbage = data.Ship.Garbage
                    .Concat(planetGarbage)
                    .Select(e => new GarbageItem(e.Key, e.Value.Select(e => (e[0], e[1])).ToList()))
                    .ToList();

                int minLimit = Phase1.GetMinLimit(data);

                var solver = new PackingSolver();
                var solution = solver.Solve(data.Ship.CapacityX, data.Ship.CapacityY,
                    totalGarbage, minLimit);

                if (solution.GarbageItems.All(g => g.IsTaken))
                {
                    data.BannedPlanets.Add(data.Ship.Planet.Name);
                }

                if (!solution.IsOk)
                {
                    Console.WriteLine($"[!] Solution is not ok");
                }
                else
                {
                    Console.WriteLine($"Solution is ok");
                }

                if (planetGarbage.Count > 0
                    && solution.IsOk)
                {
                    var collectData = Phase1.MakeCollectData(solution);

                    var collectRet = await HttpHelper.Post<CollectGarbage, CollectGarbageResponse>(
                        "https://datsedenspace.datsteam.dev/player/collect", collectData);

                    Console.WriteLine($"Collect is OK");

                    //data.Ship.Garbage.Clear();
                    //data.Ship.Garbage.Add

                    Thread.Sleep(WaitTime);
                }
                else
                {
                    data.BannedPlanets.Add(data.Ship.Planet.Name);
                }
            }
        }
    }
}
