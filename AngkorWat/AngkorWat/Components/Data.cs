using AngkorWat.Algorithms;
using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    /// <summary>
    /// Внутреннее представление входных данных
    /// </summary>
    public class Data
    {
        public Map Map { get; set; }
        public Scan CurrentScan { get; set; } = new Scan();

        public Data(Map map)
        {
            Map = map;
        }

        public void RemoveAllShips()
        {
            for (int x = 0; x < Map.SizeX; x++)
            {
                for (int y = 0; y < Map.SizeY; y++)
                {
                    if (Map.Tiles[y, x] == TileStatuses.ENEMY_SHIP
                        || Map.Tiles[y, x] == TileStatuses.ENEMY_SHIP_PIVOT
                        || Map.Tiles[y, x] == TileStatuses.MY_SHIP
                        || Map.Tiles[y, x] == TileStatuses.MY_SHIP_PIVOT
                        )
                    {
                        Map.Tiles[y, x] = TileStatuses.SEA;
                    }
                }
            }
        }

        public void UploadShipsToMap()
        {
            foreach (var ship in CurrentScan.MyShips)
            {
                PrintShipOnMap(ship, TileStatuses.MY_SHIP_PIVOT, TileStatuses.MY_SHIP);
            }

            foreach (var ship in CurrentScan.EnemyShips)
            {
                PrintShipOnMap(ship, TileStatuses.ENEMY_SHIP_PIVOT, TileStatuses.ENEMY_SHIP);
            }
        }

        public void PrintShipOnMap(Ship ship, double pivotType, double type)
        {
            Position position = new Position(ship.X, ship.Y);

            for (int i = 0; i < ship.Size; i++)
            {
                Map.Tiles[position.Y, position.X] = i == 0 ? pivotType : type;

                switch (ship.Direction)
                {
                    case Directions.NORTH:
                        position.Y--;
                        break;
                    case Directions.EAST:
                        position.X++;
                        break;
                    case Directions.SOUTH:
                        position.Y++;
                        break;
                    case Directions.WEST:
                        position.X--;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
