using AngkorWat.IO;
using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] _)
    {
        Console.WriteLine("Hello, World!");

        var outputContainer = new OutputContainer()
        {
            Punkte = new List<DerPunkt>()
            {
                new DerPunkt() { X = 1, Y = 2, Z = 3, },
                new DerPunkt() { X = 4, Y = 1, Z = 2, },
                new DerPunkt() { X = 2, Y = 0, Z = 1, },
            }
        };

        Serialize(outputContainer);

        outputContainer = Deserialize("output.json");

        foreach (var punkt in outputContainer.Punkte)
        {
            Console.WriteLine($"Der punkt: {punkt.X}, {punkt.Y}, {punkt.Z}");
        }
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