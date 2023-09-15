using AngkorWat.Algorithms.CBrewer;
using AngkorWat.Algorithms;
using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal class OmniShooter
    {
        public string FileName { get; set; }
        public int ChunkSize { get; init; }
        public int CanvasWidth { get; init; }
        public int Skip { get; init; }

        public OmniShooter(string fileName, int canvasWidth)
        {
            FileName = fileName;
            ChunkSize = 16;
            CanvasWidth = canvasWidth;
            Skip = 0;
        }

        public async Task Run()
        {
            var imageLoader = new TargetImageLoader(FileName)
            {
                ChunkSize = ChunkSize,
            };

            var chunks = imageLoader.Run();

            chunks = chunks
                .Where(e => e.IsShooting)
                .Skip(Skip)
                .ToList();

            int chunkIndex = 0;

            int amount = (int)Math.Round(Math.PI * (ChunkSize + 5) * (ChunkSize + 5) / 4.0d);

            if (amount == 0)
            {
                Console.WriteLine($"Amount == 0");
                return;
            }

            foreach (var chunk in chunks)
            {
                var shoota = new Shoota();
                int tryNumber = 0;

                chunkIndex++;

                while (true)
                {
                    Console.WriteLine($"Doing iteration {chunkIndex}/{chunks.Count}, try = {++tryNumber}");

                    if (tryNumber > 5)
                    {
                        continue;
                    }

                    Console.WriteLine("\tFetching colors from storage");

                    var availableColors = await shoota.GetAllAvailableColors();

                    Console.WriteLine("\tBrewing colors");

                    var colorBrewer = new ColorBrewer(availableColors, isInteger: true)
                    {
                        TimeLimitSeconds = 2,
                        SampleSize = 3000,
                    };
                    //var brew = colorBrewer.Brew(targetColor, amount);
                   // var brew = colorBrewer.BrewExact(TargetColor, MinAmount, MaxAmount);
                    var brew = colorBrewer.Brew(chunk.Color, amount);

                    if (brew is null)
                    {
                        Console.WriteLine("Can't do exact color. Retry");
                        continue;
                    }

                    //int amount = brew.Sum(e => e.Amount);

                    //if (amount == 0)
                    //{
                    //    Console.WriteLine("Failed to fill bomb. Retry");
                    //    continue;
                    //}

                    Console.WriteLine("\tTargeting shot");

                    var bestShot = Shoota.InitializeShot(x: chunk.X, y: chunk.Y, canvasWidth: CanvasWidth, mass: amount);

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
