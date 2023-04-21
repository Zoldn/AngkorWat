using AngkorWat.Algorithms.CBrewer.Components;
using AngkorWat.Utils;
using Google.OrTools.LinearSolver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.CBrewer
{
    internal enum ColorComponent
    {
        RED,
        GREEN,
        BLUE,
    }

    internal class ColorBrewer
    {
        private static readonly List<ColorComponent> Colors = new List<ColorComponent>()
        {
            ColorComponent.RED,
            ColorComponent.GREEN,
            ColorComponent.BLUE,
        };

        public bool IsInteger { get; }
        public List<AvailableColorRecord> AvailableColors { get; }
        public int TimeLimitSeconds { get; init; }
        /// <summary>
        /// Из всего пула берутся только SampleTaker рандомное число красок
        /// </summary>
        public int SampleSize { get; init; }
        public ColorBrewer(List<AvailableColorRecord> availableColors, bool isInteger)
        {
            AvailableColors = availableColors;
            IsInteger = isInteger;
            TimeLimitSeconds = 5;
            SampleSize = int.MaxValue;
        }

        public List<AvailableColorRecord> Brew(Color targetColor, int amount)
        {
            if (amount < 0)
            {
                throw new Exception("You wut m8?");
            }

            //AvailableColors.Shuffle(new Random());

            var selectedColors = AvailableColors
                .OrderBy(c => ColorUtils.ColorDiffL1(c.Color, targetColor))
                .Take(SampleSize)
                .ToList();

            var ret = new List<AvailableColorRecord>();

            Solver solver = Solver.CreateSolver("SCIP");

            solver.SetTimeLimit(TimeLimitSeconds * 1000);

            var colorWeights = new List<AvailableColorWeightDVar>(selectedColors.Count);

            foreach (var availableColor in selectedColors)
            {
                colorWeights.Add(new AvailableColorWeightDVar(solver, availableColor, isInteger: IsInteger));
            }

            /// Суммарный вес должен быть равен заданному весу
            var weightConstraint = solver.MakeConstraint(lb: amount, ub: amount, $"Total amount constraint");

            foreach (var colorWeight in colorWeights)
            {
                weightConstraint.SetCoefficient(colorWeight.DVar, 1.0d);
            }

            /// Переменные отличия слева и справа от целевого значения
            var colorDiffDVars = new List<ColorDiffDVar>();

            Dictionary<ColorComponent, int> targetComponents = new()
            {
                { ColorComponent.RED, targetColor.R },
                { ColorComponent.GREEN, targetColor.G },
                { ColorComponent.BLUE, targetColor.B },
            };

            foreach (var color in Colors)
            {
                var dvar = new ColorDiffDVar(solver, color, amount, isInteger: IsInteger);

                colorDiffDVars.Add(dvar);

                var leftConstraint = solver.MakeConstraint(
                    lb: amount * targetComponents[color], 
                    ub: double.PositiveInfinity, 
                    $"Left {color} constraint");

                leftConstraint.SetCoefficient(dvar.LeftDVar, amount);

                foreach (var colorWeight in colorWeights)
                {
                    leftConstraint.SetCoefficient(colorWeight.DVar, colorWeight.ColorComponents[color]);
                }

                var rightConstraint = solver.MakeConstraint(
                    lb: double.NegativeInfinity,
                    ub: amount * targetComponents[color],
                    $"Right {color} constraint");

                rightConstraint.SetCoefficient(dvar.RightDVar, -amount);

                foreach (var colorWeight in colorWeights)
                {
                    rightConstraint.SetCoefficient(colorWeight.DVar, colorWeight.ColorComponents[color]);
                }
            }

            /// Целевая функция

            var objective = solver.Objective();

            foreach (var colorDiffDVar in colorDiffDVars)
            {
                objective.SetCoefficient(colorDiffDVar.LeftDVar, 1.0d);
                objective.SetCoefficient(colorDiffDVar.RightDVar, 1.0d);
            }

            objective.SetMinimization();

            var status = solver.Solve();

            if (status != Solver.ResultStatus.OPTIMAL
                && status != Solver.ResultStatus.FEASIBLE)
            {
                return new List<AvailableColorRecord>();
            }

            //Console.WriteLine($"Objective value = {objective.Value()}");

            foreach (var colorWeight in colorWeights)
            {
                colorWeight.Extract();
            }

            foreach (var dvar in colorDiffDVars)
            {
                dvar.Extract();
            }

            ret = colorWeights
                .Where(w => w.Value > 0)
                .Select(w => new AvailableColorRecord(w.AvailableColorRecord.ColorCode, (int)Math.Round(w.Value)))
                .ToList();

            CheckBrew(ret, targetColor);

            return ret;
        }

        private void CheckBrew(List<AvailableColorRecord> ret, Color targetColor)
        {
            var r = (int)Math.Round((double)ret.Sum(e => e.R * e.Amount) / ret.Sum(e => e.Amount));
            var g = (int)Math.Round((double)ret.Sum(e => e.G * e.Amount) / ret.Sum(e => e.Amount));
            var b = (int)Math.Round((double)ret.Sum(e => e.B * e.Amount) / ret.Sum(e => e.Amount));

            var brewedRGB = Color.FromArgb(r, g, b);

            //var l1Diff = Math.Abs(r - targetColor.R)
            //   + Math.Abs(g - targetColor.G)
            //   + Math.Abs(b - targetColor.B);

            var l1Diff = ColorUtils.ColorDiffL1(brewedRGB, targetColor);

            Console.WriteLine($"\tFor target {targetColor} brewed {brewedRGB}), L1 diff is {l1Diff}");
        }
    }
}
