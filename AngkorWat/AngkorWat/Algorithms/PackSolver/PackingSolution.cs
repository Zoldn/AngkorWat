using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.PackSolver
{
    internal class Packing
    {
        public List<Phase1Gift> Gifts { get; set; }
        public Packing(IEnumerable<Phase1Gift> gifts)
        {
            Gifts = gifts.ToList();
        }

        public override string ToString()
        {
            return $"Packing with {Gifts.Count} gifts";
        }
    }
    internal class PackingSolution
    {
        public List<Packing> Packings { get; set; }
        public int BagsCount => Packings.Count;
        public List<int> BagsInsides => Packings.Select(e => e.Gifts.Count).ToList();
        public PackingSolution()
        {
            Packings = new();
        }

    }
}
