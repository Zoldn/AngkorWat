using AngkorWat.Algorithms;
using AngkorWat.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    public class RawMap
    {
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; } = string.Empty;
        [JsonProperty("islands")]
        public List<Island> Islands { get; set; } = new();
        public RawMap() { }
    }

    public class Island
    {
        [JsonProperty("start")]
        public List<int> PivotPoint { get; set; } = new List<int>(2);
        [JsonProperty("map")]
        public List<List<int>> Map { get; set; } = new List<List<int>>();
        public Island() { }
    }

    public class RawScan
    {
        [JsonProperty("errors")]
        public List<string> Errors { get; set; } = new();
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("scan")]
        public Scan Scan { get; set; } = new();
        public RawScan() { }
    }

    public class Scan
    {
        [JsonProperty("myShips")]
        public List<Ship> MyShips { get; set; } = new();
        [JsonProperty("enemyShips")]
        public List<Ship> EnemyShips { get; set; } = new();
        [JsonProperty("zone")]
        public Zone? Zone { get; set; } = null;// = new();
        [JsonProperty("tick")]
        public int Tick { get; set; } = 0;
        public Scan() { }
    }
    
    public enum Directions
    {
        NORTH = 0,
        EAST = 1,
        SOUTH = 2, 
        WEST = 3,
    }

    public class Ship : IPunkt
    {
        [JsonProperty("id")]
        public int ShipId { get; set; } = 1;
        [JsonProperty("x")]
        public int X { get; set; } = 1;
        [JsonProperty("y")]
        public int Y { get; set; } = 1;
        [JsonProperty("size")]
        public int Size { get; set; } = 1;
        [JsonProperty("hp")]
        public int HP { get; set; } = 1;
        [JsonProperty("maxHp")]
        public int MaxHP { get; set; } = 1;
        [JsonProperty("direction")]
        public string RawDirection { get; set; } = "None";
        [JsonProperty("speed")]
        public int Speed { get; set; } = 0;
        [JsonProperty("maxSpeed")]
        public int MaxSpeed { get; set; } = 1;
        [JsonProperty("minSpeed")]
        public int MinSpeed { get; set; } = -1;
        [JsonProperty("maxChangeSpeed")]
        public int MaxChangeSpeed { get; set; } = 1;
        [JsonProperty("cannonCooldown")]
        public int CannonCooldown { get; set; } = 1;
        [JsonProperty("cannonCooldownLeft")]
        public int CannonCooldownLeft { get; set; } = 0;
        [JsonProperty("cannonRadius")]
        public int CannonRadius { get; set; } = 1;
        [JsonProperty("scanRadius")]
        public int ScanRadius { get; set; } = 1;
        [JsonProperty("cannonShootSuccessCount")]
        public int CannonShootSuccessCount { get; set; } = 0;
        [JsonIgnore]
        public Directions Direction { get; set; }
        public Ship() { }
        public void Initialize()
        {
            Direction = DirectionHelper.GetFromString(RawDirection);
        }
    }

    public class Zone
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("radius")]
        public int Radius { get; set; }
    }

    public class LongScan
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
    }
    public class RawLongScanResponse
    {
        [JsonProperty("errors")]
        public List<Error> Errors { get; set; } = new();
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
        [JsonProperty("tick")]
        public int Tick { get; set; }
    }

    public class Error
    {
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }

    public class FullOrder
    {
        [JsonProperty("ships")]
        public List<ShipCommand> Commands { get; set; } = new();
        public FullOrder() { }
    }

    public class ShipCommand
    {
        [JsonIgnore]
        public Ship Ship { get; }
        [JsonProperty("id")]
        public int ShipId { get; }
        [JsonProperty("changeSpeed")]
        public int ChangeSpeed { get; set; } = 0;
        [JsonProperty("rotate")]
        public int? Rotate { get; set; } = null;
        [JsonProperty("cannonShoot")]
        public CannonShoot? Shoot { get; set; } = null;
        public ShipCommand(Ship ship) 
        {
            Ship = ship;
            ShipId = ship.ShipId;
        }
    }

    public class CannonShoot
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
    }
}
