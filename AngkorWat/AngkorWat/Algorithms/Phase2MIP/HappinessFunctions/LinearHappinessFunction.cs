using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Phase2MIP.HappinessFunctions
{
    internal class LinearFunctionForGroup
    {
        public string Gender { get; init; }
        public string GiftType { get; init; }
        public double Base { get; set; }
        public double PriceCoef { get; set; }
        public LinearFunctionForGroup(string gender, string giftType)
        {
            Gender = gender;
            GiftType = giftType;
        }
    }
    internal class LinearHappinessFunction : IHappinessFunction
    {
        private static List<LinearFunctionForGroup> Functions = new() 
        {
            new LinearFunctionForGroup("male", "board_games"){ Base = 41, PriceCoef = 0.31d },
            new LinearFunctionForGroup("male", "clothes"){ Base = 12, PriceCoef = 0.05d },
            new LinearFunctionForGroup("male", "computer_games"){ Base = 48, PriceCoef = 0.8d },
            new LinearFunctionForGroup("male", "constructors"){ Base = 46, PriceCoef = 0.21d },
            new LinearFunctionForGroup("male", "radio_controlled_toys"){ Base = 33, PriceCoef = 0.17d },
            new LinearFunctionForGroup("male", "playground"){ Base = 30, PriceCoef = 0.19d },
            new LinearFunctionForGroup("male", "dolls"){ Base = 0, PriceCoef = 0.0d },
            new LinearFunctionForGroup("male", "sweets"){ Base = 40, PriceCoef = 0.57d },
            new LinearFunctionForGroup("male", "soft_toys"){ Base = 12, PriceCoef = 0.05d },
            new LinearFunctionForGroup("male", "books"){ Base = 23, PriceCoef = 0.35d },
            new LinearFunctionForGroup("male", "toy_vehicles"){ Base = 59, PriceCoef = 0.525d },
            new LinearFunctionForGroup("male", "outdoor_games"){ Base = 53, PriceCoef = 0.45d },
            new LinearFunctionForGroup("male", "pet"){ Base = 7, PriceCoef = 0.08d },

            /*
            new LinearFunctionForGroup("female", "board_games"){ Base = 10, PriceCoef = 0.05d },
            new LinearFunctionForGroup("female", "clothes"){ Base = 40, PriceCoef = 0.8d },
            new LinearFunctionForGroup("female", "computer_games"){ Base = 20, PriceCoef = 0.2d },
            new LinearFunctionForGroup("female", "constructors"){ Base = 10, PriceCoef = 0.05d },
            new LinearFunctionForGroup("female", "radio_controlled_toys"){ Base = 20, PriceCoef = 0.2d },
            new LinearFunctionForGroup("female", "playground"){ Base = 30, PriceCoef = 0.4d },
            new LinearFunctionForGroup("female", "dolls"){ Base = 60, PriceCoef = 0.8d },
            new LinearFunctionForGroup("female", "sweets"){ Base = 40, PriceCoef = 0.6d },
            new LinearFunctionForGroup("female", "soft_toys"){ Base = 40, PriceCoef = 0.5d },
            new LinearFunctionForGroup("female", "books"){ Base = 20, PriceCoef = 0.3d },
            new LinearFunctionForGroup("female", "toy_vehicles"){ Base = 0, PriceCoef = 0.0d },
            new LinearFunctionForGroup("female", "outdoor_games"){ Base = 20, PriceCoef = 0.3d },
            new LinearFunctionForGroup("female", "pet"){ Base = 40, PriceCoef = 0.6d },
            */

            new LinearFunctionForGroup("female", "board_games"){ Base = 21, PriceCoef = 0.32d },
            new LinearFunctionForGroup("female", "clothes"){ Base = 15, PriceCoef = 0.16d },
            new LinearFunctionForGroup("female", "computer_games"){ Base = 4, PriceCoef = 0.4d },
            new LinearFunctionForGroup("female", "constructors"){ Base = 15, PriceCoef = 0.15d },
            new LinearFunctionForGroup("female", "radio_controlled_toys"){ Base = 2, PriceCoef = 0.11d },
            new LinearFunctionForGroup("female", "playground"){ Base = 0, PriceCoef = 0.125d },
            new LinearFunctionForGroup("female", "dolls"){ Base = 39, PriceCoef = 0.525d },
            new LinearFunctionForGroup("female", "sweets"){ Base = 20, PriceCoef = 0.57d },
            new LinearFunctionForGroup("female", "soft_toys"){ Base = 15, PriceCoef = 0.15d },
            new LinearFunctionForGroup("female", "books"){ Base = 11, PriceCoef = 0.45d },
            new LinearFunctionForGroup("female", "toy_vehicles"){ Base = 24, PriceCoef = 0.4d },
            new LinearFunctionForGroup("female", "outdoor_games"){ Base = 20, PriceCoef = 0.3d },
            new LinearFunctionForGroup("female", "pet"){ Base = 3, PriceCoef = 0.16d },
        };

        private static Dictionary<(string Gender, string GiftType), LinearFunctionForGroup> FunctionDict =
            Functions
                .ToDictionary(
                    f => (f.Gender, f.GiftType), 
                    f => f
                    );

        public int GetHappiness(ChildrenGroup childrenGroup, GiftGroup giftGroup)
        {
            var f = FunctionDict[(childrenGroup.Gender, giftGroup.Type)];

            return (int)Math.Round(10 * (f.Base + f.PriceCoef * giftGroup.Price));
        }
    }
}
