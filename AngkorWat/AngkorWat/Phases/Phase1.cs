﻿using AngkorWat.Algorithms.Strategies;
using AngkorWat.Components;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal static class Phase1
    {
        public async static Task Phase1Start()
        {
            var rawMap = IOHelper.ReadInputData<RawMap>("map.json");

            var map = new Map(rawMap);

            map.PrintInfo();

            var data = new Data(map);

            //await LoadScan(data);

            //await RequestLongScan(1000, 1000);
            //await RequestLongScan(1000, 1000);

            //await SendCommand(data);

            while (true)
            {
                await LoadScan(data);
                await SendCommand(data);

                await Task.Delay(3000);
            }

            ///var data = PrepairData(mapId, inputContainer);

            ///var outputContainer = new BaseOutputContainer(mapId);

            ///IOHelper.SerializeResult(outputContainer);
        }

        public static async Task LoadScan(Data data)
        {
            var rawScan = await HttpHelper.Get<RawScan>("https://datsblack.datsteam.dev/api/scan");

            if (rawScan is null)
            {
                throw new NullReferenceException("RawScan can't parse");
            }

            data.CurrentScan = rawScan.Scan;

            foreach (var ship in data.CurrentScan.MyShips)
            {
                ship.Initialize();
            }

            foreach (var ship in data.CurrentScan.EnemyShips)
            {
                ship.Initialize();
            }

            Console.WriteLine($"Scan is get for tick {rawScan.Scan.Tick}");
        }

        public static async Task RequestLongScan(int x, int y)
        {
            var longScan = new LongScan()
            {
                X = x,
                Y = y,
            };

            var ret = await HttpHelper.Post<LongScan, RawLongScanResponse>(
                "https://datsblack.datsteam.dev/api/longScan", longScan);

            if (ret is not null && ret.IsSuccess)
            {
                Console.WriteLine($"Long-range Scan is successful for tick {ret.Tick}");
            }
            else
            {
                Console.WriteLine("Long-range Scan is failed");

                if (ret is not null)
                {
                    Console.WriteLine(string.Join("\n\t", ret.Errors.Select(e => e.Message)));
                }
            }
        }

        public static async Task SendCommand(Data data)
        {
            var commands = IShipStrategy.GenerateEmpty(data);

            var strategy = new FireAtWillStrategy();
            strategy.UpdateCommands(data, commands);

            var fullOrder = new FullOrder() { Commands = commands };

            var ret = await HttpHelper.Post<FullOrder, RawLongScanResponse>(
                "https://datsblack.datsteam.dev/api/shipCommand", fullOrder);

            if (ret is not null && ret.IsSuccess)
            {
                Console.WriteLine($"Commands to ships are send successfully for tick {ret.Tick}");
            }
            else
            {
                Console.WriteLine("Commands to ships are failed");

                if (ret is not null)
                {
                    Console.WriteLine(string.Join("\n\t", ret.Errors.Select(e => e.Message)));
                }
            }
        }
    }
}
