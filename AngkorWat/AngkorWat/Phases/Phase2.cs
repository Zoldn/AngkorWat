using AngkorWat.Algorithms.Phase2DDOS;
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

            var ddosSolver = new DDOSChildToGiftSolver(allData);

            var solution = ddosSolver.SolveVictim();

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

        private static Phase2Data GetPhase2Data()
        {
            Phase2InputContainer inputContainer = ReadInputData2();

            var ret = new Phase2Data();

            ret.Children = inputContainer.children
                .Select((e, index) => new Phase2Child(e))
                .ToList();

            ret.Gifts = inputContainer.gifts
                .Select(e => new Phase2Gift(e))
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
