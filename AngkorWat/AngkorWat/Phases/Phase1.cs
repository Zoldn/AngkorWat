using AngkorWat.Algorithms;
using AngkorWat.Components;
using AngkorWat.Components.BuildingStrategies;
using AngkorWat.Components.MoveCenterStrategies;
using AngkorWat.Components.ShootingStrategies;
using AngkorWat.IO.HTTP;
using AngkorWat.IO.JSON;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Phases
{
    internal class Phase1
    {
        private readonly static string _testServer = "https://games-test.datsteam.dev/";
        private readonly static string _prodServer = "https://games.datsteam.dev/";

        public bool IsTest { get; init; } = true; 
        /// <summary>
        /// Если true, то данные запрашиваются из сервера, если false, то из локальных файлов
        /// </summary>
        public bool IsServer { get; init; } = true;
        public string PathToStaticData { get; init; } = @"E:\Projects\Hackaton\Defence\static.json";
        public string PathToDynamicData { get; init; } = @"E:\Projects\Hackaton\Defence\dynamic.json";

        public Phase1() 
        {

        }

        public async Task Run()
        {
            IBuildStrategy buildStrategy = new SquareBuildStrategy() ;
            IShootStrategy shootStrategy = new BasicShootStrategy();
            IMoveCenterStrategy moveStrategy = new EvadeToBorder();

            var predictor = new ZombieTurnPredictor();

            var data = new WorldState();

            // constants
            int turnsBetweenStaticUpdates = 10;
            int lastStaticUpdate = turnsBetweenStaticUpdates;

            while (true)
            {
                if (lastStaticUpdate == turnsBetweenStaticUpdates)
                {
                    lastStaticUpdate = 0;
                    await LoadStaticData(data);
                    await Task.Delay(200);
                }
                lastStaticUpdate++;
                await LoadDynamicData(data);

                if (DoStop(data))
                {
                    break;
                }

                PrintCurrentState(data);

                ResetCommands(data);

                shootStrategy.AddCommand(data);
                buildStrategy.AddCommand(data);
                moveStrategy.AddCommand(data);

                //var next = predictor.GetNextTurnWorld(data, data.DynamicWorld);

                PrintGeneratedCommands(data);

                if (IsServer)
                {
                    await Task.Delay(200);

                    await SendCommands(data);

                    await Task.Delay((int)(data.DynamicWorld.TurnEndsInMs + 200));
                } else
                {
                    break;
                }
            }
        }

        private bool DoStop(WorldState data)
        {
            if (!data.DynamicWorld.IsUpdated)
            {
                return false;
            }

            if (!data.DynamicWorld.TryGetBaseCenter(out _)) 
            {
                Console.WriteLine("Can't find main base in last dynamic world. YOU DIED. Sending stops");
                return true;
            }

            return false;
        }

        private void PrintCurrentState(WorldState data)
        {
            Console.WriteLine($"Turn {data.DynamicWorld.Turn}: " +
                $"baseSize={data.DynamicWorld.Base.Count}, " +
                $"nearbyZombies={data.DynamicWorld.Zombies.Count}, " +
                $"gold={data.DynamicWorld.Player.Gold}, " +
                $"HP={data.DynamicWorld.Base.Sum(b => b.Health)}");
        }

        private void ResetCommands(WorldState data)
        {
            data.TurnCommand.ShootCommands.Clear();

            foreach (var baseTile in data.DynamicWorld.Base)
            {
                baseTile.IsReadyToShoot = true;
            }

            data.TurnCommand.BuildCommands.Clear();
            data.TurnCommand.MoveCommand = new();
        }

        private void PrintGeneratedCommands(WorldState data)
        {
            Console.WriteLine($"\tShooting {data.TurnCommand.ShootCommands.Count} base tiles, " +
                $"building {data.TurnCommand.BuildCommands.Count} base tiles");
        }

        internal string GetServer()
        {
            return IsTest ? _testServer : _prodServer;
        }

        internal async Task SendCommands(WorldState data)
        {
            var ret = await HttpHelper.Post<TurnCommand, TurnCommandRespond, ErrorRespond>(
                GetServer() + "play/zombidef/command", data.TurnCommand);

            if (!ret.IsOk)
            {
                Console.WriteLine("Failed to send command");

                if (ret.Error is not null)
                {
                    Console.WriteLine($"\t{ret.Error.ErrorText}");
                }

                return;
            }
        }

        internal async Task LoadStaticData(WorldState data)
        {
            if (IsServer) 
            {
                await LoadStaticDataFromServer(data);
            }
            else
            {
                LoadStaticDataFromFile(data);
            }
        }

        internal void LoadStaticDataFromFile(WorldState data)
        {
            try
            {
                var json = File.ReadAllText(PathToStaticData);

                var staticData = JsonConvert.DeserializeObject<StaticWorld>(json) ??
                    throw new NullReferenceException("Failed to parse static data");

                data.SetStaticData(staticData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal async Task LoadStaticDataFromServer(WorldState data)
        {
            var ret = await HttpHelper.Get<StaticWorld, ErrorRespond>(GetServer() + "play/zombidef/world");

            if (!ret.IsOk || ret.Output is null)
            {
                Console.WriteLine($"Failed to load static data");

                if (ret.Error is not null)
                {
                    Console.WriteLine($"\t{ret.Error}");
                }

                return;
            }

            data.SetStaticData(ret.Output);            
        }

        internal async Task LoadDynamicData(WorldState data)
        {
            if (IsServer)
            {
                await LoadDynamicDataFromServer(data);
            }
            else
            {
                LoadDynamicDataFromFile(data);
            }
        }

        private void LoadDynamicDataFromFile(WorldState data)
        {
            try
            {
                var json = File.ReadAllText(PathToDynamicData);

                var staticData = JsonConvert.DeserializeObject<DynamicWorld>(json) ??
                    throw new NullReferenceException("Failed to parse dynamic data");

                data.SetDynamicData(staticData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal async Task LoadDynamicDataFromServer(WorldState data)
        {
            data.DynamicWorld.IsUpdated = false;

            var ret = await HttpHelper.Get<DynamicWorld, ErrorRespond>(GetServer() + "play/zombidef/units");

            if (!ret.IsOk || ret.Output is null)
            {
                Console.WriteLine($"Failed to load dynamic data");

                if (ret.Error is not null)
                {
                    Console.WriteLine($"\t{ret.Error}");
                }

                return;
            }

            data.SetDynamicData(ret.Output);

            data.DynamicWorld.IsUpdated = true;
        }
    }
}
