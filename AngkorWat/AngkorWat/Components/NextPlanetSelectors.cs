using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal interface INextPlanetSelector
    {
        internal bool TryGetNextPlanet(Data data, 
            [MaybeNullWhen(false)][NotNullWhen(true)] out Planet? nextPlanet);
    }

    internal class ClosestPlanetSelector : INextPlanetSelector
    {
        public ClosestPlanetSelector() { }
        public bool TryGetNextPlanet(Data data, 
            [MaybeNullWhen(false)][NotNullWhen(true)] out Planet? nextPlanet)
        {
            //Planet currentPlanet = data.Ship.Planet;
            nextPlanet = null;

            var distances = Dijkstra.RunAllFromSource(data, data.Ship.Planet.Name);

            string? nextPlanetName = distances
                .Where(e => !data.BannedPlanets.Contains(e.Key)
                    && !data.AlwaysEmpty.Contains(e.Key)
                )
                .OrderBy(kv => kv.Value)
                .Select(kv => kv.Key)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(nextPlanetName))
            {
                return false;
            }

            nextPlanet = data.Planets[nextPlanetName];

            return nextPlanet is not null;
        }
    }

    internal class ClosestToEdenPlanetSelector : INextPlanetSelector
    {
        public ClosestToEdenPlanetSelector() { }
        public bool TryGetNextPlanet(Data data,
            [MaybeNullWhen(false)][NotNullWhen(true)] out Planet? nextPlanet)
        {
            //Planet currentPlanet = data.Ship.Planet;
            nextPlanet = null;

            var distances = Dijkstra.RunAllFromSource(data, "Eden");

            string? nextPlanetName = distances
                .Where(e => !data.BannedPlanets.Contains(e.Key)
                    && !data.AlwaysEmpty.Contains(e.Key)
                )
                .OrderBy(kv => kv.Value)
                .Select(kv => kv.Key)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(nextPlanetName))
            {
                return false;
            }

            nextPlanet = data.Planets[nextPlanetName];

            return nextPlanet is not null;
        }
    }
}
