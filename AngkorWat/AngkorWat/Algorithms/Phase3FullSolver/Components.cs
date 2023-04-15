using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase3FullSolver
{
    internal class ChildPackingGroup
    {
        public Dictionary<Child, Gift?> ChildToGifts { get; set; }
        public int Count => ChildToGifts.Count;
        public ChildPackingGroup(IEnumerable<Child> children)
        {
            ChildToGifts = children
                .ToDictionary(
                    c => c,
                    c => null as Gift
                );
        }
    }
}
