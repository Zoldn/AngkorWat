using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Components;
using Google.OrTools.Sat;
using AngkorWat.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using AngkorWat.Algorithms.Phase2MIP.HappinessFunctions;
using AngkorWat.Algorithms.DistSolver;
using System.Diagnostics;
using AngkorWat.Algorithms.Phase3FullSolver;

namespace AngkorWat.Algorithms.Phase2MIP
{
    internal interface IHappinessFunction
    {
        public int GetHappiness(ChildrenGroup childrenGroup, GiftGroup giftGroup);
    }

    internal class ChildrenGroup
    {
        public string Gender { get; }
        public int Age { get; }
        public int Count => AvailableChildren.Count;
        public ChildPackingGroup ChildPackingGroup { get; private set; }
        public Dictionary<Child, bool> AvailableChildren { get; set; }
        public ChildrenGroup(IEnumerable<Child> children, ChildPackingGroup childPackingGroup = null)
        {
            Gender = children
                .Select(e => e.Gender)
                .Distinct()
                .Single();

            Age = children
                .Select(e => e.Age)
                .Distinct()
                .Single();

            ChildPackingGroup = childPackingGroup;

            AvailableChildren = children
                .ToDictionary(
                    c => c,
                    c => true
                );
        }

        public override string ToString()
        {
            return $"Child group of {Age}/{Gender} of {Count}";
        }
    }

    internal class GiftGroup
    {
        public int Price { get; }
        public string Type { get; }
        public int Weight { get; }
        public int Volume { get; }
        public int Count => AvailableGifts.Count;
        public Dictionary<Gift, bool> AvailableGifts { get; set; }
        public GiftGroup(IEnumerable<Gift> gifts)
        {
            Price = gifts
                .Select(e => e.Price)
                .Distinct()
                .Single();

            Type = gifts
                .Select(e => e.Type)
                .Distinct()
                .Single();

            Weight = gifts
                .Select(e => e.Weight)
                .Distinct()
                .Single();

            Volume = gifts
                .Select(e => e.Weight)
                .Distinct()
                .Single();

            AvailableGifts = gifts
                .ToDictionary(
                    g => g,
                    g => true
                );
        }

        public override string ToString()
        {
            return $"Gift group {Type}/{Price} of {Count}";
        }
    } 

    /// <summary>
    /// Переменная кол-ва подарков определенного типа для множества детей определенного типа
    /// </summary>
    internal class ChildGroupToGiftGroupDVar
    {
        public ChildrenGroup ChildrenGroup { get; }
        public GiftGroup GiftGroup { get; }
        public IntVar DVar { get; private set; }
        public int Value { get; private set; }
        public int Happiness { get; set; }
        public ChildGroupToGiftGroupDVar(ChildrenGroup child, GiftGroup gift, CpModel model)
        {
            ChildrenGroup = child;
            GiftGroup = gift;

            DVar = model.NewIntVar(
                lb: 0,
                ub: Math.Min(ChildrenGroup.Count, GiftGroup.Count),
                name: $"DVAR {ChildrenGroup} - {GiftGroup}");

            Happiness = 0;
        }
        public void Extract(CpSolver solver)
        {
            Value = (int)solver.Value(DVar);
        }
    }

    internal class MIPSolver
    {
        private readonly Data data;
        private IHappinessFunction happinessFunction;
        public int WeightLimit { get; init; }
        public int VolumeLimit { get; init; }
        public MIPSolver(Data data, IHappinessFunction happinessFunction)
        {
            this.data = data;
            this.happinessFunction = happinessFunction;
            WeightLimit = 0;
            VolumeLimit = 0;
        }

