using AngkorWat.Components;
using Newtonsoft.Json;
using System.Net;
using AngkorWat.Phases;
using System;
using System.Text;
using AngkorWat.IO.HTTP;
using AngkorWat.Algorithms.CBrewer;
using System.Drawing;

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

        //await ColorRequester();

        //await DoShoota();

        //await ShootaInterface();

        //await ShootaInterfaceStable();

        //await AutoShootaInterfaceStable();

        var imageLoader = new TargetImageLoader(@"C:\Users\User\Desktop\Хакатон\19.png");

        imageLoader.Run();

        //await FindGoodColor();
    }

    private static async Task FindGoodColor()
    {
        var shoota = new Shoota();

        var availableColors = await shoota.GetAllAvailableColors();

        var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
        {
            TimeLimitSeconds = 15,
            SampleSize = 5000,
        };

        Color targetColor = Color.FromArgb(193, 224, 224);

        //Color targetColor = Color.FromArgb(r, g, b);

        var brew = colorBrewer.Brew(targetColor, 5000);

        //Color possibleColor = Color.FromArgb(198, 194, 64);

        //var ret = colorBrewer.BrewExact(possibleColor, 100, 300);

        Console.WriteLine();
    }

    private static async Task DoShoota()
    {
        var shoota = new Shoota();

        var availableColors = await shoota.GetAllAvailableColors();

        var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
        {
            TimeLimitSeconds = 5,
            SampleSize = 2000,
        };

        Color targetColor = Color.FromArgb(252, 194, 27);

        var brew = colorBrewer.Brew(targetColor, 10);

        //var colorCode = await shoota.TakeRandomAvailableColor(minValue: 10);

        //if (colorCode is null)
        //{
        //    return;
        //}

        var bestShot = Shoota.InitializeShot(x: 56, y: 67, canvasWidth: 250, mass: 10);

        if (bestShot is null)
        {
            Console.WriteLine("Shot is unstable or impossible");
            return;
        }

        //bestShot.ColorCodes.Add(colorCode.Value, 10);

        foreach (var item in brew)
        {
            bestShot.ColorCodes.Add(item.ColorCode, item.Amount);
        }

        //var shot = new Shot()
        //{
        //    Power = 26,
        //    HAngle = 0,
        //    VAngle = 45,
        //};

        //shot.ColorCodes.Add(colorCode.Value, 10);

        //var color = ColorHelper.CodeToRGB(colorCode.Value);

        await shoota.TestShooting(bestShot);

        Console.WriteLine($"Shot with color {targetColor} and power = {bestShot.Power}");
    }

    private static async Task ShootaInterface()
    {
        while (true)
        {
            Console.WriteLine("Boss! Shoota iz redy!");
            Console.WriteLine("\tInsert parameters: CanvasWidth, Red, Green, Blue, Amount, X, Y");
            var line = Console.ReadLine();

            if (line == null)
            {
                Console.WriteLine($"Failed input. Retry");
                continue;
            }

            var args = line.Split(" ").ToList();

            if (args.Count != 7)
            {
                Console.WriteLine($"Wrong number of arguments {args.Count} need 7. Retry"); 
                continue;
            }

            int width;
            int r;
            int g;
            int b;
            int amount;
            int x;
            int y;

            try
            {
                width = int.Parse(args[0]);
                r = int.Parse(args[1]);
                g = int.Parse(args[2]);
                b = int.Parse(args[3]);
                amount = int.Parse(args[4]);
                x = int.Parse(args[5]);
                y = int.Parse(args[6]);

                if (r < 0 || r > 255 || b < 0 || b > 255 || g < 0 || g > 255 || amount <= 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to parse values. Retry");
                continue;
            }

            var shoota = new Shoota();

            Console.WriteLine("\tTargeting shot");

            var bestShot = Shoota.InitializeShot(x: x, y: y, canvasWidth: width, mass: amount);

            if (bestShot is null)
            {
                Console.WriteLine("Shot is unstable or impossible. Retry");
                continue;
            }

            Console.WriteLine("\tFetching colors from storage");

            var availableColors = await shoota.GetAllAvailableColors();

            Console.WriteLine("\tBrewing colors");

            var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
            {
                TimeLimitSeconds = 5,
                SampleSize = 2000,
            };

            Color targetColor = Color.FromArgb(r, g, b);

            var brew = colorBrewer.Brew(targetColor, amount);

            Console.WriteLine("\tCharging shoota");

            foreach (var item in brew)
            {
                bestShot.ColorCodes.Add(item.ColorCode, item.Amount);
            }

            Console.WriteLine("\tShoot!");

            await shoota.TestShooting(bestShot);

            Console.WriteLine("\tSuccessful shot!");

            await Task.Delay(100);
        }
    }

    public static async Task AutoShootaInterfaceStable()
    {
        while (true)
        {
            Console.WriteLine("Boss! Shoota iz redy!");
            Console.WriteLine("\tInsert parameters: CanvasWidth, Red, Green, Blue, MinAmount, MaxAmount, X0, Y0," +
                "X1, Y1, DistanceBetweenShots");
            var line = Console.ReadLine();

            if (line == null)
            {
                Console.WriteLine($"Failed input. Retry");
                continue;
            }

            var args = line.Split(" ").ToList();

            if (args.Count != 11)
            {
                Console.WriteLine($"Wrong number of arguments {args.Count} need 11. Retry");
                continue;
            }

            int width;
            int r;
            int g;
            int b;
            int minAmount;
            int maxAmount;
            int x0;
            int y0;
            int x1;
            int y1;
            int distance;

            try
            {
                width = int.Parse(args[0]);
                r = int.Parse(args[1]);
                g = int.Parse(args[2]);
                b = int.Parse(args[3]);
                minAmount = int.Parse(args[4]);
                maxAmount = int.Parse(args[5]);
                x0 = int.Parse(args[6]);
                y0 = int.Parse(args[7]);
                x1 = int.Parse(args[8]);
                y1 = int.Parse(args[9]);
                distance = int.Parse(args[10]);

                if (r < 0 || r > 255 || b < 0 || b > 255 || g < 0 || g > 255 || minAmount <= 0 || maxAmount <= 0
                    || distance < 1)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to parse values. Retry");
                continue;
            }

            Color targetColor = Color.FromArgb(r, g, b);

            var autoShoota = new AutoShooter(width)
            {
                X0 = x0,
                Y0 = y0,
                X1 = x1,
                Y1 = y1,
                DistanceBetweenShots = distance,
                MinAmount = minAmount,
                MaxAmount = maxAmount,
                TargetColor = targetColor,
            };

            await autoShoota.Run();

            //var shoota = new Shoota();

            //Console.WriteLine("\tFetching colors from storage");

            //var availableColors = await shoota.GetAllAvailableColors();

            //Console.WriteLine("\tBrewing colors");

            //var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
            //{
            //    TimeLimitSeconds = 5,
            //    SampleSize = 5000,
            //};



            ////var brew = colorBrewer.Brew(targetColor, amount);
            //var brew = colorBrewer.BrewExact(targetColor, minAmount, maxAmount);

            //if (brew is null)
            //{
            //    Console.WriteLine("Can't do exact color. Retry");
            //    continue;
            //}

            //int amount = brew.Sum(e => e.Amount);

            //Console.WriteLine("\tTargeting shot");

            //var bestShot = Shoota.InitializeShot(x: x, y: y, canvasWidth: width, mass: amount);

            //if (bestShot is null)
            //{
            //    Console.WriteLine("Shot is unstable or impossible. Retry");
            //    continue;
            //}

            //Console.WriteLine("\tCharging shoota");

            //foreach (var item in brew)
            //{
            //    bestShot.ColorCodes.Add(item.ColorCode, item.Amount);
            //}

            //Console.WriteLine("\tShoot!");

            //await shoota.TestShooting(bestShot);

            //Console.WriteLine("\tSuccessful shot!");

            Console.WriteLine("\tSuccessful auto shot!");

            await Task.Delay(100);
        }
    }

    private static async Task ShootaInterfaceStable()
    {
        while (true)
        {
            Console.WriteLine("Boss! Shoota iz redy!");
            Console.WriteLine("\tInsert parameters: CanvasWidth, Red, Green, Blue, MinAmount, MaxAmount, X, Y");
            var line = Console.ReadLine();

            if (line == null)
            {
                Console.WriteLine($"Failed input. Retry");
                continue;
            }

            var args = line.Split(" ").ToList();

            if (args.Count != 8)
            {
                Console.WriteLine($"Wrong number of arguments {args.Count} need 8. Retry");
                continue;
            }

            int width;
            int r;
            int g;
            int b;
            int minAmount;
            int maxAmount;
            int x;
            int y;

            try
            {
                width = int.Parse(args[0]);
                r = int.Parse(args[1]);
                g = int.Parse(args[2]);
                b = int.Parse(args[3]);
                minAmount = int.Parse(args[4]);
                maxAmount = int.Parse(args[5]);
                x = int.Parse(args[6]);
                y = int.Parse(args[7]);

                if (r < 0 || r > 255 || b < 0 || b > 255 || g < 0 || g > 255 || minAmount <= 0 || maxAmount <= 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to parse values. Retry");
                continue;
            }

            var shoota = new Shoota();

            Console.WriteLine("\tFetching colors from storage");

            var availableColors = await shoota.GetAllAvailableColors();

            Console.WriteLine("\tBrewing colors");

            var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
            {
                TimeLimitSeconds = 5,
                SampleSize = 5000,
            };

            Color targetColor = Color.FromArgb(r, g, b);

            //var brew = colorBrewer.Brew(targetColor, amount);
            var brew = colorBrewer.BrewExact(targetColor, minAmount, maxAmount);

            if (brew is null)
            {
                Console.WriteLine("Can't do exact color. Retry");
                continue;
            }

            int amount = brew.Sum(e => e.Amount);

            Console.WriteLine("\tTargeting shot");

            var bestShot = Shoota.InitializeShot(x: x, y: y, canvasWidth: width, mass: amount);

            if (bestShot is null)
            {
                Console.WriteLine("Shot is unstable or impossible. Retry");
                continue;
            }

            Console.WriteLine("\tCharging shoota");

            foreach (var item in brew)
            {
                bestShot.ColorCodes.Add(item.ColorCode, item.Amount);
            }

            Console.WriteLine("\tShoot!");

            await shoota.TestShooting(bestShot);

            Console.WriteLine("\tSuccessful shot!");

            await Task.Delay(100);
        }
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