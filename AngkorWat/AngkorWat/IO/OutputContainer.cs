using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Algorithms.RouteSolver;
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
        public Phase1OutputContainer(Data data, TSPSolution tspSolution)
        {
            mapID = data.MapId;

            moves = tspSolution
                .FullRoute
                .Select(e => new move() 
                {
                    x = e.X,
                    y = e.Y,
                })
                .ToList();

            tspSolution.OrderedPackings.Reverse();

            stackOfBags = tspSolution
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

        
        public Phase2OutputContainer(string mapId, Dictionary<Child, Gift> solution)
        {
            mapID = mapId;

            presentingGifts = solution
                .Select(g => new presentingGift()
                {
                    childID = g.Key.Id,
                    giftID = g.Value.Id,
                })
                .ToList();
        }
    }
}
