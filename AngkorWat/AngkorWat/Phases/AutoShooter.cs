using AngkorWat.Algorithms;
using AngkorWat.Algorithms.CBrewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal class AutoShooter
    {
        public int X0 { get; init; }
        public int Y0 { get; init; }
        public int X1 { get; init; }
        public int Y1 { get; init; }
        public int Shots { get; private set; }
        public double DistanceBetweenShots { get; init; }
        public int MinAmount { get; init; }
        public int MaxAmount { get; init; }
        public Color TargetColor { get; init; }
        public int CanvasWidth { get; }
        public AutoShooter(int canvasWidth)
        {
            CanvasWidth = canvasWidth;
        }

        public async Task Run()
        {
            var d1 = GeometryUtils.GetDistance(X0, Y0, X1, Y1);

            Shots = (int)Math.Ceiling(d1 / DistanceBetweenShots);

            for (int i = 0; i < Shots; i++)
            {
                int x = (int)Math.Round(X0 + (X1 - X0) * (double)i / (Shots - 1));
                int y = (int)Math.Round(Y0 + (Y1 - Y0) * (double)i / (Shots - 1));

                var shoota = new Shoota();

                int tryNumber = 0;

                while (true)
                {
                    Console.WriteLine($"Doing iteration {i}/{Shots}, try = {++tryNumber}");

                    Console.WriteLine("\tFetching colors from storage");

                    var availableColors = await shoota.GetAllAvailableColors();

                    Console.WriteLine("\tBrewing colors");

                    var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
                    {
                        TimeLimitSeconds = 5,
                        SampleSize = 5000,
                    };

                    //var brew = colorBrewer.Brew(targetColor, amount);
                    var brew = colorBrewer.BrewExact(TargetColor, MinAmount, MaxAmount);

                    if (brew is null)
                    {
                        Console.WriteLine("Can't do exact color. Retry");
                        continue;
                    }

                    int amount = brew.Sum(e => e.Amount);

                    if (amount == 0)
                    {
                        Console.WriteLine("Failed to fill bomb. Retry");
                        continue;
                    }

                    Console.WriteLine("\tTargeting shot");

                    var bestShot = Shoota.InitializeShot(x: x, y: y, canvasWidth: CanvasWidth, mass: amount);

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

                    break;
                }
            }
        }
    }
}
