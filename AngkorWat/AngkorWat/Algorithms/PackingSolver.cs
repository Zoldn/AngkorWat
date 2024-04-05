using Google.OrTools.Sat;
using OperationsResearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms
{
    internal class GarbageItem
    {
        public string Name { get; init; } = string.Empty;
        public List<(int X, int Y)> Form { get; set; } = new();
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsTaken { get; set; }
        public GarbageItem() 
        { 
        }

        public GarbageItem(string name, List<(int X, int Y)> form)
        {
            Name = name;
            Form = form
                .Select(e => (e.X, e.Y))
                .ToList();
        }

        public GarbageItem Rotate() 
        {
            return new GarbageItem() { Name = Name };
        }
    }

    internal class PackingSolution
    {
        public bool IsOk { get; set; }
        public List<GarbageItem> GarbageItems { get; set; } = new();
        public PackingSolution() { }
    }

    internal class GarbageItemDVar
    {
        public GarbageItem Item { get; init; }
        public IntVar XDVar { get; set; }
        public IntVar YDVar { get; set; }
        public BoolVar IsPresentDVar { get; set; }
        public BoolVar[, ] BoolVars { get; set; }
        public bool IsPresentResult { get; set; }
        public int XResult { get; set; }
        public int YResult { get; set; }
        public GarbageItemDVar(GarbageItem item, CpModel model, int sizeX, int sizeY) 
        {
            Item = item;

            int ySize = item.Form.Max(e => e.Y);
            int xSize = item.Form.Max(e => e.X);

            XDVar = model.NewIntVar(0, sizeX - xSize - 1, $"{item.Name} X DVar");
            YDVar = model.NewIntVar(0, sizeY - ySize - 1, $"{item.Name} Y DVar");
            IsPresentDVar = model.NewBoolVar($"{item.Name} is present");

            BoolVars = new BoolVar[sizeY, sizeX];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    BoolVars[y, x] = model.NewBoolVar("");
                }
            }
        }

        public void Extract(CpSolver solver)
        {
            IsPresentResult = solver.BooleanValue(IsPresentDVar);
            XResult = (int)solver.Value(XDVar);
            YResult = (int)solver.Value(YDVar);
        }
    }

    internal class PackingSolver
    {
        private List<GarbageItemDVar> GarbageItemDVars { get; set; } = new List<GarbageItemDVar>();
        public PackingSolver()
        {

        }

        public PackingSolution Solve(int sizeX, int sizeY, List<GarbageItem> garbageItems,
            int minLimit = 0)
        {
            CpModel model = new CpModel();

            foreach (var item in garbageItems)
            {
                var dvar = new GarbageItemDVar(item, model, sizeX, sizeY);
                GarbageItemDVars.Add(dvar);

                AddFormConstraint(model, dvar, sizeX, sizeY);
            }

            AddNoOverlapConstraint(model, sizeX, sizeY);

            AddMinLimitConstraint(model, minLimit);

            AddObjective(model);

            CpSolver solver = new CpSolver();
            solver.StringParameters = "max_time_in_seconds:4.0";

            CpSolverStatus status = solver.Solve(model);

            if (status == CpSolverStatus.Feasible
                || status == CpSolverStatus.Optimal)
            {
                var solution = ExtractSolution(solver);
                PrintResult(sizeX, sizeY);

                return solution;
            }
            else
            {
                Console.WriteLine("Failed to solve :(");

                return new PackingSolution() { IsOk = false, };
            }
        }

        private void AddMinLimitConstraint(CpModel model, int minLimit)
        {
            model.Add(LinearExpr.WeightedSum(
                GarbageItemDVars.Select(e => e.IsPresentDVar), 
                GarbageItemDVars.Select(e => e.Item.Form.Count)) >= minLimit);
        }

        private void PrintResult(int sizeX, int sizeY)
        {
            char[,] result = new char[sizeY, sizeX];

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    result[y, x] = ' ';
                }
            }

            char letter = 'A';

            foreach (var dvar in GarbageItemDVars)
            {
                if (!dvar.IsPresentResult)
                {
                    continue;
                }

                foreach (var (formX, formY) in dvar.Item.Form)
                {
                    if (result[dvar.YResult + formY, dvar.XResult + formX] != ' ')
                    {
                        Console.WriteLine("Overlap!");
                    }

                    result[dvar.YResult + formY, dvar.XResult + formX] = letter;
                }

                letter = (char)(letter + 1);
            }

            Console.WriteLine("Your result is:");

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Console.Write(result[y, x]);
                }

                Console.WriteLine();
            }
        }

        private PackingSolution ExtractSolution(CpSolver solver)
        {
            var itemCount = solver.ObjectiveValue;

            Console.WriteLine($"Result is {itemCount} item(s)");

            foreach (var dvar in GarbageItemDVars)
            {
                dvar.Extract(solver);
            }

            var packingSolution = new PackingSolution()
            {
                IsOk = true,
            };

            foreach (var dvar in GarbageItemDVars)
            {
                dvar.Item.X = dvar.XResult;
                dvar.Item.Y = dvar.YResult;
                dvar.Item.IsTaken = dvar.IsPresentResult;

                packingSolution.GarbageItems.Add(dvar.Item);
            }

            return packingSolution;
        }

        private void AddNoOverlapConstraint(CpModel model, int sizeX, int sizeY)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    model.AddAtMostOne(GarbageItemDVars.Select(e => e.BoolVars[y, x]));
                }
            }
        }

        private void AddObjective(CpModel model)
        {
            model.Maximize(LinearExpr.Sum(GarbageItemDVars.Select(dvar => dvar.IsPresentDVar)));
        }

        private void AddFormConstraint(CpModel model, GarbageItemDVar dvar, int sizeX, int sizeY)
        {
            int ySize = dvar.Item.Form.Max(e => e.Y);
            int xSize = dvar.Item.Form.Max(e => e.X);

            for (int y = 0; y < sizeY - ySize; y++)
            {
                for (int x = 0; x < sizeX - xSize; x++)
                {
                    var boolVarX1 = model.NewBoolVar("");

                    model.Add(dvar.XDVar == x).OnlyEnforceIf(boolVarX1);
                    model.Add(dvar.XDVar != x).OnlyEnforceIf(boolVarX1.Not());

                    var boolVarY1 = model.NewBoolVar("");

                    model.Add(dvar.YDVar == y).OnlyEnforceIf(boolVarY1);
                    model.Add(dvar.YDVar != y).OnlyEnforceIf(boolVarY1.Not());

                    foreach (var (shapeX, shapeY) in dvar.Item.Form)
                    {
                        var andResult = model.And(boolVarX1, boolVarY1, dvar.IsPresentDVar);

                        model.AddImplication(andResult, dvar.BoolVars[y + shapeY, x + shapeX]);
                    }
                }
            }
        }
    }


    public static class CPSATExprToLiteralExtensions
    {
        public static ILiteral And(this CpModel model, List<ILiteral> literals, string name = "And bool var")
        {
            var andExpr = LinearExpr.Sum(literals);

            var b = model.NewBoolVar(name);

            model.Add(andExpr == literals.Count).OnlyEnforceIf(b);

            model.Add(andExpr < literals.Count).OnlyEnforceIf(b.Not());

            return b;
        }

        public static ILiteral And(this CpModel model, ILiteral literal1, ILiteral literal2, string name = "And bool var")
        {
            var literals = new List<ILiteral>() { literal1, literal2 };

            return model.And(literals, name);
        }

        public static ILiteral And(this CpModel model,
            ILiteral literal1, ILiteral literal2, ILiteral literal3, string name = "And bool var")
        {
            var literals = new List<ILiteral>() { literal1, literal2, literal3 };

            return model.And(literals, name);
        }

        public static ILiteral Or(this CpModel model, List<ILiteral> literals, string name = "Or bool var")
        {
            var orExpr = LinearExpr.Sum(literals);

            var b = model.NewBoolVar(name);

            model.Add(orExpr > 0).OnlyEnforceIf(b);

            model.Add(orExpr == 0).OnlyEnforceIf(b.Not());

            //var b = model.NewBoolVar(name);

            //model.AddBoolOr(literals).OnlyEnforceIf(b);

            //model.AddBoolAnd(Inverse(literals)).OnlyEnforceIf(b.Not());

            return b;
        }
    }
}
