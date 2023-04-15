using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Utils
{
    internal static class RandomSelector
    {
        public static T SelectRandomWithWeights<T>(Dictionary<T, double> items, Random random)
            where T : notnull
        {
            Debug.Assert(items != null);
            Debug.Assert(items.Count > 0);
            Debug.Assert(items.All(kv => kv.Value >= 0));
            Debug.Assert(items.Any(kv => kv.Value > 0));


            //if (items.Count == 0)
            //{
            //    return new T();
            //}

            var totalWeight = items.Sum(kv => kv.Value);

            var randValue = random.NextDouble() * totalWeight;

            double curWeight = 0.0d;

            foreach (var (item, weight) in items)
            {
                curWeight += weight;

                if (randValue <= curWeight)
                {
                    return item;
                }
            }

            return items.Last().Key;
        }

        public static T SelectRandomWithWeights<T>(IEnumerable<T> items, Func<T, double> weightSelector, Random random)
        //where T : notnull
        {
            Debug.Assert(items != null);
            Debug.Assert(items.Count() > 0);
            //Debug.Assert(items.All(kv => kv.Value >= 0));
            //Debug.Assert(items.Any(kv => kv.Value > 0));


            //if (items.Count == 0)
            //{
            //    return new T();
            //}

            var totalWeight = items.Sum(i => weightSelector(i));

            var randValue = random.NextDouble() * totalWeight;

            double curWeight = 0.0d;

            foreach (var item in items)
            {
                curWeight += weightSelector(item);

                if (randValue <= curWeight)
                {
                    return item;
                }
            }

            return items.Last();
        }

        public static T SelectRandom<T>(List<T> items, Random random)
            where T : class
        {
            Debug.Assert(items != null);
            Debug.Assert(items.Count > 0);

            var randValue = random.Next(items.Count);

            return items[randValue];
        }

        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        public static void Test()
        {
            Random random = new Random();

            Dictionary<string, double> options = new()
            {
                { "A", 0.25d },
                { "B", 0.75d },
            };

            var rets = new List<string>();

            for (int i = 0; i < 100; i++)
            {
                //var option = SelectRandomWithWeights(options, random);

                //rets.Add(option);
            }

            var groupResult = rets
                .GroupBy(d => d)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var (result, count) in groupResult)
            {
                Console.WriteLine($"{result}: {count}");
            }
        }
    }
}