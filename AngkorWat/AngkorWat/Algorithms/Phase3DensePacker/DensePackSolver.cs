using AngkorWat.Components;
using AngkorWat.Utils;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase3DensePacker
{
    internal class GiftGroupDVar
    {
        public int Weight { get; }
        public int Volume { get; }
        public List<Gift> Gifts { get; }
        public IntVar DVar { get; }
        public int Value { get; private set; }
        public GiftGroupDVar(CpModel model, IEnumerable<Gift> gifts)
        {
            Weight = gifts
                .Select(e => e.Weight)
                .Distinct()
                .Single();

            Volume = gifts
                .Select(e => e.Volume)
                .Distinct()
                .Single();

            Gifts = gifts.ToList();

            DVar = model.NewIntVar(
                lb: 0,
                ub: Gifts.Count,
                name: $"DVar gift group {Volume}/{Weight}"
                );
        }

        public void Extract(CpSolver solver)
        {
            Value = (int)solver.Value(DVar);
        }
    }

    internal class DensePackSolver
    {
        private readonly Data data;
        public DensePackSolver(Data data)
        {
            this.data = data;
        }

        public int GetMostDensePacking(List<Gift> avialbleGifts)
        {
            var model = new CpModel();

            var giftDVars = InitializeVariables(avialbleGifts, model);

            InitializeConstraints(giftDVars, model);

            var objective = InitializeObjective(giftDVars, model);

            model.Maximize(objective);

            var solver = new CpSolver();

            var status = solver.Solve(model);

            if (status != CpSolverStatus.Optimal
                && status != CpSolverStatus.Feasible
                )
            {
                throw new Exception();
            }

            int objValue = (int)solver.ObjectiveValue;

            return objValue;
        }

        private LinearExpr InitializeObjective(List<GiftGroupDVar> giftDVars, CpModel model)
        {
            return LinearExpr.Sum(giftDVars.Select(d => d.DVar));
        }

        private void InitializeConstraints(List<GiftGroupDVar> giftDVars, CpModel model)
        {
            model.Add(
                LinearExpr.WeightedSum(
                    giftDVars.Select(e => e.DVar),
                    giftDVars.Select(e => e.Weight)
                    ) <= data.WeightLimit
                );

            model.Add(
                LinearExpr.WeightedSum(
                    giftDVars.Select(e => e.DVar),
                    giftDVars.Select(e => e.Volume)
                    ) <= data.VolumeLimit
                );
        }

        private static List<GiftGroupDVar> InitializeVariables(List<Gift> avialbleGifts, CpModel model)
        {
            var giftGroups = avialbleGifts
                            .ToLookup(g => (g.Weight, g.Volume));

            var giftDVars = new List<GiftGroupDVar>();

            foreach (var (_, gifts) in giftGroups)
            {
                var dvar = new GiftGroupDVar(model, gifts);

                giftDVars.Add(dvar);
            }

            return giftDVars;
        }
    }
}
