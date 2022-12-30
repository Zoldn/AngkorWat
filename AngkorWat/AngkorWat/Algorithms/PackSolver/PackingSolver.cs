﻿using AngkorWat.Components;
using AngkorWat.Constants;
using AngkorWat.IO;
using Google.OrTools.Sat;
using Newtonsoft.Json;
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
        private static string CachePath => Path.Combine(
                AngkorConstants.FilesRoute,
                "cache",
                "packing.json");

        private readonly Data allData;

        public Dictionary<Gift, bool> AvailableGifts { get; private set; }

        public PackingSolver(Data allData)
        {
            this.allData = allData;
            AvailableGifts = new();
        }

        public PackingSolution Solve()
        {
            Console.WriteLine($"starting");

            if (File.Exists(CachePath))
            {
                string json = File.ReadAllText(CachePath);

                var container = JsonConvert.DeserializeObject<PackingSolution>(json);

                if (container == null)
                {
                    throw new FileLoadException();
                }

                Console.WriteLine($"PACKING SOLVER: Using solving from cache");
                return container;
            }

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

            SerializeResult(ret);

            return ret;
        }

        private static void SerializeResult(PackingSolution output)
        {
            var json = JsonConvert.SerializeObject(output, Formatting.Indented);

            File.WriteAllText(CachePath, json);
        }

        private Packing SelectNextGiftPack()
        {
            Console.WriteLine("PACKING SOLVER: selecting next pack");

            CpModel model = new CpModel();


            #region setup variables

            var giftSelectionDVars = AvailableGifts
                .Where(kv => kv.Value)
                .OrderBy(kv => kv.Key.Id)
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

            /*
            solver.StringParameters = ""
                + "random_seed: 10, "
                + "use_absl_random: false, "
                + "permute_variable_randomly: false, "
                + "permute_presolve_constraint_order: false, "
                ;
                //"log_search_progress: true";
                */

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
