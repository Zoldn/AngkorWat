using AngkorWat.IO;
using AngkorWat.Tower;
using AngkorWat.TowerBuilder;
using Newtonsoft.Json;
using System.Net;

internal class Program
{
    private static void Main(string[] _)
    {
        //Console.WriteLine("Hello, World!");

        var dict = ReadDictionary();

        //var towerMaker = new TowerMaker(dict);

        //var tower = towerMaker.MakeTower();

        var towerBuilder = new TowerBuilder(dict);

        var tower = towerBuilder.SearchTower();

        tower.Serialize();
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
}