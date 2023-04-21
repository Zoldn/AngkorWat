using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.CBrewer.Components
{
    internal class ColorDiffDVar
    {
        public ColorComponent ColorComponent { get; }
        public Variable LeftDVar { get; }
        public Variable RightDVar { get; }
        public ColorDiffDVar(Solver solver, ColorComponent colorComponent, int totalAmount, bool isInteger)
        {
            ColorComponent = colorComponent;

            if (isInteger)
            {
                LeftDVar = solver.MakeIntVar(0.0d, 255.0d, $"DVar left side {ColorComponent}");
                RightDVar = solver.MakeIntVar(0.0d, 255.0d, $"DVar left side {ColorComponent}");
            }
            else
            {
                LeftDVar = solver.MakeNumVar(0.0d, 255.0d, $"DVar left side {ColorComponent}");
                RightDVar = solver.MakeNumVar(0.0d, 255.0d, $"DVar left side {ColorComponent}");
            }
        }
        internal double LeftValue { get; private set; }
        internal double RightValue { get; private set; }
        internal void Extract()
        {
            LeftValue = LeftDVar.SolutionValue();
            RightValue = RightDVar.SolutionValue();
        }
    }
}
