using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Components;
using Google.OrTools.Sat;
using AngkorWat.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AngkorWat.Algorithms.Phase2MIP.HappinessFunctions;

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
        public int Count => Children.Count;
        public List<Phase2Child> Children { get; }
        public Dictionary<Phase2Child, bool> AvailableChildren { get; set; }
        public ChildrenGroup(List<Phase2Child> children)
        {
            Children = children;

            Gender = children
                .Select(e => e.Gender)
                .Distinct()
                .Single();

            Age = children
                .Select(e => e.Age)
                .Distinct()
                .Single();

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
        public int Count => Gifts.Count;
        public List<Phase2Gift> Gifts { get; }
        public Dictionary<Phase2Gift, bool> AvailableGifts { get; set; }

        public GiftGroup(List<Phase2Gift> gifts)
        {
            Gifts = gifts;

            Price = gifts
                .Select(e => e.Price)
                .Distinct()
                .Single();

            Type = gifts
                .Select(e => e.Type)
                .Distinct()
                .Single();

            AvailableGifts = Gifts
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
        private readonly Phase2Data data;
        public MIPSolver(Phase2Data data)
        {
            this.data = data;
        }

        public ChildToGiftSolution Solve()
        {
            CpModel model = new CpModel();

            var childrenLU = data.Children
                .ToLookup(e => (e.Age, e.Gender));

            var childrenGroups = new List<ChildrenGroup>();

            foreach (var (_, childs) in childrenLU)
            {
                var childrenGroup = new ChildrenGroup(childs.ToList());

                childrenGroups.Add(childrenGroup);
            }

            var giftGroups = new List<GiftGroup>();

            var giftLU = data.Gifts
                .ToLookup(e => (e.Price, e.Type));

            foreach (var (_, gifts) in giftLU)
            {
                var giftGroup = new GiftGroup(gifts.ToList());

                giftGroups.Add(giftGroup);
            }



            var childGroupToGiftGroupDVars = new List<ChildGroupToGiftGroupDVar>();

            foreach (var childGroup in childrenGroups)
            {
                foreach (var giftGroup in giftGroups)
                {
                    var childGroupToGiftGroupDVar = new ChildGroupToGiftGroupDVar(childGroup, giftGroup, model);

                    childGroupToGiftGroupDVars.Add(childGroupToGiftGroupDVar);
                }
            }

            IHappinessFunction happinessFunction = new LinearHappinessFunction();

            InitializeHappiness(childGroupToGiftGroupDVars, happinessFunction);

            InitializeFullChildGroupConstraints(model, childGroupToGiftGroupDVars);

            InitializeLimitedGiftGroups(model, childGroupToGiftGroupDVars);

            InitializeTotalCostLimitation(model, childGroupToGiftGroupDVars);

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

            Console.WriteLine($"Total cost is {selectedPairs.Sum(e => e.Gift.Price)}");

            var d1 = selectedPairs
                .Select(e => e.Child)
                .Distinct()
                .Count();

            var d2 = selectedPairs
                .Select(e => e.Gift)
                .Distinct()
                .Count();

            var solution = new ChildToGiftSolution()
            {
                ChildToGifts = selectedPairs,
            };

            return solution;
        }

        private List<ChildToGift> ExtractSolution(CpSolver solver, 
            List<ChildGroupToGiftGroupDVar> childGroupToGiftGroupDVars)
        {
            var ret = new List<ChildToGift>();

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
                    ret.Add(new ChildToGift(childs[i], gifts[i]));
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
                ) <= Phase2Data.MAX_COST
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
