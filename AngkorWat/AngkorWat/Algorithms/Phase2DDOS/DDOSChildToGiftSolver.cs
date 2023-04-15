using AngkorWat.Components;
using AngkorWat.Constants;
using AngkorWat.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase2DDOS
{
    internal class VictimSolution
    {
        public List<int> TargetChilds { get; set; }
        public List<int> TargetGifts { get; set; }
        public List<(int ChildId, int GiftId)> Base { get; set; }
        public VictimSolution()
        {
            TargetChilds = new();
            TargetGifts = new();
            Base = new();
        }
    }

    internal class DDOSChildToGiftSolver
    {
        private Data data;
        public DDOSChildToGiftSolver(Data data)
        {
            this.data = data;
        }

        public Dictionary<Child, Gift> SolveBase()
        {
            var solution = new Dictionary<Child, Gift>();

            solution = MakeBaseLevel();

            return solution;
        }

        public (Dictionary<Child, Gift> Base, Dictionary<Child, Gift> Test) SolveVictim(int backet = 0)
        {
            var uniqueGenders = data.Children
                .Select(c => c.Gender)
                .Distinct()
                .ToList();

            var uniqueGiftTypes = data.Gifts
                .Select(e => e.Type)
                .Distinct()
                .ToList();

            List<(string Gender, string GiftType)> backets = uniqueGenders
                .SelectMany(g => uniqueGiftTypes, (g, t) => (g, t))
                .OrderBy(p => p.g)
                .ThenBy(p => p.t)
                .ToList();

            var firstBacket = backets.Skip(backet).First();

            firstBacket = ("male", "toy_vehicles");

            var lowestAge = data.Children
                .Where(c => c.Gender == firstBacket.Gender)
                .Min(c => c.Age);

            var averageAge = data.Children
                .Where(c => c.Gender == firstBacket.Gender)
                .Average(c => c.Age);

            var highestAge = data.Children
                .Where(c => c.Gender == firstBacket.Gender)
                .Max(c => c.Age);

            var lowestAgeVictim = data.Children
                .Where(c => c.Gender == firstBacket.Gender
                    && c.Age == lowestAge
                )
                .OrderBy(e => e.Id)
                .First();

            var highestAgeVictim = data.Children
                .Where(c => c.Gender == firstBacket.Gender
                    && c.Age == highestAge
                )
                .OrderBy(e => e.Id)
                .First();

            var averageAgeVictim = data.Children
                .Where(c => c.Gender == firstBacket.Gender)
                .OrderBy(e => Math.Abs(e.Age - averageAge))
                .ThenBy(e => e.Id)
                .First();

            var lowestPriceGift = data.Gifts
                .Where(g => g.Type == firstBacket.GiftType)
                .Min(g => g.Price);

            var averagePriceGift = data.Gifts
               .Where(c => c.Type == firstBacket.GiftType)
               .Average(c => c.Price);

            var highestPriceGift = data.Gifts
                .Where(g => g.Type == firstBacket.GiftType)
                .Max(g => g.Price);

            var lowestGift = data.Gifts
                .Where(g => g.Type == firstBacket.GiftType
                    && g.Price == lowestPriceGift
                )
                .OrderBy(e => e.Id)
                .First();

            var averageGift = data.Gifts
                .Where(g => g.Type == firstBacket.GiftType)
                .OrderBy(g => Math.Abs(g.Price - averagePriceGift))
                .ThenBy(g => g.Id)
                .First();

            var highestGift = data.Gifts
                .Where(g => g.Type == firstBacket.GiftType
                    && g.Price == highestPriceGift
                )
                .OrderBy(e => e.Id)
                .First();

            var testPairs = new Dictionary<Child, Gift>()
            {
                { lowestAgeVictim, lowestGift },
                { lowestAgeVictim, averageGift },
                { lowestAgeVictim, highestGift },
                { averageAgeVictim, lowestGift },
                { averageAgeVictim, averageGift },
                { averageAgeVictim, highestGift },
                { highestAgeVictim, lowestGift },
                { highestAgeVictim, averageGift },
                { highestAgeVictim, highestGift },
            };

            var solution = MakeBaseLevel(testPairs, firstBacket.GiftType);

            return (solution, testPairs);
        }

        internal (Dictionary<Child, Gift> Base, Dictionary<Child, Gift> Test) SolveVictim2() // 
        {
            var uniqueGenders = data.Children
                .Select(c => c.Gender)
                .Distinct()
                .ToList();

            var uniqueGiftTypes = data.Gifts
                .Select(e => e.Type)
                .Distinct()
                .ToList();

            List<(string Gender, string GiftType)> backets = uniqueGenders
                .SelectMany(g => uniqueGiftTypes, (g, t) => (g, t))
                .OrderBy(p => p.g)
                .ThenBy(p => p.t)
                .ToList();

            var similarTargetPrice = data.Gifts
                .GroupBy(h => h.Price)
                .Where(g => g.Select(e => e.Type).Distinct().Count() >= 12)
                .Select(g => g.Key)
                .First();

            var selected40PriceGifts = uniqueGiftTypes
                .Select(e => data
                    .Gifts
                    .Where(q => q.Type == e && q.Price == similarTargetPrice)
                    .OrderBy(q => q.Id)
                    .FirstOrDefault()
                )
                .Where(e => e != null)
                .ToList();

            var mostPriceGiftsInCategories = data.Gifts
                .GroupBy(e => e.Type)
                .ToDictionary(g => g.Key, g => g.Key != "sweets" ? g.Max(q => q.Price) : g.Min(q => q.Price));

            var additionalGifts = data.Gifts
                .GroupBy(e => e.Type)
                .Select(g => g
                    .Where(q => q.Price == mostPriceGiftsInCategories[q.Type])
                    .OrderBy(q => q.Id)
                    .First()
                    )
                .ToList();

            var leastValuablePetCost = data.Gifts
                .Where(e => e.Type == "pet")
                .Min(e => e.Price);

            var leastValuablePet = data.Gifts
                .Where(e => e.Type == "pet" && e.Price == leastValuablePetCost)
                .OrderBy(e => e.Id)
                .First();

            var totalSelectedGifts = selected40PriceGifts
                .Concat(additionalGifts)
                .Append(leastValuablePet)
                .ToList();

            var maleVictim = data
                .Children
                .Where(e => e.Age == 5 && e.Gender == "male")
                .OrderBy(e => e.Id)
                .First();

            var femaleVictim = data
                .Children
                .Where(e => e.Age == 5 && e.Gender == "female")
                .OrderBy(e => e.Id)
                .First();

            HashSet<Child> selectedChildren = new() 
            { 
                maleVictim, 
                femaleVictim 
            };

            var testPairs = selectedChildren
                .SelectMany(c => totalSelectedGifts, (c, g) => (c, g))
                .ToDictionary(kv => kv.c, kv => kv.g);

            var baseSolution = MakeBaseLevel(testPairs);

            return (baseSolution, testPairs);
        }

        private Dictionary<Child, Gift> MakeBaseLevel(Dictionary<Child, Gift> excludes)
        {
            var solution = new Dictionary<Child, Gift>();

            var excludeGifts = excludes
                .Select(e => e.Value)
                .ToHashSet();

            var freegifts = data.Gifts
                .Where(e => !excludeGifts.Contains(e))
                .ToList();

            var selectedGifts = data.Gifts
                .Where(e => !excludeGifts.Contains(e))
                .OrderBy(x => x.Price)
                .ThenBy(x => x.Id)
                .Take(data.Children.Count)
                .ToList();

            for (int i = 0; i < data.Children.Count; i++)
            {
                solution.Add(data.Children[i], selectedGifts[i]);
            }

            var ttt1 = solution
                .GroupBy(e => e.Value)
                .Count(g => g.Count() > 1);

            var ttt2 = solution
                .Select(e => e.Key)
                .Distinct()
                .Count();

            return solution;
        }

        private Dictionary<Child, Gift> MakeBaseLevel(Dictionary<Child, Gift> excludes, string giftType)
        {
            var solution = new Dictionary<Child, Gift>();

            var excludeGifts = excludes
                .Select(e => e.Value)
                .ToHashSet();

            var freegifts = data.Gifts
                .Where(e => e.Type == giftType
                    && !excludeGifts.Contains(e)
                )
                .ToList();

            var freePrice = freegifts
                .GroupBy(e => e.Price)
                .Where(g => g.Count() >= 3)
                .First()
                .Key;

            var similarGifts = freegifts
                .Where(e => e.Price == freePrice)
                .Take(3)
                .ToList();

            var selectedGifts = data.Gifts
                .Where(e => !excludeGifts.Contains(e) && !similarGifts.Contains(e))
                .OrderBy(x => x.Price)
                .ThenBy(x => x.Id)
                .Take(data.Children.Count - 3)
                .ToList();

            var targetChildren = excludes
                .Select(e => e.Key)
                .Distinct()
                .ToList();

            for (int i = 0; i < 3; i++)
            {
                solution.Add(targetChildren[i], similarGifts[i]);
            }

            int giftCounter = 0;

            foreach (var child in data.Children)
            {
                if (targetChildren.Contains(child))
                {
                    continue;
                }

                solution.Add(child, selectedGifts[giftCounter++]);
            }

            var ttt1 = solution
                .GroupBy(e => e.Value)
                .Count(g => g.Count() > 1);

            var ttt2 = solution
                .Select(e => e.Key)
                .Distinct()
                .Count();

            return solution;
        }

        private Dictionary<Child, Gift> MakeBaseLevel()
        {
            var solution = new Dictionary<Child, Gift>();

            var selectedGifts = data.Gifts
                            .OrderBy(x => x.Price)
                            .ThenBy(x => x.Id)
                            .Take(data.Children.Count)
                            .ToList();

            for (int i = 0; i < data.Children.Count; i++)
            {
                solution.Add(data.Children[i], selectedGifts[i]);
            }

            return solution;
        }
    }
}