        public Dictionary<Child, Gift> Solve(List<Child> targetChildren, List<Gift> availableGifts,
            Dictionary<Child, ChildPackingGroup> childToPackGroups)
        {
            var model = new CpModel();

            var childrenGroups = GroupChildren(targetChildren, childToPackGroups);
            var giftGroups = GroupGifts(availableGifts);

            var childGroupToGiftGroupDVars = InitializeDVars(model, 
                childrenGroups, giftGroups);

            InitializeHappiness(childGroupToGiftGroupDVars, happinessFunction);

            InitializeFullChildGroupConstraints(model, childGroupToGiftGroupDVars);

            InitializeLimitedGiftGroups(model, childGroupToGiftGroupDVars);

            InitializeTotalCostLimitation(model, childGroupToGiftGroupDVars);

            InitializeWeightVolumeConstraints(model, childGroupToGiftGroupDVars, childToPackGroups);

            LinearExpr happinessExpr = InitializeHappinessObjective(model, childGroupToGiftGroupDVars);

            model.Maximize(happinessExpr);

            double timeLimit = 300.0d;
            int workers = 3;
            double relGap = 1e-2;

            NumberFormatInfo nfi = new NumberFormatInfo
            {
                NumberDecimalSeparator = "."
            };

            CpSolver solver = new()
            {
                StringParameters =
                    $"relative_gap_limit: {relGap.ToString(nfi)}," +
                    $"log_search_progress: true," +
                    //$"search_branching: 1, " +
                    $"subsolvers: 'default_lp'," +
                    //$"use_lns_only: true," +
                    //$"use_relaxation_lns: true"
                    $"max_time_in_seconds: {timeLimit}, " +
                    $"num_search_workers: {workers}"
            };

            solver.SetLogCallback(new StringToVoidDelegate(s => Console.WriteLine(s)));

            var status = solver.Solve(model);

            var objValue = solver.ObjectiveValue;
            var boundValue = solver.BestObjectiveBound;

            Console.WriteLine($"Problem solved with {objValue} (boundary = {boundValue})");

            var selectedPairs = ExtractSolution(solver, childGroupToGiftGroupDVars);

            Console.WriteLine($"Total cost is {selectedPairs.Sum(e => e.Value.Price)}");

            Debug.Assert(selectedPairs.Count == targetChildren.Count);
            Debug.Assert(selectedPairs.Select(e => e.Value).Distinct().Count() == targetChildren.Count);

            return selectedPairs;
        }

        private void InitializeWeightVolumeConstraints(CpModel model, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars,
            Dictionary<Child, ChildPackingGroup> childToPackGroups)
        {
            var lu = childGroupToGiftGroupDVars
                .ToLookup(dvar => dvar.ChildrenGroup.ChildPackingGroup);

            foreach (var (_, dvars) in lu)
            {
                if (WeightLimit > 0)
                {
                    model.Add(
                        LinearExpr.WeightedSum(
                            dvars.Select(e => e.DVar),
                            dvars.Select(e => e.GiftGroup.Weight)
                            ) <= WeightLimit
                        );
                }

                if (VolumeLimit > 0)
                {
                    model.Add(
                        LinearExpr.WeightedSum(
                            dvars.Select(e => e.DVar),
                            dvars.Select(e => e.GiftGroup.Volume)
                            ) <= VolumeLimit
                        );
                }
            }
        }

        private static List<ChildGroupToGiftGroupDVar> InitializeDVars(CpModel model, List<ChildrenGroup> childrenGroups, List<GiftGroup> giftGroups)
        {
            var childGroupToGiftGroupDVars = new List<ChildGroupToGiftGroupDVar>();

            foreach (var childGroup in childrenGroups)
            {
                foreach (var giftGroup in giftGroups)
                {
                    var childGroupToGiftGroupDVar = new ChildGroupToGiftGroupDVar(childGroup, giftGroup, model);

                    childGroupToGiftGroupDVars.Add(childGroupToGiftGroupDVar);
                }
            }

            return childGroupToGiftGroupDVars;
        }

