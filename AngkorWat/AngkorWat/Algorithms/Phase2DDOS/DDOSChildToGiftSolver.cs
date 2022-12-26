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
    internal class DDOSChildToGiftSolver
    {
        private Phase2Data data;
        public DDOSChildToGiftSolver(Phase2Data data)
        {
            this.data = data;
        }

        public ChildToGiftSolution SolveBase()
        {
            var solution = new ChildToGiftSolution();

            solution = MakeBaseLevel();

            return solution;
        }

        public (ChildToGiftSolution Base, ChildToGiftSolution Test) SolveVictim(int backet = 0)
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

            var testPairs = new List<ChildToGift>()
            {
                new ChildToGift(lowestAgeVictim, lowestGift),
                new ChildToGift(lowestAgeVictim, averageGift),
                new ChildToGift(lowestAgeVictim, highestGift),
                new ChildToGift(averageAgeVictim, lowestGift),
                new ChildToGift(averageAgeVictim, averageGift),
                new ChildToGift(averageAgeVictim, highestGift),
                new ChildToGift(highestAgeVictim, lowestGift),
                new ChildToGift(highestAgeVictim, averageGift),
                new ChildToGift(highestAgeVictim, highestGift),
            };

            var testSolution = new ChildToGiftSolution();

            testSolution.ChildToGifts = testPairs;

            var solution = MakeBaseLevel(testPairs);

            return (solution, testSolution);
        }

        private ChildToGiftSolution MakeBaseLevel(List<ChildToGift> excludes)
        {
            var solution = new ChildToGiftSolution();

            var excludeGifts = excludes
                .Select(e => e.Gift)
                .ToHashSet();

            var selectedGifts = data.Gifts
                .Where(e => !excludeGifts.Contains(e))
                .OrderBy(x => x.Price)
                .ThenBy(x => x.Id)
                .Take(data.Children.Count)
                .ToList();

            for (int i = 0; i < data.Children.Count; i++)
            {
                solution.ChildToGifts.Add(new ChildToGift(
                    data.Children[i], selectedGifts[i]
                    ));
            }

            return solution;
        }

        private ChildToGiftSolution MakeBaseLevel()
        {
            var solution = new ChildToGiftSolution();

            var selectedGifts = data.Gifts
                            .OrderBy(x => x.Price)
                            .ThenBy(x => x.Id)
                            .Take(data.Children.Count)
                            .ToList();

            for (int i = 0; i < data.Children.Count; i++)
            {
                solution.ChildToGifts.Add(new ChildToGift(
                    data.Children[i], selectedGifts[i]
                    ));
            }

            return solution;
        }
    }
}
