using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase3Solver
{
    internal class Phase3ChildToGift
    {
        public Phase3Child Child { get; }
        public Phase3Gift Gift { get; }
        public Phase3ChildToGift(Phase3Child child, Phase3Gift gift)
        {
            Child = child;
            Gift = gift;
        }
    }
}
