using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    public enum ZombieType
    {
        Normal = 0,
        Fast,
        Bomber,
        Liner,
        Jaggernaut,
        ChaosKnight,
    }

    public enum DirectionType
    {
        Up = 0,
        Right,
        Down,
        Left,
    }

    public enum ZPotType
    {
        Default = 0,
        Wall,
    }

    public static class DirectionHelper
    {
        private static Dictionary<DirectionType, (int, int)> DirectionVectors = new(4)
        {
            { DirectionType.Up, (0, 1) },
            { DirectionType.Left, (-1, 0) },
            { DirectionType.Down, (0, -1) },
            { DirectionType.Right, (1, 0) },
        };
        //public static (int, int) GetShiftForDirection(DirectionType direction)
        //{

        //}
    }

    public static class EnumParserHelper
    {
        public static bool TryParseZombieType(string str,
            [MaybeNullWhen(false)][NotNullWhen(true)] out ZombieType? zombieType)
        {
            zombieType = str switch
            {
                "normal" => ZombieType.Normal,
                "fast" => ZombieType.Fast,
                "bomber" => ZombieType.Bomber,
                "liner" => ZombieType.Liner,
                "juggernaut" => ZombieType.Jaggernaut,
                "chaos_knight" => ZombieType.ChaosKnight,
                _ => null,
            };

            if (zombieType is null)
            {
                Console.WriteLine($"Failed to parse zombie type {str}");
            }

            return zombieType is not null;
        }

        public static bool TryParseZombieDirection(string str,
            [MaybeNullWhen(false)][NotNullWhen(true)] out DirectionType? direction)
        {
            direction = str switch
            {
                "up" => DirectionType.Up,
                "left" => DirectionType.Left,
                "down" => DirectionType.Down,
                "right" => DirectionType.Right,
                _ => null,
            };

            if (direction is null)
            {
                Console.WriteLine($"Failed to parse direction {str}");
            }

            return direction is not null;
        }

        public static bool TryParseZPotType(string str,
            [MaybeNullWhen(false)][NotNullWhen(true)] out ZPotType? zpotType)
        {
            zpotType = str switch
            {
                "default" => ZPotType.Default,
                "wall" => ZPotType.Wall,
                _ => null,
            };

            if (zpotType is null)
            {
                Console.WriteLine($"Failed to parse zpotType {str}");
            }

            return zpotType is not null;
        }
    }
}
