using AngkorWat.IO;
using AngkorWat.Tower;
using Newtonsoft.Json;
using System.Net;

internal class Program
{
    private static void Main(string[] _)
    {
        Console.WriteLine("Hello, World!");

        var dict = ReadDictionary();

        var towerMaker = new TowerMaker(dict);

        var tower = towerMaker.MakeTower();

        var lengths = dict
            .GroupBy(g => g.Length)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());

        var words23 = dict
            .Where(e => e.Length >= 23)
            .ToDictionary(e => e, e => e.Length);


        //var outputContainer = new OutputContainer()
        //{
        //    Punkte = new List<DerPunkt>()
        //    {
        //        new DerPunkt() { X = 1, Y = 2, Z = 3, },
        //        new DerPunkt() { X = 4, Y = 1, Z = 2, },
        //        new DerPunkt() { X = 2, Y = 0, Z = 1, },
        //    }
        //};

        //Serialize(outputContainer);

        //outputContainer = Deserialize("output.json");

        //foreach (var punkt in outputContainer.Punkte)
        //{
        //    Console.WriteLine($"Der punkt: {punkt.X}, {punkt.Y}, {punkt.Z}");
        //}
    }

    private static List<string> ReadDictionary()
    {
        string json = File.ReadAllText("../../../data.json");

        var container = JsonConvert.DeserializeObject<InputContainer>(json);

        if (container is null)
        {
            throw new FileNotFoundException();
        }

        return container.Input.Keys.ToList();
    }

    public static void Serialize(OutputContainer outputContainer)
    {
        string json = JsonConvert.SerializeObject(outputContainer);

        File.WriteAllText("output.json", json);

        Console.WriteLine("Collections are serialized");
    }

    public static OutputContainer Deserialize(string filepath)
    {
        string json = File.ReadAllText(filepath);

        var container = JsonConvert.DeserializeObject<OutputContainer>(json);

        if (container is null)
        {
            throw new FileNotFoundException();
        }

        return container;
    }
}