using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase2DDOS
{
    internal class ChildToGift
    {
        public Phase2Child Child { get; }
        public Phase2Gift Gift { get; }
        public ChildToGift(Phase2Child child, Phase2Gift gift)
        {
            Child = child;
            Gift = gift;
        }
    }

    internal class ChildToGiftSolution
    {
        public List<ChildToGift> ChildToGifts { get; set; }
        public ChildToGiftSolution()
        {
            ChildToGifts = new List<ChildToGift>();
        }
    }
}
