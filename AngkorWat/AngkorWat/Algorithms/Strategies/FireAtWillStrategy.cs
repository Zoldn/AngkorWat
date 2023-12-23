using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.Strategies
{
    internal class FireAtWillStrategy : IShipStrategy
    {
        public FireAtWillStrategy() { }
        public void UpdateCommands(Data data, List<ShipCommand> commands)
        {
            foreach (var command in commands)
            {
                var ship = command.Ship;
                /// На кулдауне, пока не можем стрелять
                if (ship.CannonCooldownLeft > 0)
                {
                    continue;
                }

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

        public static Position PredictPositionAfterMovement(Ship ship)
        {
            int futureX = ship.X;
            int futureY = ship.Y;

            switch (ship.Direction)
            {
                case Directions.NORTH:
                    futureY -= ship.Y;
                    break;
                case Directions.SOUTH:
                    futureY += ship.Y;
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
}
