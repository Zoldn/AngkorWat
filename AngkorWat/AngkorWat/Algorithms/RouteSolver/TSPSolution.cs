using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.RouteSolver
{
    internal class LocationSequence
    {
        public Packing Packing { get; }
        public List<ILocation> Locations { get; set; }
        public LocationSequence(Packing packing, List<ILocation> locations)
        {
            Packing = packing;
            Locations = locations;
        }
    }

    internal class TSPSolution
    {
        public Dictionary<Packing, LocationSequence> Sequences { get; set; }
        public TSPSolution()
        {
            Sequences = new();
        }
    }
}
