using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Algorithms.Phase2MIP;
using AngkorWat.Algorithms.Phase2MIP.HappinessFunctions;
using AngkorWat.Algorithms.Phase3DensePacker;
using AngkorWat.Algorithms.Phase3FullSolver;
using AngkorWat.Algorithms.RouteSolver;
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
    internal static class Phase3
    {
        public static void Solve()
        {
            var data = GetAllData();

            var distSolver = new DistanceSolver(data);

            var routes = distSolver.Solve();

            var phase3Solver = new Phase3Solver(data, routes);

            phase3Solver.Solve();

            //while (availableChilds.Any(kv => kv.Value))
            //{
            //    var denseSolver = new DensePackSolver(data);

            //    var maxPackingSize = denseSolver.GetMostDensePacking(availableGifts
            //        .Where(kv => kv.Value)
            //        .Select(kv => kv.Key)
            //        .ToList()
            //        );


            //}

            //var mipSolver = new MIPSolver(data, new LinearHappinessFunction());

            //var childToGiftSolution = mipSolver.Solve();

            //var tspSolver = new Phase3TSPSolver(phase3Data, phase1Solution, childToGiftSolution);

            //var tspSolution = tspSolver.Solve();

            //var output = new Phase1OutputContainer(data, null);

            //SerializeResult(output);

        }

        private static void SerializeResult(Phase1OutputContainer output)
        {
            var json = JsonConvert.SerializeObject(output);

            string path = Path.Combine(AngkorConstants.FilesRoute, "result.json");

            File.WriteAllText(path, json);
        }

        private static Data GetAllData()
        {
            var inputContainer = ReadInputData();

            var ret = new Data
            {
                MapId = "dd6ed651-8ed6-4aeb-bcbc-d8a51c8383cc",

                MaxGiftCost = 50000,

                Children = inputContainer.children
                    .Select((e, index) => new Child(e, index + 1))
                    .ToList(),

                SnowAreas = inputContainer.snowAreas
                    .Select(e => new SnowArea(e))
                    .ToList(),

                Gifts = inputContainer.gifts
                    .Select(e => new Gift(e))
                    .ToList()
            };

            return ret;
        }

        private static Phase3InputContainer ReadInputData()
        {
            string path = Path.Combine(AngkorConstants.FilesRoute, "phase3_santa.json");

            string json = File.ReadAllText(path);

            var container = JsonConvert.DeserializeObject<Phase3InputContainer>(json);

            if (container == null)
            {
                throw new FileLoadException();
            }

            return container;
        }
    }
}
