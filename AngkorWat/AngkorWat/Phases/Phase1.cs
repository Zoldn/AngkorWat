using AngkorWat.Components;
using AngkorWat.IO;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using AngkorWat.Utils;
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
            /*
            foreach (var item in data.AggregatedNews.OrderByDescending(e => e.Rate))
            {
                Console.WriteLine($"{item.Ticker}: {item.Rate}");
            }
            */

            /// 

            int iteration = 0;

            while (true)
            {
                Data data = new Data();

                await FetchData(data);                

                AggregateNews(data);

                Console.WriteLine($"Doing iteration {iteration}");

                ///await TryToBuyCheapest(data);

                await TryToSell(data);

                Console.WriteLine($"Snoozing...");

                await Task.Delay(60000);
            }
        }

        private static async Task TryToSell(Data data)
        {
            var cheapingActives = data
                .AggregatedNews
                .Where(n => n.Rate < 1.0d)
                .Select(n => n.Ticker)
                .ToHashSet();

            var assetsForSale = data.AccountInfo.Assets
                .Where(a => a.Quantity > 0
                    && cheapingActives.Contains(a.Name)
                )
                //.Select(a => a.Name)
                .ToList();

            foreach (var asset in assetsForSale)
            {
                var active = data.Actives.Single(s => s.Ticker == "Oranges/" + asset.Name);

                await PlaceBidBestPriceSell(active, asset.Quantity);

                await Task.Delay(1100);
            }
        }

        private static async Task TryToBuyCheapest(Data data)
        {
            var lastNew = data.AggregatedNews
                .OrderByDescending(e => e.Rate)
                .FirstOrDefault();

            if (lastNew is null
                || lastNew.Rate <= 1.0d)
            {
                Console.WriteLine("Can't buy cheapest");
                return;
            }

            var active = data.Actives.Single(s => s.Ticker == "Oranges/" + lastNew.Ticker);

            //await PlaceBidForLimitPriceBuy(active, 150, 1);

            await PlaceBidForBestPriceBuy(active, 1);
        }

        private static async Task PlaceBidBestPriceSell(Active active, int quantity)
        {
            var bid = new BidWithoutPriceLimit()
            {
                SymbolId = active.Id,
                Quantity = quantity,
            };

            var result = await HttpHelper.Post("https://datsorange.devteam.games/BestPriceSell", bid);

            Console.WriteLine($"PlaceBidBestPriceSell result is: \n {result}");
        }

        private static async Task PlaceBidForLimitPriceSell(Active active, int priceLimit, int quantity)
        {
            var bid = new BidWithPriceLimit()
            {
                SymbolId = active.Id,
                Price = priceLimit,
                Quantity = quantity,
            };

            var result = await HttpHelper.Post("https://datsorange.devteam.games/LimitPriceSell", bid);

            Console.WriteLine($"PlaceBidForLimitPriceSell result is: \n {result}");
        }

        private static async Task PlaceBidForLimitPriceBuy(Active active, int priceLimit, int quantity)
        {
            var bid = new BidWithPriceLimit() 
            {
                SymbolId = active.Id,
                Price = priceLimit,
                Quantity = quantity,
            };

            var result = await HttpHelper.Post("https://datsorange.devteam.games/LimitPriceBuy", bid);

            Console.WriteLine($"PlaceBidForLimitPriceBuy result is: \n {result}");
        }

        private static async Task PlaceBidForBestPriceBuy(Active active, int quantity)
        {
            var bid = new BidWithoutPriceLimit()
            {
                SymbolId = active.Id,
                Quantity = quantity,
            };

            var result = await HttpHelper.Post("https://datsorange.devteam.games/BestPriceBuy", bid);

            Console.WriteLine($"PlaceBidForBestPriceBuy result is: \n {result}");
        }

        private static void AggregateNews(Data data)
        {
            data.AggregatedNews = data.News
                .SelectMany(
                    n => n.AffectedActives,
                    (n, e) => new
                    {
                        ActiveName = e,
                        Rate = n.Rate
                    })
                .GroupBy(e => e.ActiveName)
                .Select(g => new AggregatedNew()
                {
                    Ticker = g.Key,
                    Rate = g.Product(e => 1.0d + e.Rate / 100.0d),
                })
                .ToList();
        }

        private static async Task FetchData(Data data)
        {
            Console.WriteLine("Fetching actives");
            var actives = await HttpHelper.Get<List<Active>>("https://datsorange.devteam.games/getSymbols");

            await Task.Delay(1100);

            Console.WriteLine("Fetching news");

            var news = await HttpHelper.Get<List<New>>("https://datsorange.devteam.games/news/LatestNews5Minutes");

            await Task.Delay(1100);

            Console.WriteLine("Fetching account info");

            var accountInfo = await HttpHelper.Get<AccountInfo>("https://datsorange.devteam.games/info");

            await Task.Delay(1100);

            data.Actives = actives ?? new List<Active>();
            data.News = news ?? new List<New>();
            data.AccountInfo = accountInfo ?? new AccountInfo();
        }
    }
}