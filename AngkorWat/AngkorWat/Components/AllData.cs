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
    internal class AllData
    {
        public Santa Santa { get; }
        public List<Child> Children { get; set; }
        public List<SnowArea> SnowAreas { get; set; }
        public List<Gift> Gifts { get; set; }
        public PackingSolution PackingSolution { get; internal set; }
        public DistanceSolution Routes { get; internal set; }
        public TSPSolution Sequences { get; internal set; }

        public AllData()
        {
            Santa = new();

            Children = new();
            SnowAreas = new();
            Gifts = new();

            PackingSolution = new();
            Routes = new();
            Sequences = new();
        }
    }
}
