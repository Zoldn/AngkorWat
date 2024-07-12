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

        public Phase1() 
        {

        }

        public async Task Run()
        {
            IBuildStrategy buildStrategy = new DoCrossBuildStrategy();
            IShootStrategy shootStrategy = new DoNothingShootStrategy();
            IMoveCenterStrategy moveStrategy = new DoNothingMoveStrategy();

            var data = new WorldState();

            await LoadStaticData(data); 
            await Task.Delay(100);

            while (true)
            {
                await LoadDynamicData(data);

                shootStrategy.AddCommand(data);
                buildStrategy.AddCommand(data);
                moveStrategy.AddCommand(data);

                await Task.Delay(200);

                await SendCommands(data);

                await Task.Delay(1800);
            }
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

            data.StaticWorld = ret.Output;

            data.StaticWorld.FillNullLists();
        }

        internal async Task LoadDynamicData(WorldState data)
        {
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

            data.DynamicWorld = ret.Output;

            data.DynamicWorld.FillNullLists();
        }
    }
}
