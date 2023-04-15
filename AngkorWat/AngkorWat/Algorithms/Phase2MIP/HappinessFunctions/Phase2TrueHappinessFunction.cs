using AngkorWat.Components;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace AngkorWat.Algorithms.Phase2MIP.HappinessFunctions
{
    internal class Phase2TrueHappinessFunction : IHappinessFunction
    {

        //constructors
        //dolls
        //radio_controlled_toys
        //toy_vehiclestoy_vehicles
        //board_games
        //outdoor_games
        //playground
        //soft_toys
        //computer_games
        //sweets
        //books
        //pet
        //clothes

        public static Dictionary<string, List<int>> BaseByTypesMales = new()
        {
            { "constructors", new List<int> {5,   10,  20,  40,  5 } },
            { "dolls", new List<int> {1,   5,  1,  0,  0 } },
            { "radio_controlled_toys", new List<int> {1,   3,  10,  30,  40 } },
            { "toy_vehicles", new List<int> {10,   20,  30,  40,  5 } },
            { "board_games", new List<int> {0,   3,  15,  30,  40 } },
            { "outdoor_games", new List<int> {5,   30,  20,  40,  20 } },
            { "playground", new List<int> {10,   30,  40,  30,  0 } },
            { "soft_toys", new List<int> {10,   10,  20,  10, 3 } },
            { "computer_games", new List<int> {0,   5,  30,  40, 40 } },
            { "sweets", new List<int> {1,   15,  20,  20, 10 } },
            { "books", new List<int> {1,   5,  10,  15, 5 } },
            { "pet", new List<int> {1,   10,  10,  10, 5 } },
            { "clothes", new List<int> {0,   3,  5,  10, 15 } },
        };

        public static Dictionary<string, List<int>> BaseByTypesFemales = new()
        {
            { "constructors", new List<int> { 3,  10,  20,  30,  5 } },
            { "dolls", new List<int> { 1, 10,  30,  40,  5 } },
            { "radio_controlled_toys", new List<int> { 1, 3,   10,  20,  1 } },
            { "toy_vehicles", new List<int> { 10,   10,  20,  30,  1 } },
            { "board_games", new List<int> { 0,  3,   10,  30,  40 } },
            { "outdoor_games", new List<int> { 5,    20 , 40,  30,  20 } },
            { "playground", new List<int> { 10 ,  30,  40,  20 , 0 } },
            { "soft_toys", new List<int> { 10,   20,  40,  30,  15 } },
            { "computer_games", new List<int> { 0,    5,   10,  20,  20 } },
            { "sweets", new List<int> {1,   15,  15,  20,  10} },
            { "books", new List<int> { 1,    10,  20 , 20,  20 } },
            { "pet", new List<int> { 1,  5,  10 , 20 , 30 } },
            { "clothes", new List<int> { 0 , 10,  20,  30 , 40 } },
        };

        private Dictionary<string, int> minTypePrice = new();
        private Dictionary<string, int> maxTypePrice = new();

        private static int ChildAgeToAgeGroup(ChildrenGroup child)
        {
            if (child.Age <= 0) { return 0; }
            if (child.Age <= 2) { return 1; }
            if (child.Age <= 4) { return 2; }
            if (child.Age <= 7) { return 3; }
            return 4;
        }

        public Phase2TrueHappinessFunction(Data data)
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
