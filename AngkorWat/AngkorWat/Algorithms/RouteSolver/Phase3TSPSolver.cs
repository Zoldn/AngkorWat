//using AngkorWat.Algorithms.DistSolver;
//using AngkorWat.Algorithms.PackSolver;
//using AngkorWat.Algorithms.Phase2DDOS;
//using AngkorWat.Components;
//using Google.OrTools.ConstraintSolver;
//using Google.Protobuf.WellKnownTypes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AngkorWat.Algorithms.RouteSolver
//{
//    internal class Phase3TSPSolver
//    {
//        private Data data;
//        private Phase1Solution phase1Solution;
//        public Dictionary<Phase1Child, bool> AvailableChildren { get; set; }
//        public Dictionary<Phase1Child, double> DistancesToSanta { get; set; }

//        public Phase3TSPSolver(Data data, Phase1Solution phase1Solution, 
//            ChildToGiftSolution childToGiftSolution)
//        {
//            this.data = data;
//            this.phase1Solution = phase1Solution;
//            this.childToGiftSolution = childToGiftSolution;

//            AvailableChildren = new();
//            DistancesToSanta = new();
//        }

//        internal TSPSolution Solve()
//        {
//            var solution = new TSPSolution();

//            InitializeChildren();

//            CalculateDistancesToSantaEuclid();

//            while (AvailableChildren.Any(kv => kv.Value))
//            {
//                var furthestChild = GetFurthestChild();

//                var closestChilds = GetClosestChildsToSelected(furthestChild);

//                var targetChilds = closestChilds
//                    .Append(furthestChild)
//                    .ToList();

//                var curRoute = SolveSequence(targetChilds);

//                foreach (var child in targetChilds)
//                {
//                    AvailableChildren[child] = false;
//                }

//                solution.Sequences2.Add(new LocationSequence(null, curRoute));
//            }

//            //OrderPackings(solution);

//            ConcatFullRoute(solution);

//            return solution;
//        }

//        //private void OrderPackings(TSPSolution solution)
//        //{
//        //    solution.Sequences2

//        //    var packings = phase1Solution.PackingSolution
//        //        .Packings
//        //        //.Select(e => e.Gifts.Select(g => g.Id).ToList())
//        //        .ToList();

//        //    solution.OrderedPackings = packings;
//        //}

//        private void ConcatFullRoute(TSPSolution solution)
//        {
//            var fullRoute = new List<IPunkt>();

//            var totalTime = 0.0d;
//            var totalLength = 0.0d;

//            var rev = solution.Sequences2
//                .ToList();

//            rev.Reverse();

//            var lastPacking = rev.Last();

//            var childToGifts = childToGiftSolution
//                .ChildToGifts
//                .ToDictionary(kv => kv.Child.Id, kv => kv.Gift.Id);

//            var giftIds = data.Gifts
//                .ToDictionary(g => g.Id);

//            foreach (var rout in rev)
//            {
//                //var subSequence = solution.Sequences[packing];

//                var tt = rout.Locations
//                    .Where(e => e.PunktType == PunktType.CHILD)
//                    .Select(e => giftIds[childToGifts[e.Id]])
//                    .ToList();

//                if (tt.Sum(e => e.Volume) > 100 || tt.Sum(e => e.Weight) > 200)
//                {
//                    int y = 1;
//                }

//                tt.Reverse();

//                var packing = new Packing(tt);

//                solution.OrderedPackings.Add(packing);

//                for (int i = 0; i < rout.Locations.Count - 1; i++)
//                {
//                    var from = rout.Locations[i];
//                    var to = rout.Locations[i + 1];

//                    /// Если мы уже развезли все мешки, и это последний, то
//                    /// возвращаться домой не надо
//                    if (to == data.Santa
//                        && rout == lastPacking
//                        )
//                    {
//                        continue;
//                    }

//                    var subRoute = phase1Solution.Routes.Routes[(from, to)];

//                    totalTime += subRoute.TravelTime;
//                    totalLength += subRoute.Distance;

//                    fullRoute.AddRange(subRoute.Punkts.Skip(1));
//                }
//            }

//            solution.FullRoute = fullRoute;
//            solution.TravelTime = totalTime;
//            solution.Distance = totalLength;

//            //Console.WriteLine($"FINAL RESULT: Travel time = {solution.TravelTime}, distance = {solution.Distance}");
//        }

//        private List<ILocation> SolveSequence(List<Phase1Child> targetChilds)
//        {
//            var manager = new RoutingIndexManager(targetChilds.Count + 1, 1, 0);

//            var routing = new RoutingModel(manager);

//            Dictionary<int, ILocation> childrenIndex = targetChilds
//                .Select((c, index) => (c, index))
//                .ToDictionary(
//                    p => p.index + 1,
//                    p => p.c as ILocation
//                );

//            childrenIndex.Add(0, data.Santa);

//            long[,] transitionMatrix = new long[childrenIndex.Count, childrenIndex.Count];

