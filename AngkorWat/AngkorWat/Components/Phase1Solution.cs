using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.RouteSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Phase1Solution
    {
        public PackingSolution PackingSolution { get; internal set; }
        public DistanceSolution Routes { get; internal set; }
        public TSPSolution Sequences { get; internal set; }
        public Phase1Solution()
        {
            PackingSolution = new();
            Routes = new();
            Sequences = new();
        }

        public Phase1Solution(Phase1Solution deterministicSolution)
        {
            Routes = deterministicSolution.Routes;

            PackingSolution = new();
            Sequences = new();
        }
    }
}
