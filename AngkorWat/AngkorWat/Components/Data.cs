using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    /// <summary>
    /// Внутреннее представление входных данных
    /// </summary>
    internal class Data
    {
        public Dictionary<string, Planet> Planets { get; set; } = new();
        public List<Route> Routes { get; set; } = new();
        public ILookup<Planet, Route> RoutesFrom { get; set; }
        public ILookup<Planet, Route> RoutesTo { get; set; }
        public Ship Ship { get; internal set; }

        public HashSet<string> BannedPlanets { get; set; } = new(); 
        public HashSet<string> AlwaysEmpty { get; set; } = new() 
        {
            "Earth", "Eden",
        };

        public Data()
        {
        }
    }
}
