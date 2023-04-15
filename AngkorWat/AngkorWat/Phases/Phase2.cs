using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Algorithms.Phase2MIP;
using AngkorWat.Algorithms.Phase2MIP.HappinessFunctions;
using AngkorWat.Algorithms.Phase3FullSolver;
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
        public static void SolvePhase2Packing()
        {
            var allData = GetPhase2Data();

            var happinessFunction = new Phase2TrueHappinessFunction(allData);

            var mipSolver = new MIPSolver(allData, happinessFunction);

            var solution = mipSolver.Solve(
                targetChildren: allData.Children,
                availableGifts: allData.Gifts,
                childToPackGroups: allData.Children.ToDictionary(c => c, c => null as ChildPackingGroup)
                );

            var baseOutput = new Phase2OutputContainer(mapId: "a8e01288-28f8-45ee-9db4-f74fc4ff02c8", solution);

            SerializeResult(baseOutput, "out");
        }

        public static void Solve()
        {
            var allData = GetPhase2Data();


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

            var ret = new Data
            {
                MapId = "a8e01288-28f8-45ee-9db4-f74fc4ff02c8",

                Children = inputContainer.children
                    .Select((e, index) => new Child(e))
                    .ToList(),

                Gifts = inputContainer.gifts
                    .Select(e => new Gift(e))
                    .ToList(),

                MaxGiftCost = 100000,
            };

            return ret;
        }

        private static Phase2InputContainer ReadInputData2()
        {
            string path = Path.Combine(AngkorConstants.FilesRoute, "phase2_santa.json");

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
