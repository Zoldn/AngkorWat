using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Components;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;
using OperationsResearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.RouteSolver
{
    internal class TSPSolver
    {
        private readonly AllData allData;

        public Dictionary<Child, bool> AvailableChildren { get; set; }
        public Dictionary<Child, double> DistancesToSanta { get; set; }

        public TSPSolver(AllData allData)
        {
            this.allData = allData;

            AvailableChildren = new();
            DistancesToSanta = new();
        }

        public TSPSolution Solve()
        {
            var solution = new TSPSolution();

            InitializeChildren();

            //CalculateDistancesToSanta();
            CalculateDistancesToSantaEuclid();

            foreach (var packing in allData.PackingSolution.Packings)
            {
                Console.WriteLine($"TSP SOLVER: solving {packing}");

                var furthestChild = GetFurthestChild();

                var closestChilds = GetClosestChildsToSelected(furthestChild, packing.Gifts.Count - 1);

                var targetChilds = closestChilds
                    .Append(furthestChild)
                    .ToList();

                var curRoute = SolveSequence(targetChilds);

                foreach (var child in targetChilds)
                {
                    AvailableChildren[child] = false;
                }

                solution.Sequences.Add(packing, new LocationSequence(packing, curRoute));
            }

            OrderPackings(solution);

            ConcatFullRoute(solution);

            return solution;
        }

        private void OrderPackings(TSPSolution solution)
        {
            var packings = allData.PackingSolution
                .Packings
                //.Select(e => e.Gifts.Select(g => g.Id).ToList())
                .ToList();

            solution.OrderedPackings = packings;
        }

        private void ConcatFullRoute(TSPSolution solution)
        {
            var fullRoute = new List<IPunkt>();

            var totalTime = 0.0d;
            var totalLength = 0.0d;

            var rev = solution.OrderedPackings
                .ToList();

            rev.Reverse();

            var lastPacking = rev.Last();

            foreach (var packing in rev)
            {
                var subSequence = solution.Sequences[packing];

                for (int i = 0; i < subSequence.Locations.Count - 1; i++)
                {
                    var from = subSequence.Locations[i];
                    var to = subSequence.Locations[i + 1];

                    /// Если мы уже развезли все мешки, и это последний, то
                    /// возвращаться домой не надо
                    if (to == allData.Santa
                        && packing == lastPacking
                        )
                    {
                        continue;
                    }

                    var subRoute = allData.Routes.Routes[(from, to)];

                    totalTime += subRoute.TravelTime;
                    totalLength += subRoute.Distance;

                    fullRoute.AddRange(subRoute.Punkts.Skip(1));
                }
            }

            solution.FullRoute = fullRoute;
            solution.TravelTime = totalTime;
            solution.Distance = totalLength;
        }

        private List<ILocation> SolveSequence(List<Child> targetChilds)
        {
            var manager = new RoutingIndexManager(targetChilds.Count + 1, 1, 0);

            var routing = new RoutingModel(manager);

            Dictionary<int, ILocation> childrenIndex = targetChilds
                .Select((c, index) => (c, index))
                .ToDictionary(
                    p => p.index + 1,
                    p => p.c as ILocation
                );

            childrenIndex.Add(0, allData.Santa);

            long[, ] transitionMatrix = new long[childrenIndex.Count, childrenIndex.Count];

            for (int i = 0; i < childrenIndex.Count; i++)
            {
                for (int j = 0; j < childrenIndex.Count; j++)
                {
                    if (i == j)
                    {
                        transitionMatrix[i, j] = 0;
                    }
                    else
                    {
                        transitionMatrix[i, j] = (long)Math.Round(allData.Routes.Routes[(
                            childrenIndex[i], childrenIndex[j]
                            )].TravelTime);
                    }
                }
            }

            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
            {
                // Convert from routing variable Index to
                // distance matrix NodeIndex.
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return transitionMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

            RoutingSearchParameters searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();

            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration { Seconds = 5 };

            Assignment solution = routing.SolveWithParameters(searchParameters);

            var ret = ExtractSolution(routing, manager, solution);

            var route = ret
                .Select(r => childrenIndex[(int)r])
                .ToList();

            return route;
        }

        private List<long> ExtractSolution(RoutingModel routing, RoutingIndexManager manager, Assignment solution)
        {
            var ret = new List<long>();

            Console.WriteLine($"Objective: {solution.ObjectiveValue()}");
            // Inspect solution.
            Console.WriteLine("Route:");
            long routeDistance = 0;
            var index = routing.Start(0);

            while (!routing.IsEnd(index))
            {
                Console.Write($"{manager.IndexToNode((int)index)} -> ");
                var previousIndex = index;

                ret.Add(index);

                index = solution.Value(routing.NextVar(index));

                routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
            }

            ret.Add(0);

            Console.WriteLine($"{manager.IndexToNode((int)index)}");
            Console.WriteLine($"Route distance: {routeDistance} miles");

            return ret;
        }

        private void InitializeChildren()
        {
            AvailableChildren = allData
                .Children
                .ToDictionary(
                    c => c,
                    c => true
                );
        }

        private List<Child> GetClosestChildsToSelected(Child furthestChild, int count)
        {
            var distancesToSelected = AvailableChildren
                .Where(kv => kv.Value && kv.Key != furthestChild)
                .Select(e => e.Key)
                .ToDictionary(
                    c => c,
                    //c => allData.Routes.Routes[(c, furthestChild)].TravelTime
                    c => DistanceSolver.GetDistance(c, furthestChild)
                );

            return distancesToSelected
                .OrderByDescending(kv => kv.Value)
                .Take(count)
                .Select(kv => kv.Key)
                .ToList();
        }

        private void CalculateDistancesToSanta()
        {
            DistancesToSanta = allData.Children
                .ToDictionary(
                    c => c,
                    c => allData.Routes.Routes[(c, allData.Santa)].TravelTime
                );
        }

        private void CalculateDistancesToSantaEuclid()
        {
            DistancesToSanta = allData.Children
                .ToDictionary(
                    c => c,
                    c => DistanceSolver.GetDistance(c, allData.Santa) //allData.Routes.Routes[(c, allData.Santa)].TravelTime
                );
        }

        private Child GetFurthestChild()
        {
            return DistancesToSanta
                .Where(kv => AvailableChildren[kv.Key])
                .OrderBy(kv => kv.Value)
                .Select(kv => kv.Key)
                .First();
        }
    }
}
