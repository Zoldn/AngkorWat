using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Components;
using AngkorWat.IO;
using Newtonsoft.Json;
using System.Net;

internal class Program
{
    private static void Main(string[] _)
    {
        var allData = GetAllData();

        var packingSolver = new PackingSolver(allData);

        allData.Packings = packingSolver.Solve();
    }

    private static AllData GetAllData()
    {
        var inputContainer = ReadInputData();

        var ret = new AllData();

        ret.Children = inputContainer.children
            .Select(e => new Child(e))
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
        string json = File.ReadAllText("../../../santa.json");

        var container = JsonConvert.DeserializeObject<InputContainer>(json);

        if (container == null)
        {
            throw new FileLoadException();
        }

        return container;
    }
}