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
    internal class FullSolution
    {
        public PackingSolution PackingSolution { get; internal set; }
        public DistanceSolution Routes { get; internal set; }
        public TSPSolution Sequences { get; internal set; }
        public FullSolution()
        {
            PackingSolution = new();
            Routes = new();
            Sequences = new();
        }

        public FullSolution(FullSolution deterministicSolution)
        {
            Routes = deterministicSolution.Routes;

            PackingSolution = new();
            Sequences = new();
        }
    }
}