        private List<GiftGroup> GroupGifts(List<Gift> availableGifts)
        {
            var giftGroups = new List<GiftGroup>();

            var giftLU = availableGifts
                .ToLookup(e => (e.Price, e.Type, e.Weight, e.Volume));

            foreach (var (_, gifts) in giftLU)
            {
                var giftGroup = new GiftGroup(gifts);

                giftGroups.Add(giftGroup);
            }

            return giftGroups;
        }

        private static List<ChildrenGroup> GroupChildren(List<Child> targetChildren, 
            Dictionary<Child, ChildPackingGroup> childToPackGroups)
        {
            var childrenLU = targetChildren
                .ToLookup(e => (e.Age, e.Gender, childToPackGroups[e]));

            var childrenGroups = new List<ChildrenGroup>();

            foreach (var ((_, _, childToPackGroup), childs) in childrenLU)
            {
                var childrenGroup = new ChildrenGroup(childs, childToPackGroup);

                childrenGroups.Add(childrenGroup);
            }

            return childrenGroups;
        }

        private Dictionary<Child, Gift> ExtractSolution(CpSolver solver, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars)
        {
            var ret = new Dictionary<Child, Gift>();

            foreach (var dvar in childGroupToGiftGroupDVars)
            {
                dvar.Extract(solver);
            }

            foreach (var dvar in childGroupToGiftGroupDVars)
            {
                if (dvar.Value == 0)
                {
                    continue;
                }

                var childs = dvar.ChildrenGroup
                    .AvailableChildren
                    .Where(kv => kv.Value)
                    .Take(dvar.Value)
                    .Select(kv => kv.Key)
                    .ToList();

                var gifts = dvar.GiftGroup
                    .AvailableGifts
                    .Where(kv => kv.Value)
                    .Take(dvar.Value)
                    .Select(kv => kv.Key)
                    .ToList();

                for (int i = 0; i < dvar.Value; i++)
                {
                    ret.Add(childs[i], gifts[i]);
                }

                foreach (var child in childs)
                {
                    dvar.ChildrenGroup
                        .AvailableChildren[child] = false;
                }

                foreach (var gift in gifts)
                {
                    dvar.GiftGroup
                        .AvailableGifts[gift] = false;
                }
            }

            return ret;
        }

        private void InitializeHappiness(List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars, 
            IHappinessFunction happinessFunction)
        {
            foreach (var dvar in childGroupToGiftGroupDVars)
            {
                dvar.Happiness = happinessFunction.GetHappiness(dvar.ChildrenGroup, dvar.GiftGroup);
            }
        }

        private LinearExpr InitializeHappinessObjective(CpModel model, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars)
        {
            return LinearExpr.WeightedSum(
                    childGroupToGiftGroupDVars.Select(e => e.DVar),
                    childGroupToGiftGroupDVars.Select(e => e.Happiness)
                );
        }

        private void InitializeTotalCostLimitation(CpModel model, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars)
        {
            model.Add(
                LinearExpr.WeightedSum(
                    childGroupToGiftGroupDVars.Select(e => e.DVar),
                    childGroupToGiftGroupDVars.Select(e => e.GiftGroup.Price)
                ) <= data.MaxGiftCost
                );
        }

        private void InitializeLimitedGiftGroups(CpModel model, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars)
        {
            var lu = childGroupToGiftGroupDVars
                .ToLookup(e => e.GiftGroup);

            foreach (var (giftGroup, dvars) in lu)
            {
                model.Add(LinearExpr.Sum(dvars.Select(e => e.DVar)) <= giftGroup.Count);
            }
        }

        private void InitializeFullChildGroupConstraints(CpModel model, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars)
        {
            var lu = childGroupToGiftGroupDVars
                .ToLookup(e => e.ChildrenGroup);

            foreach (var (childrenGroup, dvars) in lu)
            {
                model.Add(LinearExpr.Sum(dvars.Select(e => e.DVar)) == childrenGroup.Count);
            }
        }
    }
}
