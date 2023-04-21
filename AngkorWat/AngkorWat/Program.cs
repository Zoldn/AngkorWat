using AngkorWat.Components;
using Newtonsoft.Json;
using System.Net;
using AngkorWat.Phases;
using System;
using System.Text;
using AngkorWat.IO.HTTP;

internal class Program
{
    public class User
    {
        public string Name { get; set; }
        public int Occupation { get; set; }
        public User()
        {
            Name = string.Empty;
        }
    }

    private static async Task Main(string[] _)
    {
        HttpHelper.SetApiKey("643ec1df4dabc643ec1df4dac0");


        await DoShoota();
    }

    private static async Task DoShoota()
    {
        var shoota = new Shoota();

        var colorCode = await shoota.TakeRandomAvailableColor(minValue: 10);

        if (colorCode is null)
        {
            return;
        }

        var bestShot = Shoota.InitializeShot(x: 56, y: 67, canvasWidth: 250, mass: 10);
        bestShot.ColorCodes.Add(colorCode.Value, 10);
        //var shot = new Shot()
        //{
        //    Power = 26,
        //    HAngle = 0,
        //    VAngle = 45,
        //};

        //shot.ColorCodes.Add(colorCode.Value, 10);

        var color = ColorHelper.CodeToRGB(colorCode.Value);

        await shoota.TestShooting(bestShot);

        Console.WriteLine($"Shot with color {color} and power = {bestShot.Power}");
    }

    private static async Task ColorRequester()
    {
        var requester = new ColorRequester();

        await requester.Run();
    }

    /// <summary>
    /// 
    /// </summary>
    private static async void Test()
    {
        var ddoser = new DDoser<User, string>("https://httpbin.org/post", 3);

        for (int i = 0; i < 3; i++)
        {
            var user = new User()
            {
                Name = "John Doe",
                Occupation = 100 * i,
            };

            await ddoser.RunStep(user);
        }
    }
}