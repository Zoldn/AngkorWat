using AngkorWat.Algorithms.Phase2DDOS;
using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.PackSolver
{
    internal class Phase3PackingSolver
    {
        private static readonly long MAX_WEIGHT = 200;
        private static readonly long MAX_VOLUME = 100;

        private Phase1Solution phase1Solution;
        private ChildToGiftSolution childToGiftSolution;

        public Phase3PackingSolver(Phase1Solution phase1Solution, ChildToGiftSolution childToGiftSolution)
        {
            this.phase1Solution = phase1Solution;
            this.childToGiftSolution = childToGiftSolution;
        }

        internal void Solve()
        {
            
        }
    }
}
