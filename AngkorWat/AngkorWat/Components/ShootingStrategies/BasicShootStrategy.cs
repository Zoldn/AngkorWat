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
                if (!ShootTools.IsZombieInFireRange(worldState, zombie))
                {
                    continue;
                }

                potentialZombies.Add(zombie);
            }

            foreach (var zombie in potentialZombies)
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
                BaseTile? shooter = null;

                foreach (var baseTile in worldState.DynamicWorld.Base)
                {
                    if (!baseTile.IsReadyToShoot)
                    {
                        continue;
                    }

                    if (ShootTools.GetDistanceFromBaseToEnemy(baseTile, enemy) > baseTile.Range)
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
    }
}
