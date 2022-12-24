using AngkorWat.Algorithms.DistSolver;
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

        public OutputContainer(string mapId, AllData allData)
        {
            mapID = mapId;

            moves = allData
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

            stackOfBags = allData.Sequences
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
}
