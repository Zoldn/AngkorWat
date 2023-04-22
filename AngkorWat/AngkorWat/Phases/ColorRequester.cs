using AngkorWat.Algorithms;
using AngkorWat.IO.HTTP;
using AngkorWat.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal class ColorItem 
    {
        public int amount { get; set; }
        public int color { get; set; }
    }

    internal class FactoryInfo
    {
        public long tick { get; set; }
    }

    internal class FactoryResponse
    {
        public bool success { get; set; }
        public int status { get; set; }
        public FactoryInfo info { get; set; }
        public Dictionary<string, ColorItem> response { get; set; }
        public FactoryResponse()
        {
            response = new();
            info = new();
        }
    }

    internal class BasicResponse 
    {
        public bool success { get; set; }
        public int status { get; set; }
        public FactoryInfo info { get; set; }
        public BasicResponse()
        {
            info = new();
        }
    }


    internal class ColorRequester
    {
        public string UrlGenerate { get; }
        public string UrlPick { get; }
        public int DelayMilliseconds { get; }
        public int MaxIteration { get; }
        public ColorRequester()
        {
            UrlGenerate = "http://api.datsart.dats.team/art/factory/generate";

            UrlPick = "http://api.datsart.dats.team/art/factory/pick";

            DelayMilliseconds = 100;

            MaxIteration = 10000000;
        }

        public async Task Run() 
        {
            var random = new Random();

            int iteration = 0;

            Color gray = Color.Gray;

            while (true)
            {
                ++iteration;

                if (iteration > MaxIteration)
                {
                    break;
                }

                var factoryResponse = await DoIteration();

                if (factoryResponse is null || !factoryResponse.success)
                {
                    Console.WriteLine("Failed on generate. Restart in 1 sec...");

                    await Task.Delay(DelayMilliseconds);

                    continue;
                }

                //var selected = (random.Next(3) + 1);
                //var selected = factoryResponse.response.ArgMax(kv => kv.Value.amount).Key;

                var ttt = factoryResponse.response
                    .Select(
                        kv => new
                        {
                            Pot = kv.Key,
                            Color = Color.FromArgb(
                                kv.Value.color / 256 / 256,
                                kv.Value.color / 256 % 256,
                                kv.Value.color % 256),
                        }
                    )
                    .ToArray();

                var selected = ttt
                    .ArgMax(kv => ColorUtils.ColorDiffL0(Color.Gray, kv.Color))
                    .Pot;

                var sendToTake = new Dictionary<string, string>()
                {
                    { "num", selected.ToString() },
                    { "tick", factoryResponse.info.tick.ToString() },
                    
                };

                var response = await HttpHelper.PostMultipartWithContent(UrlPick, sendToTake);

                var pickContainer = JsonConvert.DeserializeObject<BasicResponse>(response);

                if (pickContainer is null || !pickContainer.success)
                {
                    Console.WriteLine($"Failed on pick. Restart in 1 sec... {response}");

                    await Task.Delay(DelayMilliseconds);

                    continue;
                }

                Console.WriteLine($"Success {iteration}");

                await Task.Delay(1200);

                //break;
            }
        }

        private async Task<FactoryResponse?> DoIteration()
        {
            var response = await HttpHelper.PostMultipart(UrlGenerate);

            var container = JsonConvert.DeserializeObject<FactoryResponse>(response);

            //Console.WriteLine(response);

            return container;
        }
    }
}
