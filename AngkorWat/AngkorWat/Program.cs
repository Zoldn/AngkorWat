﻿using AngkorWat.Algorithms.DistSolver;
using AngkorWat.Algorithms.PackSolver;
using AngkorWat.Algorithms.RouteSolver;
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

        allData.PackingSolution = packingSolver.Solve();

        //DistanceSolver.TestOverlap();

        var distanceSolver = new DistanceSolver(allData);

        allData.Routes = distanceSolver.Solve();

        var tspSolver = new TSPSolver(allData);

        allData.Sequences = tspSolver.Solve();

        var output = new OutputContainer(mapId: "faf7ef78-41b3-4a36-8423-688a61929c08", allData);

        SerializeResult(output);
    }

    private static void SerializeResult(OutputContainer output)
    {
        var json = JsonConvert.SerializeObject(output);

        File.WriteAllText("../../../../result.json", json);
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
        string json = File.ReadAllText("../../../../santa.json");

        var container = JsonConvert.DeserializeObject<InputContainer>(json);

        if (container == null)
        {
            throw new FileLoadException();
        }

        return container;
    }
}