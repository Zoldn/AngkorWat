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
        public Phase1Child Child { get; }
        public Gift Gift { get; }
        public Phase3ChildToGift(Phase1Child child, Gift gift)
        {
            Child = child;
            Gift = gift;
        }
    }
}
