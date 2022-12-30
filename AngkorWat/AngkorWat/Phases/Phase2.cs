using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Algorithms.Phase2MIP;
using AngkorWat.Components;
using AngkorWat.Constants;
using AngkorWat.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal static class Phase2
    {
        public static void SolveMIP()
        {
            var allData = GetPhase2Data();

            //var mipSolver = new MIPSolver(allData);

            //var solution = mipSolver.Solve();

            //var baseOutput = new Phase2OutputContainer(mapId: "a8e01288-28f8-45ee-9db4-f74fc4ff02c8", solution);

            //SerializeResult(baseOutput, "out");
        }

        public static void Solve()
        {
            var allData = GetPhase2Data();

            //var commonPrices = allData.Gifts
            //    .GroupBy(g => g.Price / 100)
            //    .ToDictionary(
            //        g => g.Key,
            //        g => g.Select(e => e.Type).Distinct().Count()
            //    )
            //    .Where(kv => kv.Value >= 13)
            //    .Select(kv => kv.Key)
            //    .ToList();

            var d = allData
                .Gifts
                .Select(e => new { e.Price, e.Type })
                .Distinct()
                .Count();

            var ddosSolver = new DDOSChildToGiftSolver(allData);

            //var solution = ddosSolver.SolveVictim();
            var solution = ddosSolver.SolveVictim2();

            var baseOutput = new Phase2OutputContainer(mapId: "a8e01288-28f8-45ee-9db4-f74fc4ff02c8", solution.Base);
            var testOutput = new Phase2OutputContainer(mapId: "a8e01288-28f8-45ee-9db4-f74fc4ff02c8", solution.Test);

            SerializeResult(baseOutput, "base");
            SerializeResult(testOutput, "test");

            Console.WriteLine();
        }

        private static void SerializeResult(Phase2OutputContainer output, string suffix)
        {
            var json = JsonConvert.SerializeObject(output);

            string path = Path.Combine(AngkorConstants.FilesRoute, $"phase2_result_{suffix}.json");

            File.WriteAllText(path, json);
        }

        private static Data GetPhase2Data()
        {
            Phase2InputContainer inputContainer = ReadInputData2();

            var ret = new Data();

            ret.Children = inputContainer.children
                .Select((e, index) => new Phase1Child(e))
                .ToList();

            ret.Gifts = inputContainer.gifts
                .Select(e => new Gift(e))
                .ToList();

            return ret;
        }

        private static Phase2InputContainer ReadInputData2()
        {
            string path = Path.Combine(AngkorConstants.FilesRoute, "santa_gifts.json");

            string json = File.ReadAllText(path);

            var container = JsonConvert.DeserializeObject<Phase2InputContainer>(json);

            if (container == null)
            {
                throw new FileLoadException();
            }

            return container;
        }
    }
}
