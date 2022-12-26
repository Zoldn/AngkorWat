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
    internal class OutputContainer
    {
        public string mapID { get; set; }
        public List<move> moves { get; set; }
        public List<List<int>> stackOfBags { get; set; }
        public OutputContainer(string mapId)
        {
            mapID = mapId;

            moves = new();
            stackOfBags = new();
        }

        public OutputContainer(string mapId, AllData allData, FullSolution fullSolution)
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

            /*
            moves = allData.Sequences
                .Sequences
                .Values
                .SelectMany(e => e.Locations.Select(e => new move() 
                { 
                    x = e.X,
                    y = e.Y,
                }))
                .Skip(1)
                .ToList();
            */

            stackOfBags = fullSolution
                .Sequences
                .OrderedPackings
                .Select(e => e.Gifts.Select(g => g.Id).ToList())
                .ToList();

            /*
            stackOfBags = allData.PackingSolution
                .Packings
                .Select(e => e.Gifts.Select(g => g.Id).ToList())
                .ToList();

            stackOfBags.Reverse();
            */
        }
    }

    //presentingGift

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
