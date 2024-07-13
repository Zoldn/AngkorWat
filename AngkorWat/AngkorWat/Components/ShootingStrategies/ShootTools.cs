using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components.ShootingStrategies
{
    internal static class ShootTools
    {
        internal static void AddShootCommandForTarget(WorldState worldState, Zombie zombie)
        {
            int hpLeft = zombie.Health;

            foreach (var baseTile in worldState.DynamicWorld.Base)
            {
                if (hpLeft <= 0)
                {
                    break;
                }

                if (!baseTile.IsReadyToShoot)
                {
                    continue;
                }

                if (GetDistanceFromBaseToZombie(baseTile, zombie) > baseTile.Range)
                {
                    continue;
                }

                var shooter = baseTile;
                baseTile.IsReadyToShoot = false;
                hpLeft -= baseTile.Attack;

                if (shooter is null)
                {
                    return;
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
        }

        internal static void AddShootCommandForTarget(WorldState worldState, EnemyBaseTile enemyBase)
        {
            BaseTile? shooter = null;
            int baseHP = enemyBase.Health;

            foreach (var baseTile in worldState.DynamicWorld.Base)
            {
                if (baseHP <= 0)
                {
                    break;
                }

                if (!baseTile.IsReadyToShoot)
                {
                    continue;
                }

                if (ShootTools.GetDistanceFromBaseToEnemy(baseTile, enemyBase) > baseTile.Range)
                {
                    continue;
                }

                shooter = baseTile;
                baseTile.IsReadyToShoot = false;
                baseHP -= baseTile.Attack;

                worldState.TurnCommand.ShootCommands.Add(
                    new ShootCommand()
                    {
                        BlockId = shooter.Id,
                        Target = new Coordinate()
                        {
                            X = enemyBase.X,
                            Y = enemyBase.Y,
                        }
                    }
                );
            }
        }

        public static bool IsEnemyBaseInFireRange(WorldState worldState, EnemyBaseTile enemy)
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


        public static bool IsZombieInFireRange(WorldState worldState, Zombie zombie)
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
    }
}
