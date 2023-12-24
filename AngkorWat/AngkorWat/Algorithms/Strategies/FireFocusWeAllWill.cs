using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    public class FireFocusWeAllWill : IShipStrategy
    {
        public FireFocusWeAllWill() { }
        public void UpdateCommands(Data data, List<ShipCommand> commands)
        {
            var KillRooms = new List<ShipsKillroom>();
            foreach (var enemyShip in data.CurrentScan.EnemyShips)
            {
                KillRooms.Add(new ShipsKillroom(enemyShip, commands));
            }

            // Делаем список готовых стрелять кораблей
            var noComandsShipsList = new List<ShipCommand>();
            foreach (var command in commands)
            {
                if (command.Ship.CannonCooldownLeft == 0)
                {
                    noComandsShipsList.Add(command);
                }
            }


            // Пока у нас есть корабли без команд стрельбы
            while (noComandsShipsList.Count > 0 && KillRooms.Count > 0) 
            {
                // Пересчитываем возможные цели
                foreach (var killroom in KillRooms)
                {
                    killroom.RecalcOwnShips(noComandsShipsList);
                }

                // Удаляем тех, по кому не попасть
                for (var i = KillRooms.Count - 1; i >= 0; --i)
                {
                    if (KillRooms[i].ownShipsInRange.Count == 0)
                    {
                        KillRooms.Remove(KillRooms[i]);
                    }
                }

                // Находим лучшую цель 
                KillRooms.Sort(ShipsKillroom.KillroomComparison);
                if (KillRooms.Count > 0)
                {
                    var killroom = KillRooms[0]; // лучшая цель

                    // Вносим максимум урона в центр равномерного движения
                    for (var hpLeft = killroom.enemyShip.HP; hpLeft >= 0; --hpLeft)
                    {
                        if (killroom.ownShipsInRange.Count > 0)
                        {
                            AddShot(killroom.ownShipsInRange[0], killroom.shotsPositions[0], noComandsShipsList);
                            killroom.ownShipsInRange.Remove(killroom.ownShipsInRange[0]);
                        }
                    }

                    // Остальные возможные корабли стреляют в возможные точки равномерно размазывая урон
                    for (var hpLeft = killroom.enemyShip.HP; hpLeft >= 0; --hpLeft)
                    {
                        for (var i = 1; i < killroom.shotsPositions.Count; i++)
                        {
                            if (killroom.ownShipsInRange.Count > 0)
                            {
                                AddShot(killroom.ownShipsInRange[0], killroom.shotsPositions[i], noComandsShipsList);
                                killroom.ownShipsInRange.Remove(killroom.ownShipsInRange[0]);
                            }
                        }
                    }
                }
            }

            // Если кто-то не стреляет, но кд < 3, то пусть всё равно пальнёт
            foreach (var command in noComandsShipsList)
            {
                var ship = command.Ship;

                if (ship.CannonCooldown < 3)
                {
                    foreach (var enemyShip in data.CurrentScan.EnemyShips)
                    {
                        var futureEnemyPosition = PredictPositionAfterMovement(enemyShip);

                        /// Слишком далеко
                        if (GeometryUtils.GetDistance(futureEnemyPosition, ship) > ship.CannonRadius)
                        {
                            continue;
                        }

                        Console.WriteLine($"Ship {ship.ShipId} has detected enemy and fire at " +
                            $"({futureEnemyPosition.X}, {futureEnemyPosition.Y})");

                        command.Shoot = new CannonShoot()
                        {
                            X = futureEnemyPosition.X,
                            Y = futureEnemyPosition.Y,
                        };

                        break;
                    }
                }
            }
        }

        // Добавляем команду выстрела кораблю и удаляем корабль из списока "не отданых команд"
        public static void AddShot(Ship ownShip, Position target, List<ShipCommand> noComandsShipsList)
        {
            foreach (var command in noComandsShipsList)
            {
                // Нашли команду для нужного корабля
                if (command.ShipId == ownShip.ShipId)
                {
                    // добавили команду стрелять
                    command.Shoot = new CannonShoot()
                    {
                        X = (int)target.X,
                        Y = (int)target.Y,
                    };

                    Console.WriteLine($"Ship {ownShip.ShipId} has detected enemy and fire at " +
                        $"({target.X}, {target.Y})");
                    // удалили корабль из списка "без команды"
                    noComandsShipsList.Remove(command);
                }
            }
        }

        public static Position PredictPositionAfterMovement(Ship ship)
        {
            int futureX = ship.X;
            int futureY = ship.Y;

            switch (ship.Direction)
            {
                case Directions.NORTH:
                    futureY -= ship.Speed;
                    break;
                case Directions.SOUTH:
                    futureY += ship.Speed;
                    break;
                case Directions.EAST:
                    futureX += ship.Speed;
                    break;
                case Directions.WEST:
                    futureX -= ship.Speed;
                    break;
                default:
                    break;
            }

            return new Position(futureX, futureY);
        }
    }

    public class ShipsKillroom
    {
        public Ship enemyShip;
        public List<Ship> ownShipsInRange;
        public float hitProbability;
        public float killProbability;
        public int shotsToKillThisRound;
        public List<Position> shotsPositions;
        public ShipsKillroom(Ship enemyShipEntered, List<ShipCommand> commands)
        {
            enemyShip = enemyShipEntered;

            // считаем hitProbability как 1 / (количество возможных позиций кормы оппонента). Не учитываем врезания или острова
            var possibleSpeedMax = Math.Min(enemyShip.Speed + enemyShip.MaxChangeSpeed, enemyShip.MaxSpeed);
            var possibleSpeedMin = Math.Max(enemyShip.Speed - enemyShip.MaxChangeSpeed, enemyShip.MinSpeed);
            hitProbability = 1 / (possibleSpeedMax - possibleSpeedMin);
            ownShipsInRange = new List<Ship> { };
            shotsPositions = new List<Position>();


            var futureEnemyPosition = FireAtWillStrategy.PredictPositionAfterMovement(enemyShip);

            if (hitProbability == 1)
            {
                // Если враг не играется со скоростью, то мы точно знаем куда стрелять.
                shotsPositions.Add(new Position(futureEnemyPosition.X, futureEnemyPosition.Y));
            } else
            {
                // Делаем массив возможных точек куда стрелять, чтобы попасть. От точки перед самым заторможенным шагом с шагом 3.
                for (var move = possibleSpeedMax - 1; move <= possibleSpeedMin; move -= 3)
                {
                    switch (enemyShip.Direction)
                    {
                        case Directions.NORTH:
                            shotsPositions.Add(new Position(enemyShip.X, enemyShip.Y - move));
                            break;
                        case Directions.SOUTH:
                            shotsPositions.Add(new Position(enemyShip.X, enemyShip.Y + move));
                            break;
                        case Directions.EAST:
                            shotsPositions.Add(new Position(enemyShip.X + move, enemyShip.Y));
                            break;
                        case Directions.WEST:
                            shotsPositions.Add(new Position(enemyShip.X - move, enemyShip.Y - move));
                            break;
                        default:
                            break;
                    }
                }

                // Сортируем возможные точки куда стрелать. В приоритете предполагаем что враг не ускоряется.
                shotsPositions.Sort(delegate (Position v1, Position v2)
                {
                    var shipPosition = new Vector2(futureEnemyPosition.X, futureEnemyPosition.Y);
                    var v1dist = Vector2.Distance(new Vector2(v1.X, v1.Y), shipPosition);
                    var v2dist = Vector2.Distance(new Vector2(v2.X, v2.Y), shipPosition);

                    return (int)Math.Round(v1dist - v2dist);
                });
            }

            RecalcOwnShips(commands);
        }

        public void RecalcOwnShips(List<ShipCommand> commands)
        {
            ownShipsInRange = new List<Ship> { };
            shotsPositions = new List<Position>();
            var futureEnemyPosition = FireAtWillStrategy.PredictPositionAfterMovement(enemyShip);

            foreach (var command in commands)
            {
                var ownShip = command.Ship;

                /// На кулдауне, пока не можем стрелять
                if (ownShip.CannonCooldownLeft > 0)
                {
                    continue;
                }

                /// В ренже - добавляем в список
                if (GeometryUtils.GetDistance(futureEnemyPosition, ownShip) < ownShip.CannonRadius)
                {
                    ownShipsInRange.Add(ownShip);
                }
            }

            /// Сортируем наши корабли по дальности, начиная с самых недальнобойных (они стреляют в приоритете)
            ownShipsInRange.Sort(delegate (Ship x, Ship y)
            {
                return -x.CannonRadius + y.CannonRadius;
            });

            /// Считаем killProbability с одного залпа
            killProbability = hitProbability * ownShipsInRange.Count / enemyShip.HP;
            if (killProbability < 1)
            {
                shotsToKillThisRound = ownShipsInRange.Count;
            } else
            {
                shotsToKillThisRound = (int)Math.Ceiling(enemyShip.HP / hitProbability);
            }

        }

        public static int KillroomComparison(ShipsKillroom x, ShipsKillroom y)
        {
            if (x.killProbability > 1)
            {
                if (y.killProbability > 1)
                {
                    return x.ownShipsInRange.Count - y.ownShipsInRange.Count;
                }
                else
                {
                    return 1000 - x.ownShipsInRange.Count;
                }
            }
            else
            {
                if (y.killProbability > 1)
                {
                    return 1000 - y.ownShipsInRange.Count;
                }
                else
                {
                    return x.ownShipsInRange.Count - y.ownShipsInRange.Count;
                }
            }
        }
    }
}
