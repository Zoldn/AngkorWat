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

    public static class ZombieTypeHelper
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

            return zombieType is not null;
        }
}
}
