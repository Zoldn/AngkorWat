using AngkorWat.Components;
using AngkorWat.Components.ShootingStrategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms
{
    internal class ZombieTurnPredictor
    {
        private List<(int X, int Y)> _knightShifts = new(8)
        {
            (2, 1),
            (2, -1),
            (1, 2), 
            (-1, 2),
            (-2, 1),
            (-2, -1),
            (1, -2),
            (-1, -2),
        };


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
                zombie.PossibleDamage = 0;
                RunZombie(zombie, worldState, current, next);
            }

            return next;
        }
        public void NormalHandler(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next) 
        {
            var speedVector = DirectionHelper.GetShiftForDirection(zombie.DirectionEnum);

            for (var step = 0; step < zombie.Speed; step++) 
            {
                int futureX = zombie.X + speedVector.X * step;
                int futureY = zombie.Y + speedVector.Y * step;

                if (world.StaticWorld.ZPotsDict.TryGetValue((futureX, futureY), out var wall))
                {
                    /// Врезались в стену, не копируемся
                    return;
                }

                if (next.EnemyBasesDict.TryGetValue((futureX, futureY), out var enemyBase))
                {
                    DamageEnemyBase(next, enemyBase, zombie);
                    /// Куснули базу, умерли
                    return;
                }

                if (next.BaseTileDict.TryGetValue((futureX, futureY), out var baseTile))
                {
                    DamageBase(next, baseTile, zombie);
                    /// Куснули базу, умерли
                    return;
                }
            }

            int finalX = zombie.X + speedVector.X * zombie.Speed;
            int finalY = zombie.Y + speedVector.Y * zombie.Speed;

            KeepZombieAlive(zombie, current, next, finalX, finalY);
        }

        public void FastHandler(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next) 
        {
            NormalHandler(zombie, world, current, next);
        }

        public void BomberHandler(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next)
        {
            var speedVector = DirectionHelper.GetShiftForDirection(zombie.DirectionEnum);

            for (var step = 0; step < zombie.Speed; step++)
            {
                int futureX = zombie.X + speedVector.X * step;
                int futureY = zombie.Y + speedVector.Y * step;

                if (world.StaticWorld.ZPotsDict.TryGetValue((futureX, futureY), out var wall))
                {
                    /// Врезались в стену, не копируемся
                    return;
                }

                if (next.EnemyBasesDict.TryGetValue((futureX, futureY), out var enemyBase))
                {
                    for (var xs = -1; xs <= 1; xs++) 
                    {
                        for (var ys = -1; ys <= 1; ys++)
                        {
                            if (next.EnemyBasesDict.TryGetValue((futureX + xs, futureY + ys), out var curEnemyBase))
                            {
                                DamageEnemyBase(next, curEnemyBase, zombie);
                            }
                        }
                    }

                    /// Куснули базу, умерли
                    return;
                }

                if (next.BaseTileDict.TryGetValue((futureX, futureY), out var baseTile))
                {
                    for (var xs = -1; xs <= 1; xs++)
                    {
                        for (var ys = -1; ys <= 1; ys++)
                        {
                            if (next.BaseTileDict.TryGetValue((futureX + xs, futureY + ys), out var curEnemyBase))
                            {
                                DamageBase(next, curEnemyBase, zombie);
                            }
                        }
                    }

                    /// Куснули базу, умерли
                    return;
                }
            }

            int finalX = zombie.X + speedVector.X * zombie.Speed;
            int finalY = zombie.Y + speedVector.Y * zombie.Speed;

            KeepZombieAlive(zombie, current, next, finalX, finalY);
        }

        private void DamageBase(DynamicWorld next, BaseTile baseTile, Zombie zombie)
        {
            baseTile.Health -= zombie.Attack;

            zombie.PossibleDamage += zombie.Attack;

            if (baseTile.Health < 0)
            {
                next.BaseTileDict.Remove((baseTile.X, baseTile.Y));
                next.Base.Remove(baseTile);
            }
        }

        private void DamageEnemyBase(DynamicWorld next, EnemyBaseTile curEnemyBase, Zombie zombie)
        {
            curEnemyBase.Health -= zombie.Attack;

            if (curEnemyBase.Health < 0)
            {
                next.EnemyBasesDict.Remove((curEnemyBase.X, curEnemyBase.Y));
                next.EnemyBases.Remove(curEnemyBase);
            }
        }

        public void LineHandler(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next)
        {
            var speedVector = DirectionHelper.GetShiftForDirection(zombie.DirectionEnum);

            for (var step = 0; step < zombie.Speed; step++)
            {
                int futureX = zombie.X + speedVector.X * step;
                int futureY = zombie.Y + speedVector.Y * step;

                if (world.StaticWorld.ZPotsDict.TryGetValue((futureX, futureY), out var wall))
                {
                    /// Врезались в стену, не копируемся
                    return;
                }

                if (next.EnemyBasesDict.TryGetValue((futureX, futureY), out var enemyBase))
                {
                    int s = 0;

                    while (true)
                    {
                        int damageX = futureX + speedVector.X * s;
                        int damageY = futureY + speedVector.Y * s;

                        if (!next.EnemyBasesDict.TryGetValue((damageX, damageY), out var curEnemyBase))
                        {
                            break;
                        }

                        DamageEnemyBase(next, curEnemyBase, zombie);
                        s++;
                    }

                    /// Куснули базу, умерли
                    return;
                }

                if (next.BaseTileDict.TryGetValue((futureX, futureY), out var baseTile))
                {
                    int s = 0;

                    while (true)
                    {
                        int damageX = futureX + speedVector.X * s;
                        int damageY = futureY + speedVector.Y * s;

                        if (!next.BaseTileDict.TryGetValue((damageX, damageY), out var curEnemyBase))
                        {
                            break;
                        }

                        DamageBase(next, curEnemyBase, zombie);
                        s++;
                    }

                    /// Куснули базу, умерли
                    return;
                }
            }

            int finalX = zombie.X + speedVector.X * zombie.Speed;
            int finalY = zombie.Y + speedVector.Y * zombie.Speed;

            KeepZombieAlive(zombie, current, next, finalX, finalY);
        }

        public void JaggernautHandler(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next)
        {
            var speedVector = DirectionHelper.GetShiftForDirection(zombie.DirectionEnum);

            for (var step = 0; step < zombie.Speed; step++)
            {
                int futureX = zombie.X + speedVector.X * step;
                int futureY = zombie.Y + speedVector.Y * step;

                if (world.StaticWorld.ZPotsDict.TryGetValue((futureX, futureY), out var wall))
                {
                    /// Врезались в стену, не копируемся
                    return;
                }

                if (next.EnemyBasesDict.TryGetValue((futureX, futureY), out var enemyBase))
                {
                    DamageEnemyBase(next, enemyBase, zombie);
                }

                if (next.BaseTileDict.TryGetValue((futureX, futureY), out var baseTile))
                {
                    DamageBase(next, baseTile, zombie);
                }
            }

            int finalX = zombie.X + speedVector.X * zombie.Speed;
            int finalY = zombie.Y + speedVector.Y * zombie.Speed;

            KeepZombieAlive(zombie, current, next, finalX, finalY);
        }

        public void ChaosKnightHandler(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next)
        {
            if (!current.TryGetBaseCenter(out var baseCenter))
            {
                return;
            }

            var shift = _knightShifts
                .OrderBy(s => ShootTools.GetDistanceFromBaseToCoordinate(baseCenter, 
                    zombie.X + s.X, zombie.Y + s.Y))
                .FirstOrDefault();

            if (shift.X == 0 || shift.Y == 0)
            {
                return;
            }

            int futureX = zombie.X + shift.X;
            int futureY = zombie.Y + shift.Y;

            if (world.StaticWorld.ZPotsDict.TryGetValue((futureX, futureY), out var wall))
            {
                /// Врезались в стену, не копируемся
                return;
            }

            if (next.EnemyBasesDict.TryGetValue((futureX, futureY), out var enemyBase))
            {
                DamageEnemyBase(next, enemyBase, zombie);
            }

            if (next.BaseTileDict.TryGetValue((futureX, futureY), out var baseTile))
            {
                DamageBase(next, baseTile, zombie);
            }

            KeepZombieAlive(zombie, current, next, futureX, futureY);
        }

        public void RunZombie(Zombie zombie, WorldState world, DynamicWorld current, DynamicWorld next)
        {
            if (IsZombieWaiting(zombie, current, next))
            {
                return;
            }

            switch (zombie.ZombieTypeEnum)
            {
                case ZombieType.Normal:
                    NormalHandler(zombie, world, current, next);
                    break;
                case ZombieType.Fast:
                    FastHandler(zombie, world, current, next);
                    break;
                case ZombieType.Bomber:
                    BomberHandler(zombie, world, current, next);
                    break;
                case ZombieType.Liner:
                    LineHandler(zombie, world, current, next);
                    break;
                case ZombieType.Jaggernaut:
                    JaggernautHandler(zombie, world, current, next);
                    break;
                case ZombieType.ChaosKnight:
                    ChaosKnightHandler(zombie, world, current, next);
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

        private void KeepZombieAlive(Zombie zombie, DynamicWorld current, DynamicWorld next,
            int x, int y)
        {
            var futureZombie = new Zombie(zombie)
            {
                WaitTurns = 1,
                X = x,
                Y = y,
            };

            next.Zombies.Add(futureZombie);
        }
    }
}
