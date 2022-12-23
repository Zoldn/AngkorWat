using AngkorWat.Components;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.PackSolver
{
    internal class PackingSolver
    {
        private static readonly long MAX_WEIGHT = 200;
        private static readonly long MAX_VOLUME = 100;

        private readonly AllData allData;

        public Dictionary<Gift, bool> AvailableGifts { get; private set; }

        public PackingSolver(AllData allData)
        {
            this.allData = allData;
            AvailableGifts = new();
        }

        public PackingSolution Solve()
        {
            var ret = new PackingSolution();

            AvailableGifts = allData.Gifts
                .ToDictionary(
                    g => g,
                    g => true
                );

            while (AvailableGifts.Any(kv => kv.Value))
            {
                var packing = SelectNextGiftPack();

                ret.Packings.Add(packing);
            }

            return ret;
        }

        private Packing SelectNextGiftPack()
        {
            Console.WriteLine("PACKING SOLVER: selecting next pack");

            CpModel model = new CpModel();

            #region setup variables

            var giftSelectionDVars = AvailableGifts
                .Where(kv => kv.Value)
                .ToDictionary(
                    g => g.Key,
                    g => model.NewBoolVar($"DVAR {g} is taken")
                    );

            var weightExpr = LinearExpr.WeightedSum(
                giftSelectionDVars.Values,
                giftSelectionDVars.Keys.Select(e => e.Weight)
                );

            model.Add(weightExpr <= MAX_WEIGHT);

            var volumeExpr = LinearExpr.WeightedSum(
                giftSelectionDVars.Values,
                giftSelectionDVars.Keys.Select(e => e.Volume)
                );

            model.Add(volumeExpr <= MAX_VOLUME);

            #endregion

            #region setup objective

            var objExpr = LinearExpr
                .Sum(giftSelectionDVars.Values);

            model.Maximize(objExpr);

            #endregion

            #region solve

            CpSolver solver = new CpSolver();

            var status = solver.Solve(model);

            #endregion

            if (status != CpSolverStatus.Optimal
                && status != CpSolverStatus.Feasible
                )
            {
                throw new ArgumentOutOfRangeException("U WOT M8?");
            }

            var selectedGifts = giftSelectionDVars
                .Where(kv => solver.BooleanValue(kv.Value))
                .Select(e => e.Key)
                .ToList();

            Debug.Assert(selectedGifts.Sum(e => e.Weight) <= MAX_WEIGHT);
            Debug.Assert(selectedGifts.Sum(e => e.Volume) <= MAX_VOLUME);

            var packing = new Packing(selectedGifts);

            foreach (var gift in selectedGifts)
            {
                AvailableGifts[gift] = false;
            }

            Console.WriteLine($"PACKING SOLVER: pack selected with {selectedGifts.Count} gifts and " +
                $"{selectedGifts.Sum(e => e.Weight)} mass " +
                $"{selectedGifts.Sum(e => e.Volume)} volume");

            return packing;
        }
    }
}