//            for (int i = 0; i < childrenIndex.Count; i++)
//            {
//                for (int j = 0; j < childrenIndex.Count; j++)
//                {
//                    if (i == j)
//                    {
//                        transitionMatrix[i, j] = 0;
//                    }
//                    else
//                    {
//                        transitionMatrix[i, j] = (long)Math.Round(phase1Solution.Routes.Routes[(
//                            childrenIndex[i], childrenIndex[j]
//                            )].TravelTime);
//                    }
//                }
//            }

//            int transitCallbackIndex = routing.RegisterTransitCallback((long fromIndex, long toIndex) =>
//            {
//                // Convert from routing variable Index to
//                // distance matrix NodeIndex.
//                var fromNode = manager.IndexToNode(fromIndex);
//                var toNode = manager.IndexToNode(toIndex);
//                return transitionMatrix[fromNode, toNode];
//            });

//            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);

//            RoutingSearchParameters searchParameters =
//                operations_research_constraint_solver.DefaultRoutingSearchParameters();

//            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
//            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
//            searchParameters.TimeLimit = new Duration { Seconds = 2 };

//            Assignment solution = routing.SolveWithParameters(searchParameters);

//            var ret = ExtractSolution(routing, manager, solution);

//            var route = ret
//                .Select(r => childrenIndex[(int)r])
//                .ToList();

//            return route;
//        }

//        private List<long> ExtractSolution(RoutingModel routing, RoutingIndexManager manager, Assignment solution)
//        {
//            var ret = new List<long>();

//            Console.WriteLine($"Objective: {solution.ObjectiveValue()}");
//            // Inspect solution.
//            Console.WriteLine("Route:");
//            long routeDistance = 0;
//            var index = routing.Start(0);

//            while (!routing.IsEnd(index))
//            {
//                Console.Write($"{manager.IndexToNode((int)index)} -> ");
//                var previousIndex = index;

//                ret.Add(index);

//                index = solution.Value(routing.NextVar(index));

//                routeDistance += routing.GetArcCostForVehicle(previousIndex, index, 0);
//            }

//            ret.Add(0);

//            Console.WriteLine($"{manager.IndexToNode((int)index)}");
//            Console.WriteLine($"Route distance: {routeDistance} miles");

//            return ret;
//        }

//        private List<Phase1Child> GetClosestChildsToSelected(Phase1Child furthestChild)
//        {
//            Func<Phase1Child, double> distanceCalculator = c => GeometryUtils.GetDistance(c, furthestChild);

//            List<Phase1Child> ret = new();

//            var distancesToSelected = AvailableChildren
//                .Where(kv => kv.Value && kv.Key != furthestChild)
//                .Select(e => e.Key)
//                .ToDictionary(
//                    c => c,
//                    //c => allData.Routes.Routes[(c, furthestChild)].TravelTime
//                    //c => GeometryUtils.GetDistance(c, furthestChild)
//                    c => distanceCalculator(c)
//                );

//            var orderedChildren = distancesToSelected
//                .OrderBy(kv => kv.Value)
//                .Select(e => e.Key)
//                .ToList();

//            var childToGifts = childToGiftSolution
//                .ChildToGifts
//                .ToDictionary(kv => kv.Child.Id, kv => kv.Gift.Id);

//            var weights = data.Gifts.ToDictionary(g => g.Id, g => g.Weight);
//            var volumes = data.Gifts.ToDictionary(g => g.Id, g => g.Volume);

//            var giftId = childToGifts[furthestChild.Id];

//            int totalWeight = weights[giftId];
//            int totalVolume = volumes[giftId];

//            for (int i = 0; i < orderedChildren.Count; i++)
//            {
//                giftId = childToGifts[orderedChildren[i].Id];

//                int currentWeight = weights[giftId];
//                int currentVolume = volumes[giftId];

//                if (currentVolume + totalVolume > 100
//                    || currentWeight + totalWeight > 200
//                    )
//                {
//                    continue;
//                }
//                else
//                {
//                    totalVolume += currentVolume;
//                    totalWeight += currentWeight;

//                    ret.Add(orderedChildren[i]);
//                }
//            }

//            if (totalVolume > 100 || totalWeight > 200)
//            {
//                int y = 1;
//            }

//            return ret;
//        }

//        private Phase1Child GetFurthestChild()
//        {
//            return DistancesToSanta
//                .Where(kv => AvailableChildren[kv.Key])
//                .OrderByDescending(kv => kv.Value)
//                .Select(kv => kv.Key)
//                .First();
//        }

//        private void InitializeChildren()
//        {
//            AvailableChildren = data
//                .Children
//                .ToDictionary(
//                    c => c,
//                    c => true
//                );
//        }

//        private void CalculateDistancesToSantaEuclid()
//        {
//            DistancesToSanta = data.Children
//                .ToDictionary(
//                    c => c,
//                    c => GeometryUtils.GetDistance(c, data.Santa) //allData.Routes.Routes[(c, allData.Santa)].TravelTime
//                );
//        }
//    }
//}
