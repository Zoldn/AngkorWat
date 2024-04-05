using AngkorWat.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AngkorWat.Components
{
    internal class UniverseState
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("ship")]
        public Ship Ship { get; set; } = new();
        [JsonProperty("planet")]
        public Planet Planet { get; set; } = new();
        [JsonProperty("universe")]
        public List<(string, string, int)> Routes { get; set; } = new();
        public UniverseState() { }
    }

    internal class Route
    {
        public string LocationFrom { get; set; } = string.Empty;
        public string LocationTo { get; set; } = string.Empty;
        public int Cost { get; set; }
        public Route() { }
    }

    internal class Ship
    {
        [JsonProperty("capacityX")]
        public int CapacityX { get; set; }
        [JsonProperty("capacityY")]
        public int CapacityY { get; set; }
        [JsonProperty("fuelUsed")]
        public int FuelUsed { get; set; }
        [JsonProperty("garbage")]
        public Dictionary<string, List<List<int>>> Garbage { get; set; } = new();
        public Ship() { }
    }

    internal class Planet
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty; 
        [JsonProperty("garbage")]
        public Dictionary<string, List<List<int>>> Garbage { get; set; } = new();
        public Planet() { }
    }

    internal class Travel
    {
        [JsonProperty("planets")]
        public List<string> Route { get; set; } = new(2);
        public Travel() { }
    }

    internal class TravelResponse
    {
        [JsonProperty("fuelDiff")]
        public int FuelDiff { get; set; }
        [JsonProperty("planetDiffs")]
        public PlanetDiff PlanetDiff { get; set; } = new();
    }
    internal class PlanetDiff
    {
        [JsonProperty("from")]
        public string LocationFrom { get; set; } = string.Empty;
        [JsonProperty("to")]
        public string LocationTo { get; set; } = string.Empty;
        [JsonProperty("fuel")]
        public int Fuel { get; set; }
        [JsonProperty("planetGarbage")]
        public Dictionary<string, List<List<int>>> PlanetGarbage { get; set; } = new();
        [JsonProperty("shipGarbage")]
        public Dictionary<string, List<List<int>>> ShipGarbage { get; set; } = new();
        public PlanetDiff() { }
    }

    internal class CollectGarbage
    {
        [JsonProperty("garbage")]
        public Dictionary<string, List<List<int>>> ShipGarbage { get; set; } = new();
    }

    internal class CollectGarbageResponse
    {
        [JsonProperty("garbage")]
        public Dictionary<string, List<List<int>>> ShipGarbage { get; set; } = new();
        [JsonProperty("leaved")]
        public List<string> LeavedGarbage { get; set; } = new();   
    }
}
