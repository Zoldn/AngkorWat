using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.CBrewer.Components
{
    internal class AvailableColorWeightDVar
    {
        public AvailableColorRecord AvailableColorRecord { get; }
        public Dictionary<ColorComponent, int> ColorComponents { get; }
        public Variable DVar { get; private set; }
        public double Value { get; private set; }
        public void Extract()
        {
            Value = DVar.SolutionValue();
        }
        public AvailableColorWeightDVar(Solver solver, AvailableColorRecord availableColorRecord,
            bool isInteger = true)
        {
            AvailableColorRecord = availableColorRecord;

            ColorComponents = new()
            {
                { ColorComponent.RED, availableColorRecord.R },
                { ColorComponent.GREEN, availableColorRecord.G },
                { ColorComponent.BLUE, availableColorRecord.B },
            };

            if (isInteger)
            {
                DVar = solver.MakeIntVar(0.0, AvailableColorRecord.Amount, $"Record {availableColorRecord.Color}");
            }
            else
            {
                DVar = solver.MakeNumVar(0.0, AvailableColorRecord.Amount, $"Record {availableColorRecord.Color}");
            }
        }
    }
}
