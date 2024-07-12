﻿using AngkorWat.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ZombieSpawn() { }
    }
    public class StaticWorld
    {
        [JsonProperty("realmName")]
        public string RealName { get; set; } = string.Empty;
        [JsonProperty("zpots")]
        public List<ZombieSpawn> ZPots { get; set; } = new();
        public StaticWorld() { }

        internal void FillNullLists()
        {
            ZPots ??= new();
        }
    }

    public class ErrorRespond
    {
        [JsonProperty("errCode")]
        public int ErrorCode { get; set; }
        [JsonProperty("error")]
        public string ErrorText { get; set; } = string.Empty;
        public ErrorRespond() { }
    }

    public class Coordinate
    {
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

        public Zombie() { }
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
        public DynamicWorld() { }

        internal void FillNullLists()
        {
            Base ??= new();
            EnemyBases ??= new();
            Zombies ??= new();
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
