using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase2MIP.HappinessFunctions
{
    internal class Phase3TrueHappinessFunction : IHappinessFunction
    {
        // Обучающие игры[educational_games]
        // Музыкальные игры[music_games]
        // Игрушки в ванную[bath_toys]
        // Велосипед[bike]
        // Краски[paints]
        // Шкатулка[casket]
        // Футбольный мяч[soccer_ball]
        // Игрушечная кухня[toy_kitchen]

        public static Dictionary<string, List<int>> BaseByTypesMales = new()
        {
            { "educational_games", new List<int> {20, 20, 10 } },
            { "music_games", new List<int> {20, 15, 15 } },
            { "bath_toys", new List<int> {40, 30, 10 } },
            { "bike", new List<int> {5, 20, 40 } },
            { "paints", new List<int> {25, 25, 15 } },
            { "casket", new List<int> {5,   5,  5 } },
            { "soccer_ball", new List<int> {10,   25,  40 } },
            { "toy_kitchen", new List<int> {5,   15,  10 } },
        };

        public static Dictionary<string, List<int>> BaseByTypesFemales = new()
        {
            { "educational_games", new List<int> {20, 20, 15 } },
            { "music_games", new List<int> {20, 20, 15 } },
            { "bath_toys", new List<int> {40, 30, 10 } },
            { "bike", new List<int> {5, 20, 30 } },
            { "paints", new List<int> {25, 25, 20 } },
            { "casket", new List<int> {10,   20,  30 } },
            { "soccer_ball", new List<int> {5,   15,  20 } },
            { "toy_kitchen", new List<int> {5,   25,  40 } },
        };

        private readonly Dictionary<string, int> minTypePrice = new();
        private readonly Dictionary<string, int> maxTypePrice = new();

        public static int ChildAgeToAgeGroup(ChildrenGroup child)
        {
            if (child.Age <= 2) { return 0; }
            if (child.Age <= 4) { return 1; }
            return 2;
        }

        public Phase3TrueHappinessFunction(Data data)
        {
            minTypePrice = data
                .Gifts
                .GroupBy(g => g.Type)
                .ToDictionary(g => g.Key, g => g.Min(e => e.Price));

            maxTypePrice = data
                .Gifts
                .GroupBy(g => g.Type)
                .ToDictionary(g => g.Key, g => g.Max(e => e.Price));
        }

        public int GetHappiness(ChildrenGroup childrenGroup, GiftGroup giftGroup)
        {
            Dictionary<string, List<int>> baseData;

            if (childrenGroup.Gender == "male")
            {
                baseData = BaseByTypesMales;
            }
            else
            {
                baseData = BaseByTypesFemales;
            }

            int ageGroup = ChildAgeToAgeGroup(childrenGroup);

            int baseValue = baseData[giftGroup.Type][ageGroup];

            return (int)Math.Round(baseValue *
                (1.0d +
                    (double)(giftGroup.Price - minTypePrice[giftGroup.Type]) /
                    (maxTypePrice[giftGroup.Type] - minTypePrice[giftGroup.Type])
                   )
                   );
        }
    }
}
