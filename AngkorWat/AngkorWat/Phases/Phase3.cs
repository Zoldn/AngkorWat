using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Algorithms.Phase2MIP;
using AngkorWat.Algorithms.Phase3Solver;
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
            var phase3Data = GetAllData();

            var phase2Data = ToPhase2Data(phase3Data);

            var phase2MIPSolver = new MIPSolver(phase2Data);

            var childToGifts = phase2MIPSolver.Solve();

            var selectedPairs = ToPhase3ChildsToGift(childToGifts, phase3Data);

            var phase1Data = ToPhase1Data(phase3Data);
        }

        private static List<Phase3ChildToGift> ToPhase3ChildsToGift(
            ChildToGiftSolution childToGifts, Phase3Data phase3Data)
        {
            var childDict = phase3Data
                .Children
                .ToDictionary(c => c.Id);

            var giftDict = phase3Data
                .Gifts
                .ToDictionary(g => g.Id);

            return childToGifts.ChildToGifts
                .Select(e => new Phase3ChildToGift(
                    childDict[e.Child.Id],
                    giftDict[e.Gift.Id]
                    ))
                .ToList();
        }

        private static Phase1Data ToPhase1Data(Phase3Data data)
        {
            Phase1Data phase1Data = new Phase1Data();

            phase1Data.Children = data.Children
                .Select(c => new Phase1Child(c) as IPhase1Child)
                .ToList();

            phase1Data.SnowAreas = data.SnowAreas;

            phase1Data.Gifts = data.Gifts
                .Select(g => new Phase1Gift(g))
                .ToList();

            return phase1Data;
        }

        private static Phase2Data ToPhase2Data(Phase3Data data)
        {
            Phase2Data phase2Data = new Phase2Data();

            phase2Data.Children = data.Children
                .Select(c => new Phase2Child(c))
                .ToList();

            phase2Data.Gifts = data.Gifts
                .Select(g => new Phase2Gift(g))
                .ToList();

            return phase2Data;
        }

        private static Phase3Data GetAllData()
        {
            var inputContainer = ReadInputData();

            var ret = new Phase3Data();

            ret.Children = inputContainer.children
                .Select((e, index) => new Phase3Child(e))
                .ToList();

            ret.SnowAreas = inputContainer.snowAreas
                .Select(e => new SnowArea(e))
                .ToList();

            ret.Gifts = inputContainer.gifts
                .Select(e => new Phase3Gift(e))
                .ToList();

            return ret;
        }

        private static Phase3InputContainer ReadInputData()
        {
            string path = Path.Combine(AngkorConstants.FilesRoute, "santa_phase3.json");

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
