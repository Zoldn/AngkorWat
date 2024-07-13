using AngkorWat.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.ShootingStrategies
{
    internal class VVShootStrategy : IShootStrategy
    {
        private BaseTile _baseCenter;
        private readonly ZombieTurnPredictor _zombieTurnPredictor;
        private readonly Dictionary<ZombieType, int> _priorities = new()
        {
            { ZombieType.Jaggernaut, 1 },
            { ZombieType.Bomber, 2 },
            { ZombieType.Liner, 3 },
            { ZombieType.ChaosKnight, 4 },
            { ZombieType.Fast, 5 },
            { ZombieType.Normal, 6 },
        };
        public VVShootStrategy() 
        {
            _zombieTurnPredictor = new ZombieTurnPredictor();
        }

        public void AddCommand(WorldState worldState)
        {
            var nextTurn = _zombieTurnPredictor.GetNextTurnWorld(worldState, worldState.DynamicWorld);

            if (!worldState.DynamicWorld.TryGetBaseCenter(out var baseCenter))
            {
                return;
            }

            _baseCenter = baseCenter;

            var damagingZombies = worldState.DynamicWorld.Zombies
                .Where(z => z.PossibleDamage > 0)
                .ToList();

            damagingZombies.Sort(SortingDelegate);

            foreach (var zombie in damagingZombies)
            {
                ShootTools.AddShootCommandForTarget(worldState, zombie);
            }

            var nonDamagingZombies = worldState.DynamicWorld.Zombies
                .Where(z => z.PossibleDamage == 0)
                .ToList();

            nonDamagingZombies.Sort(SortingDelegate);

            foreach (var zombie in nonDamagingZombies)
            {
                ShootTools.AddShootCommandForTarget(worldState, zombie);
            }

            var potentialEnemyBases = new List<EnemyBaseTile>();

            foreach (var enemyBase in worldState.DynamicWorld.EnemyBases)
            {
                if (!ShootTools.IsEnemyBaseInFireRange(worldState, enemyBase))
                {
                    continue;
                }

                potentialEnemyBases.Add(enemyBase);
            }

            foreach (var enemy in potentialEnemyBases)
            {
                ShootTools.AddShootCommandForTarget(worldState, enemy);
            }
        }

        private int SortingDelegate(Zombie x, Zombie y)
        {
            if (x.ZombieTypeEnum == ZombieType.Jaggernaut 
                && y.ZombieTypeEnum != ZombieType.Jaggernaut) 
            {
                return 1;
            }

            if (x.ZombieTypeEnum != ZombieType.Jaggernaut
                && y.ZombieTypeEnum == ZombieType.Jaggernaut)
            {
                return -1;
            }

            double distanceForX = ShootTools.GetDistanceFromBaseToZombie(_baseCenter, x);
            double distanceForY = ShootTools.GetDistanceFromBaseToZombie(_baseCenter, y);

            if (distanceForX != distanceForY)
            {
                return distanceForX.CompareTo(distanceForY);
            }

            if (x.PossibleDamage != y.PossibleDamage)
            {
                return -x.PossibleDamage.CompareTo(y.PossibleDamage);
            }

            int priorityX = _priorities[x.ZombieTypeEnum];
            int priorityY = _priorities[y.ZombieTypeEnum];

            if (priorityX != priorityY) 
            {
                return priorityX.CompareTo(priorityY);
            }

            return 0;
        }
    }
}
