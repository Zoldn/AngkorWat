using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase2DDOS
{
    internal class Phase2ChildToGift
    {
        public Phase1Child Child { get; }
        public Gift Gift { get; }
        public Phase2ChildToGift(Phase1Child child, Gift gift)
        {
            Child = child;
            Gift = gift;
        }
    }

    internal class ChildToGiftSolution
    {
        public List<Phase2ChildToGift> ChildToGifts { get; set; }
        public ChildToGiftSolution()
        {
            ChildToGifts = new List<Phase2ChildToGift>();
        }
    }
}
