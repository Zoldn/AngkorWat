using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.CBrewer.Components
{
    internal class TotalWeightDVar
    {
        public Variable DVar { get; private set; }
        public TotalWeightDVar(Solver solver, int minAmount, int maxAmount)
        {
            DVar = solver.MakeIntVar(minAmount, maxAmount, "Total weight");
        }
    }
}
