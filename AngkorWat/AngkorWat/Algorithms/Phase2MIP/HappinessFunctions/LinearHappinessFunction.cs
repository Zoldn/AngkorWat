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
        public int NormedAge { get; init; }
        public double Base { get; set; }
        public double PriceCoef { get; set; }
        public LinearFunctionForGroup(string gender, string giftType, int normedAge)
        {
            Gender = gender;
            GiftType = giftType;
            NormedAge = normedAge;
        }
    }
    internal class LinearHappinessFunction : IHappinessFunction
    {
        private static List<LinearFunctionForGroup> Functions = new() 
        {
            new LinearFunctionForGroup("male", "educational_games", 0) { Base = 200, PriceCoef = 0.14d },
            new LinearFunctionForGroup("male", "music_games", 0){ Base = 80, PriceCoef = 0.02d },
            new LinearFunctionForGroup("male", "bath_toys", 0){ Base = 160, PriceCoef = 0.14d },
            new LinearFunctionForGroup("male", "bike", 0){ Base = 0, PriceCoef = 0.10d },
            new LinearFunctionForGroup("male", "paints", 0){ Base = 24, PriceCoef = 0.0d },
            new LinearFunctionForGroup("male", "casket", 0){ Base = 4, PriceCoef = 0.16d },
            new LinearFunctionForGroup("male", "soccer_ball", 0){ Base = 32, PriceCoef = 0.1d },
            new LinearFunctionForGroup("male", "toy_kitchen", 0){ Base = 0, PriceCoef = 0.1d },

            new LinearFunctionForGroup("male", "educational_games", 1) { Base = 40, PriceCoef = 0.2d },
            new LinearFunctionForGroup("male", "music_games", 1){ Base = 16, PriceCoef = 0.14d },
            new LinearFunctionForGroup("male", "bath_toys", 1){ Base = 20, PriceCoef = 0.0d },
            new LinearFunctionForGroup("male", "bike", 1){ Base = 220, PriceCoef = 0.20d },
            new LinearFunctionForGroup("male", "paints", 1){ Base = 80, PriceCoef = 0.10d },
            new LinearFunctionForGroup("male", "casket", 1){ Base = 20, PriceCoef = 0.16d },
            new LinearFunctionForGroup("male", "soccer_ball", 1){ Base = 320, PriceCoef = 0.1d },
            new LinearFunctionForGroup("male", "toy_kitchen", 1){ Base = 40, PriceCoef = 0.2d },

            new LinearFunctionForGroup("female", "educational_games", 0) { Base = 200, PriceCoef = 0.14d },
            new LinearFunctionForGroup("female", "music_games", 0){ Base = 300, PriceCoef = 0.02d },
            new LinearFunctionForGroup("female", "bath_toys", 0){ Base = 256, PriceCoef = 0.14d },
            new LinearFunctionForGroup("female", "bike", 0){ Base = 0, PriceCoef = 0.1d },
            new LinearFunctionForGroup("female", "paints", 0){ Base = 36, PriceCoef = 0.0d },
            new LinearFunctionForGroup("female", "casket", 0){ Base = 32, PriceCoef = 0.16d },
            new LinearFunctionForGroup("female", "soccer_ball", 0){ Base = 4, PriceCoef = 0.1d },
            new LinearFunctionForGroup("female", "toy_kitchen", 0){ Base = 0, PriceCoef = 0.1d },

            new LinearFunctionForGroup("female", "educational_games", 1) { Base = 40, PriceCoef = 0.20d },
            new LinearFunctionForGroup("female", "music_games", 1){ Base = 60, PriceCoef = 0.14d },
            new LinearFunctionForGroup("female", "bath_toys", 1){ Base = 160, PriceCoef = 0.0d },
            new LinearFunctionForGroup("female", "bike", 1){ Base = 180, PriceCoef = 0.2d },
            new LinearFunctionForGroup("female", "paints", 1){ Base = 120, PriceCoef = 0.10d },
            new LinearFunctionForGroup("female", "casket", 1){ Base = 160, PriceCoef = 0.16d },
            new LinearFunctionForGroup("female", "soccer_ball", 1){ Base = 40, PriceCoef = 0.10d },
            new LinearFunctionForGroup("female", "toy_kitchen", 1){ Base = 240, PriceCoef = 0.20d },
        };

        private static Dictionary<(string Gender, string GiftType, int NormedAge), LinearFunctionForGroup> FunctionDict =
            Functions
                .ToDictionary(
                    f => (f.Gender, f.GiftType, f.NormedAge), 
                    f => f
                    );

        public int GetHappiness(ChildrenGroup childrenGroup, GiftGroup giftGroup)
        {
            var f = FunctionDict[(childrenGroup.Gender, giftGroup.Type, childrenGroup.Age / 6)];

            return (int)Math.Round(10 * (f.Base + f.PriceCoef * giftGroup.Price));
        }
    }
}
