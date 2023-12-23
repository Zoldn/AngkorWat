using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    public enum TileStatus
    {
        SEA = 0,
        ISLAND = 1,
        MY_SHIP = 2,
        ENEMY_SHIP = 3,
    }

    public class Map
    {
        public int SizeX { get; set; } = 0;
        public int SizeY { get; set; } = 0;
        public string Slug { get; set; } = string.Empty;
        public TileStatus[, ] Tiles { get; }
        public Map(RawMap rawMap) 
        {
            Tiles = new TileStatus[rawMap.Width, rawMap.Height];
            Slug = rawMap.Slug;

            SizeX = rawMap.Width;
            SizeY = rawMap.Height;

            foreach (var island in rawMap.Islands)
            {
                int y0 = island.PivotPoint[1];
                int x0 = island.PivotPoint[0];

                for (int y = 0; y < island.Map.Count; y++)
                {
                    for (int x = 0; x < island.Map.First().Count; x++)
                    {
                        int xt = x0 + x - 1;
                        int yt = y0 + y - 1;

                        if (yt >= SizeY || xt >= SizeX || yt < 0 || xt < 0)
                        {
                            continue;
                        }

                        if (island.Map[y][x] == 1)
                        {
                            Tiles[xt, yt] = TileStatus.ISLAND;
                        }
                    }
                }
            }
        }

        internal void PrintInfo()
        {
            int islandCount = 0;

            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    if (Tiles[i, j] == TileStatus.ISLAND)
                    {
                        islandCount++;
                    }
                }
            }

            Console.WriteLine($"Island count is {islandCount}");
        }
    }
}
