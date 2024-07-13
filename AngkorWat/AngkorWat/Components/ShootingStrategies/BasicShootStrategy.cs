using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.ShootingStrategies
{
    internal class BasicShootStrategy : IShootStrategy
    {
        public BasicShootStrategy() { }
        public void AddCommand(WorldState worldState)
        {
            var potentialZombies = new List<Zombie>();

            foreach (var zombie in worldState.DynamicWorld.Zombies)
            {
                if (!IsZombieInFireRange(worldState, zombie))
                {
                    continue;
                }

                potentialZombies.Add(zombie);
            }

            foreach (var zombie in potentialZombies)
            {
                BaseTile? shooter = null;

                foreach (var baseTile in worldState.DynamicWorld.Base)
                {
                    if (!baseTile.IsReadyToShoot)
                    {
                        continue;
                    }

                    if (GetDistanceFromBaseToZombie(baseTile, zombie) > baseTile.Range)
                    {
                        continue;
                    }

                    shooter = baseTile;
                    baseTile.IsReadyToShoot = false;
                    break;
                }

                if (shooter is null)
                {
                    continue;
                }

                worldState.TurnCommand.ShootCommands.Add(
                    new ShootCommand() 
                    {
                        BlockId = shooter.Id,
                        Target = new Coordinate()
                        {
                            X = zombie.X,
                            Y = zombie.Y,
                        }
                    }
                    );
            }

            var potentialEnemyBases = new List<EnemyBaseTile>();

            foreach (var enemyBase in worldState.DynamicWorld.EnemyBases)
            {
                if (!IsEnemyBaseInFireRange(worldState, enemyBase))
                {
                    continue;
                }

                potentialEnemyBases.Add(enemyBase);
            }

            foreach (var enemy in potentialEnemyBases)
            {
                BaseTile? shooter = null;

                foreach (var baseTile in worldState.DynamicWorld.Base)
                {
                    if (!baseTile.IsReadyToShoot)
                    {
                        continue;
                    }

                    if (GetDistanceFromBaseToEnemy(baseTile, enemy) > baseTile.Range)
                    {
                        continue;
                    }

                    shooter = baseTile;
                    baseTile.IsReadyToShoot = false;
                    break;
                }

                if (shooter is null)
                {
                    continue;
                }

                worldState.TurnCommand.ShootCommands.Add(
                    new ShootCommand()
                    {
                        BlockId = shooter.Id,
                        Target = new Coordinate()
                        {
                            X = enemy.X,
                            Y = enemy.Y,
                        }
                    }
                    );
            }
        }

        public static double GetDistanceFromBaseToZombie(BaseTile baseTile, Zombie zombie)
        {
            return Math.Sqrt((baseTile.X - zombie.X) * (baseTile.X - zombie.X) +
                (baseTile.Y - zombie.Y) * (baseTile.Y - zombie.Y)
                );
        }

        public static double GetDistanceFromBaseToEnemy(BaseTile baseTile, EnemyBaseTile enemy)
        {
            return Math.Sqrt((baseTile.X - enemy.X) * (baseTile.X - enemy.X) +
                (baseTile.Y - enemy.Y) * (baseTile.Y - enemy.Y)
                );
        }

        public static double GetDistanceFromBaseToCoordinate(BaseTile baseTile, int x, int y)
        {
            return Math.Sqrt((baseTile.X - x) * (baseTile.X - x) +
                (baseTile.Y - y) * (baseTile.Y - y)
                );
        }

        private bool IsZombieInFireRange(WorldState worldState, Zombie zombie)
        {
            foreach (var baseTile in worldState.DynamicWorld.Base)
            {
                if (GetDistanceFromBaseToZombie(baseTile, zombie) > baseTile.Range)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private bool IsEnemyBaseInFireRange(WorldState worldState, EnemyBaseTile enemy)
        {
            foreach (var baseTile in worldState.DynamicWorld.Base)
            {
                if (GetDistanceFromBaseToEnemy(baseTile, enemy) > baseTile.Range)
                {
                    continue;
                }

                return true;
            }

            return false;
        }
    }
}
