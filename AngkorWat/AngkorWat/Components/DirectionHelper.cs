using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    public static class DirectionHelper
    {
        private static Dictionary<string, Directions> stringToEnum = new Dictionary<string, Directions>()
        {
            { "north", Directions.NORTH },
            { "south", Directions.SOUTH },
            { "east", Directions.EAST },
            { "west", Directions.WEST },
        };

        private static Dictionary<Directions, string> enumToString = stringToEnum
            .ToDictionary(
                kv => kv.Value,
                kv => kv.Key
            );

        public static Directions GetFromString(string str)
        {
            return stringToEnum[str];
        }
    }
}
