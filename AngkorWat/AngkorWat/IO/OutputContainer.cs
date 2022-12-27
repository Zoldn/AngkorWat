using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class Phase1OutputContainer
    {
        public string mapID { get; set; }
        public List<move> moves { get; set; }
        public List<List<int>> stackOfBags { get; set; }
        public Phase1OutputContainer(string mapId)
        {
            mapID = mapId;

            moves = new();
            stackOfBags = new();
        }

        public Phase1OutputContainer(string mapId, Phase1Data allData, Phase1Solution fullSolution)
        {
            mapID = mapId;

            moves = fullSolution
                .Sequences
                .FullRoute
                .Select(e => new move() 
                {
                    x = e.X,
                    y = e.Y,
                })
                .ToList();

            stackOfBags = fullSolution
                .Sequences
                .OrderedPackings
                .Select(e => e.Gifts.Select(g => g.Id).ToList())
                .ToList();
        }
    }


    internal class Phase2OutputContainer
    {
        public string mapID { get; set; }
        public List<presentingGift> presentingGifts { get; set; }
        public Phase2OutputContainer(string mapId)
        {
            mapID = mapId;

            presentingGifts = new();
        }

        
        public Phase2OutputContainer(string mapId, ChildToGiftSolution solution)
        {
            mapID = mapId;

            presentingGifts = solution
                .ChildToGifts
                .Select(g => new presentingGift()
                {
                    childID = g.Child.Id,
                    giftID = g.Gift.Id,
                })
                .ToList();
        }
    }
}
