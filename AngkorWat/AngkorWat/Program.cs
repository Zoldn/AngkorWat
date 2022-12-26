using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.RouteSolver;
using AngkorWat.Components;
using AngkorWat.IO;
using Newtonsoft.Json;
using System.Net;
using AngkorWat.Constants;

internal class Program
{
    private static void Main(string[] _)
    {
        var allData = GetAllData();

        var deterministicSolution = new FullSolution();

        var distanceSolver = new DistanceSolver(allData);

        deterministicSolution.Routes = distanceSolver.Solve();

        var solutionVariants = new List<FullSolution>();

        for (int i = 0; i < 10; i++)
        {
            var curSolution = new FullSolution(deterministicSolution);

            solutionVariants.Add(curSolution);

            var packingSolver = new PackingSolver(allData);

            curSolution.PackingSolution = packingSolver.Solve();

            var tspSolver = new TSPSolver(allData, curSolution);

            curSolution.Sequences = tspSolver.Solve();
        }

        solutionVariants = solutionVariants
            .OrderBy(v => v.Sequences.TravelTime)
            .ToList();

        var times = solutionVariants
            .Select(v => v.Sequences.TravelTime)
            .ToList();

        foreach (var solutionVariant in solutionVariants)
        {
            var output = new OutputContainer(mapId: "faf7ef78-41b3-4a36-8423-688a61929c08",
                allData, solutionVariant);

            SerializeResult(output);
        }
    }

    private static void SerializeResult(OutputContainer output)
    {
        var json = JsonConvert.SerializeObject(output);

        string path = Path.Combine(AngkorConstants.FilesRoute, "result.json");

        File.WriteAllText(path, json);
    }

    private static AllData GetAllData()
    {
        var inputContainer = ReadInputData();

        var ret = new AllData();

        ret.Children = inputContainer.children
            .Select((e, index) => new Child(e, index + 1))
            .ToList();

        ret.SnowAreas = inputContainer.snowAreas
            .Select(e => new SnowArea(e))
            .ToList();

        ret.Gifts = inputContainer.gifts
            .Select(e => new Gift(e))
            .ToList();

        return ret;
    }

    private static InputContainer ReadInputData()
    {
        string path = Path.Combine(AngkorConstants.FilesRoute, "santa.json");

        string json = File.ReadAllText(path);

        var container = JsonConvert.DeserializeObject<InputContainer>(json);

        if (container == null)
        {
            throw new FileLoadException();
        }

        return container;
    }
}