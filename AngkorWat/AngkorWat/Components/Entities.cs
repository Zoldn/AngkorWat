using AngkorWat.IO;
using Newtonsoft.Json;
using OperationsResearch.Pdlp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    public class ZombieSpawn
    {
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;  
        public ZPotType PotType { get; set; }
        public ZombieSpawn() { }
        public void ParseType()
        {
            if (!EnumParserHelper.TryParseZPotType(Type, out var type))
            {
                throw new Exception();
            }

            PotType = type.Value;
        }
    }
    public class StaticWorld
    {
        [JsonProperty("realmName")]
        public string RealName { get; set; } = string.Empty;
        [JsonProperty("zpots")]
        public List<ZombieSpawn> ZPots { get; set; } = new();
        public Dictionary<(int, int), ZombieSpawn> ZPotsDict = new();
        public StaticWorld() { }

        internal void FillNullLists()
        {
            ZPots ??= new();
        }
        internal void FillDict()
        {
            ZPotsDict = ZPots.ToDictionary(b => (b.X, b.Y)); 
        }
    }

    public class ErrorRespond
    {
        [JsonProperty("errCode")]
        public int ErrorCode { get; set; }
        [JsonProperty("error")]
        public string ErrorText { get; set; } = string.Empty;
        public ErrorRespond() { }
        public override string ToString()
        {
            return $"{ErrorCode}: {ErrorText}";
        }
    }

    public class Coordinate
    {
        public Coordinate() { }
        public Coordinate(Coordinate c)
        {
            X = c.X; 
            Y = c.Y;
        }

        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
    }

    public class BaseTile
    {
        [JsonProperty("attack")]
        public int Attack { get; set; }
        [JsonProperty("health")]
        public int Health { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        [JsonProperty("isHead")]
        public bool IsHead { get; set; }
        [JsonProperty("lastAttack")]
        public Coordinate LastAttack { get; set; } = new();
        [JsonProperty("range")]
        public int Range { get; set; }
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }


        public bool IsReadyToShoot { get; set; }
        public BaseTile() { }

        public BaseTile(BaseTile c)
        {
            Attack = c.Attack;
            Health = c.Health;
            Id = c.Id;

            if (c.LastAttack is not null)
            {
                LastAttack = new Coordinate() { X = c.LastAttack.X, Y = c.LastAttack.Y };
            }
            
            Range = c.Range;
            X = c.X;
            Y = c.Y;
            IsHead = c.IsHead;
            IsReadyToShoot = c.IsReadyToShoot;
        }
    }

    public class EnemyBaseTile
    {
        [JsonProperty("attack")]
        public int Attack { get; set; }
        [JsonProperty("health")]
        public int Health { get; set; }
        [JsonProperty("name")]
        public string Id { get; set; } = string.Empty;
        [JsonProperty("isHead")]
        public bool IsHead { get; set; }
        [JsonProperty("lastAttack")]
        public Coordinate LastAttack { get; set; } = new();
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        public EnemyBaseTile() { }

        public EnemyBaseTile(EnemyBaseTile e)
        {
            e.Attack = Attack;
            e.Health = Health;
            e.Id = Id;
            e.IsHead = IsHead;

            if (e.LastAttack is not null)
            {
                e.LastAttack = new Coordinate(e.LastAttack);
            }
            
            e.X = X;
            e.Y = Y;
        }
    }

    public class Player
    {
        [JsonProperty("enemyBlockKills")]
        public int EnemyBlockKills { get; set; }
        [JsonProperty("gameEndedAt")]
        public string GameEndedAt { get; set; } = string.Empty;
        [JsonProperty("gold")]
        public int Gold { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
        [JsonProperty("zombieKills")]
        public int ZombieKills { get; set; }
        [JsonProperty("points")]
        public int Points { get; set; }
    }

    public class Zombie
    {
        [JsonProperty("attack")]
        public int Attack { get; set; }
        [JsonProperty("health")]
        public int Health { get; set; }
        [JsonProperty("direction")]
        public string Direction { get; set; } = string.Empty;
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;
        [JsonProperty("speed")]
        public int Speed { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;
        [JsonProperty("waitTurns")]
        public int WaitTurns { get; set; }
        [JsonProperty("x")]
        public int X { get; set; }
        [JsonProperty("y")]
        public int Y { get; set; }
        public ZombieType ZombieTypeEnum { get; set; }
        public DirectionType DirectionEnum { get; set; }
        public Zombie() { }

        public Zombie(Zombie zombie)
        {
            Attack = zombie.Attack;
            Health = zombie.Health;
            Direction = zombie.Direction;
            Id = zombie.Id;
            Speed = zombie.Speed;
            Type = zombie.Type;
            WaitTurns = zombie.WaitTurns;
            X = zombie.X;
            Y = zombie.Y;
            ZombieTypeEnum = zombie.ZombieTypeEnum;
            DirectionEnum = zombie.DirectionEnum;
        }

        internal void ParseTypes()
        {
            if (!EnumParserHelper.TryParseZombieType(Type, out var ztype))
            {
                throw new Exception();
            }

            ZombieTypeEnum = ztype.Value;

            if (!EnumParserHelper.TryParseZombieDirection(Direction, out var dir))
            {
                throw new Exception();
            }

            DirectionEnum = dir.Value;
        }
    }

    public class DynamicWorld
    {
        [JsonProperty("base")]
        public List<BaseTile> Base { get; set; } = new();
        [JsonProperty("enemyBlocks")]
        public List<EnemyBaseTile> EnemyBases { get; set; } = new();
        [JsonProperty("player")]
        public Player Player { get; set; } = new();
        [JsonProperty("realmName")]
        public string RealmName { get; set; } = string.Empty;
        [JsonProperty("turn")]
        public int Turn { get; set; }
        [JsonProperty("turnEndsInMs")]
        public long TurnEndsInMs { get; set; }
        [JsonProperty("zombies")]
        public List<Zombie> Zombies { get; set; } = new();
        /// <summary>
        /// Удалось ли получить последнюю актуальную информацию по раунду
        /// </summary>
        public bool IsUpdated { get; set; }
        public Dictionary<(int, int), BaseTile> BaseTileDict { get; set; } = new();
        public Dictionary<(int, int), EnemyBaseTile> EnemyBasesDict { get; set; } = new();
        public Dictionary<(int, int), Zombie> ZombiesDict { get; set; } = new();
        public DynamicWorld() { }
        internal void FillNullLists()
        {
            Base ??= new();
            EnemyBases ??= new();
            Zombies ??= new();
        }
        internal void FillDicts()
        {
            BaseTileDict = Base.ToDictionary(b => (b.X, b.Y));
            EnemyBasesDict = EnemyBases.ToDictionary(b => (b.X, b.Y));
            //ZombiesDict = Zombies.ToDictionary(b => (b.X, b.Y));
        }

        public bool TryGetBaseCenter([MaybeNullWhen(false)][NotNullWhen(true)] out BaseTile? baseCenter)
        {
            baseCenter = Base.FirstOrDefault(b => b.IsHead);

            return baseCenter is not null;
        }
    }

    public class ShootCommand
    {
        [JsonProperty("blockId")]
        public string BlockId { get; set; } = string.Empty;
        [JsonProperty("target")]
        public Coordinate Target { get; set; } = new();
    }

    public class TurnCommand
    {
        [JsonProperty("attack")]
        public List<ShootCommand> ShootCommands { get; set; } = new();
        [JsonProperty("build")]
        public List<Coordinate> BuildCommands { get; set; } = new();
        [JsonProperty("moveBase")]
        public Coordinate? MoveCommand { get; set; } = null;
        public TurnCommand() { }
    }

    public class TurnCommandRespond
    {
        [JsonProperty("acceptedCommands")]
        public TurnCommand AcceptedCommands { get; set; } = new();
        [JsonProperty("errors")]
        public List<string> Errors { get; set; } = new();
        public TurnCommandRespond() { }
    }
}
