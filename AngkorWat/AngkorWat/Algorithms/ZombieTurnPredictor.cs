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
        public ZombieTurnPredictor() { }
        public DynamicWorld GetNextTurnWorld(WorldState worldState, DynamicWorld current) 
        {
            var next = new DynamicWorld();

            next.Base = current.Base
                .Select(c => new BaseTile(c))
                .ToList();

            next.EnemyBases = current.EnemyBases
                .Select(e => new EnemyBaseTile(e))
                .ToList();

            next.FillDicts();

            foreach (var zombie in current.Zombies)
            {
                RunZombie(zombie, current, next);
            }

            return next;
        }
        public void NormalHandler(Zombie zombie, DynamicWorld current, DynamicWorld next) 
        {
            for (var step = 0; step < zombie.Speed; step++) 
            {
                var speedVector = DirectionHelper.GetShiftForDirection(zombie.DirectionEnum);

                int futureX = zombie.X + speedVector.X * step;
                int futureY = zombie.Y + speedVector.Y * step;

                ///current.
            }
        }

        public void FastHandler(Zombie zombie, DynamicWorld current, DynamicWorld next) 
        {
            
        }

        public void BomberHandler(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {

        }

        public void LineHandler(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {

        }

        public void JaggernautHandler(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {

        }

        public void ChaosKnightHandler(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {

        }

        public void RunZombie(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {
            if (IsZombieWaiting(zombie, current, next))
            {
                return;
            }

            switch (zombie.ZombieTypeEnum)
            {
                case ZombieType.Normal:
                    NormalHandler(zombie, current, next);
                    break;
                case ZombieType.Fast:
                    FastHandler(zombie, current, next);
                    break;
                case ZombieType.Bomber:
                    BomberHandler(zombie, current, next);
                    break;
                case ZombieType.Liner:
                    LineHandler(zombie, current, next);
                    break;
                case ZombieType.Jaggernaut:
                    JaggernautHandler(zombie, current, next);
                    break;
                case ZombieType.ChaosKnight:
                    ChaosKnightHandler(zombie, current, next);
                    break;
                default:
                    break;
            }
        }

        private bool IsZombieWaiting(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {
            if (zombie.WaitTurns > 1)
            {
                var futureZombie = new Zombie(zombie);
                futureZombie.WaitTurns -= 1;

                next.Zombies.Add(futureZombie);
                return true;
            }

            return false;
        }

        private void KeepZombieAlive(Zombie zombie, DynamicWorld current, DynamicWorld next)
        {
            var futureZombie = new Zombie(zombie)
            {
                WaitTurns = 1
            };

            next.Zombies.Add(futureZombie);
        }
    }
}
