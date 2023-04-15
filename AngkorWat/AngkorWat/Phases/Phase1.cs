using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
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
    internal static class Phase1
    {
        public static void Phase1SingleStart()
        {
            var allData = GetData();

            var deterministicSolution = new Phase1Solution();

            var distanceSolver = new DistanceSolver(allData);

            deterministicSolution.Routes = distanceSolver.Solve();

            var curSolution = new Phase1Solution(deterministicSolution);

            var packingSolver = new PackingSolver(allData);

            curSolution.PackingSolution = packingSolver.Solve();

            var tspSolver = new TSPSolver(allData, curSolution);

            curSolution.Sequences = tspSolver.Solve();

            var output = new Phase1OutputContainer(allData, curSolution.Sequences);

            SerializeResult(output);
        }

        public static void Phase1MultiStart()
        {
            var data = GetData();

            var deterministicSolution = new Phase1Solution();

            var distanceSolver = new DistanceSolver(data);

            deterministicSolution.Routes = distanceSolver.Solve();

            var solutionVariants = new List<Phase1Solution>();

            for (int i = 0; i < 10; i++)
            {
                var curSolution = new Phase1Solution(deterministicSolution);

                solutionVariants.Add(curSolution);

                var packingSolver = new PackingSolver(data);

                curSolution.PackingSolution = packingSolver.Solve();

                var tspSolver = new TSPSolver(data, curSolution);

                curSolution.Sequences = tspSolver.Solve();
            }

            solutionVariants = solutionVariants
                .OrderBy(v => v.Sequences.TravelTime)
                .ToList();

            var times = solutionVariants
                .Select(v => v.Sequences.TravelTime)
                .ToList();

            var bestSolution = solutionVariants
                .First();

            var output = new Phase1OutputContainer(data, bestSolution.Sequences);

            SerializeResult(output);

            //foreach (var solutionVariant in solutionVariants)
            //{
            //    var output = new Phase1OutputContainer(data, solutionVariant.Sequences);

            //    SerializeResult(output);
            //}
        }

        private static void SerializeResult(Phase1OutputContainer output)
        {
            var json = JsonConvert.SerializeObject(output);

            string path = Path.Combine(AngkorConstants.FilesRoute, "result.json");

            File.WriteAllText(path, json);
        }

        private static Data GetData()
        {
            var inputContainer = ReadInputData();

            var ret = new Data()
            {
                MapId = "faf7ef78-41b3-4a36-8423-688a61929c08",

                Children = inputContainer.children
                    .Select((e, index) => new Child(e, index + 1))
                    .ToList(),

                SnowAreas = inputContainer.snowAreas
                    .Select(e => new SnowArea(e))
                    .ToList(),

                Gifts = inputContainer.gifts
                    .Select(e => new Gift(e))
                    .ToList(),
            };

            return ret;
        }

        private static Phase1InputContainer ReadInputData()
        {
            string path = Path.Combine(AngkorConstants.FilesRoute, "phase1_santa.json");

            string json = File.ReadAllText(path);

            var container = JsonConvert.DeserializeObject<Phase1InputContainer>(json);

            if (container == null)
            {
                throw new FileLoadException();
            }

            return container;
        }
    }
}
