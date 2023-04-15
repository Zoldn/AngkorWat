using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.Phase2MIP;
using AngkorWat.Algorithms.Phase2MIP.HappinessFunctions;
using AngkorWat.Algorithms.Phase3DensePacker;
using AngkorWat.Algorithms.RouteSolver;
using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase3FullSolver
{
    internal class Phase3Solver
    {
        private Data data;
        private DistanceSolution routes;
        public int PackingCount { get; init; }
        public Metric SelectClosestChildStrategy { get; init; }
        public Dictionary<Child, bool> AvailableChildren { get; set; }
        public Dictionary<Gift, bool> AvailableGifts { get; set; }
        public Dictionary<Child, double> DistancesToSanta { get; set; }
        public Phase3Solver(Data data, DistanceSolution routes)
        {
            this.data = data; 
            this.routes = routes;

            PackingCount = 22;
            SelectClosestChildStrategy = Metric.EUCLID;

            AvailableChildren = new();
            DistancesToSanta = new();
            AvailableGifts = new();
        }

        public void Solve()
        {
            AvailableChildren = data.Children
                .ToDictionary(
                    c => c,
                    c => true
                );

            AvailableGifts = data.Gifts
                .ToDictionary(
                    c => c,
                    c => true
                );

            CalculateDistancesToSantaEuclid();

            List<int> packingSizes = GetPackingSizes();

            var childPackGroups = InitializeChildPackGroups(packingSizes);

            var happinessFunction = new Phase3TrueHappinessFunction(data);

            var mipSolver = new MIPSolver(data, happinessFunction) 
            {
                WeightLimit = data.WeightLimit,
                VolumeLimit = data.VolumeLimit,
            };

            var childToPGroups = childPackGroups
                .SelectMany(cg => cg.ChildToGifts.Keys, (cg, c) => (cg, c))
                .ToDictionary(p => p.c, p => p.cg);

            var childToGifts = mipSolver.Solve(data.Children, data.Gifts, childToPGroups);
        }

        private List<ChildPackingGroup> InitializeChildPackGroups(List<int> packingSizes)
        {
            List<ChildPackingGroup> childGroups = new();

            foreach (var packingSize in packingSizes)
            {
                var furthestChild = GetFurthestChild();

                var closestChilds = GetClosestChildsToSelected(furthestChild, packingSize - 1);

                var childPackGroup = new ChildPackingGroup(closestChilds
                    .Append(furthestChild)
                    );

                childGroups.Add(childPackGroup);

                foreach (var (child, _) in childPackGroup.ChildToGifts)
                {
                    AvailableChildren[child] = false;
                }
            }

            return childGroups;
        }

        private void CalculateDistancesToSantaEuclid()
        {
            DistancesToSanta = data.Children
                .ToDictionary(
                    c => c,
                    c => GeometryUtils.GetDistance(c, data.Santa)
                );
        }

        private Child GetFurthestChild()
        {
            return DistancesToSanta
                .Where(kv => AvailableChildren[kv.Key])
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .First();
        }

        private List<int> GetPackingSizes()
        {
            int basePackSize = data.Children.Count / PackingCount;

            var ret = Enumerable
                .Repeat(basePackSize, PackingCount)
                .ToList();

            int leftOvers = data.Children.Count - basePackSize * PackingCount;

            for (int i = 0; i < leftOvers; i++)
            {
                ret[i]++;
            }

            return ret;
        }

        private List<Child> GetClosestChildsToSelected(Child furthestChild, int count)
        {
            Func<Child, double> distanceCalculator;

            switch (SelectClosestChildStrategy)
            {
                case Metric.SNOW:
                    distanceCalculator = c => routes.Routes[(c, furthestChild)].TravelTime;
                    break;
                case Metric.EUCLID:
                    distanceCalculator = c => GeometryUtils.GetDistance(c, furthestChild);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var distancesToSelected = AvailableChildren
                .Where(kv => kv.Value && kv.Key != furthestChild)
                .Select(e => e.Key)
                .ToDictionary(
                    c => c,
                    //c => allData.Routes.Routes[(c, furthestChild)].TravelTime
                    //c => GeometryUtils.GetDistance(c, furthestChild)
                    c => distanceCalculator(c)
                );

            return distancesToSelected
                .OrderBy(kv => kv.Value)
                .Take(count)
                .Select(kv => kv.Key)
                .ToList();
        }
    }
}
