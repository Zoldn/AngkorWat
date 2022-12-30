using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.RouteSolver;
using AngkorWat.Components;
using AngkorWat.IO;
using Newtonsoft.Json;
using System.Net;
using AngkorWat.Constants;
using AngkorWat.Phases;

internal class Program
{
    private static void Main(string[] _)
    {
        //Phase1.Phase1MultiStart();

        //Phase2.Solve();

        //Phase2.SolveMIP();

        Phase3.Solve();
    }
}