using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms
{
    internal class ZombieTurnPredictor
    {
        private Dictionary<ZombieType, Action<DynamicWorld, DynamicWorld>> ZombieHandlers = new() 
        {
            //{ ZombieType.Normal, NormalHandler },
        };
        public ZombieTurnPredictor() { }
        public DynamicWorld GetNextTurnWorld(WorldState worldState, DynamicWorld current) 
        {
            var next = new DynamicWorld();

            foreach (var zombie in current.Zombies)
            {

            }

            return next;
        }
        public void NormalHandler(DynamicWorld current, DynamicWorld next) 
        {
            
        }
    }
}
