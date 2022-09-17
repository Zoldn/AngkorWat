using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class TowerProject
    {
        public List<TowerWord> TowerWords { get; set; }
        public HashSet<string> UsedWords => TowerWords.Select(e => e.Word).ToHashSet();
        public TowerProject()
        {
            TowerWords = new();
        }

        public void CheckMass()
        {
            var tower = this.ToTower();

            if (tower.Points.Count != Mass)
            {
                int y = 1;
            }
        }

        public void CheckDublicateWords()
        {
            var dups = TowerWords
                .GroupBy(e => e.Word)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (dups.Count > 0)
            {
                Console.WriteLine($"Repeatable words");
            }
        }

        public int CheckOverlaps()
        {
            List<(TowerWord, TowerWord)> fails = new();

            foreach (var t1 in TowerWords)
            {
                foreach (var overlap in t1.Overlaps)
                {
                    if (!overlap.Overlaps.Contains(t1))
                    {
                        fails.Add((t1, overlap));
                    }
                }
            }

            if (fails.Count > 0)
            {
                int y = 1;
            }

            return fails.Count;
        }

        internal Tower ToTower()
        {
            var tower = new Tower();

            Dictionary<(int, int, int), int> overlaps = new Dictionary<(int, int, int), int>();

            foreach (var towerWord in TowerWords)
            {
                int x = towerWord.X0;
                int y = towerWord.Y0;
                int z = towerWord.Z0;

                for (int i = 0; i < towerWord.Word.Length; i++)
                {
                    if (!tower.Points.TryGetValue((x, y, z), out var c))
                    {
                        tower.Points.Add((x, y, z), towerWord.Word[i]);
                    }
                    else
                    {
                        if (c != towerWord.Word[i])
                        {
                            throw new Exception();
                        }
                    }

                    if (!overlaps.ContainsKey((x, y, z)))
                    {
                        overlaps.Add((x, y, z), 1);
                    }
                    else
                    {
                        overlaps[(x, y, z)] += 1;
                    }

                    switch (towerWord.Direction)
                    {
                        case WordDirection.X:
                            x += 1;
                            break;
                        case WordDirection.Y:
                            y += 1;
                            break;
                        case WordDirection.Z:
                            z -= 1;
                            break;
                        default:
                            break;
                    }
                }
            }

            int minX = tower.Points.Min(kv => kv.Key.X);
            int minY = tower.Points.Min(kv => kv.Key.Y);
            int minZ = tower.Points.Min(kv => kv.Key.Z);

            tower.Points = tower.Points
                .ToDictionary(
                    kv => (kv.Key.X - minX, kv.Key.Y - minY, kv.Key.Z - minZ),
                    kv => kv.Value
                );

            return tower;
        }

        internal int Height
        {
            get
            {
                int maxHeight = TowerWords.Max(t => t.Z0);
                int minHeight = MinZ;

                return maxHeight - minHeight + 1;
            }
        }

        internal int Mass
        {
            get
            {
                int mass = TowerWords.Sum(t => t.Word.Length);

                int overlaps = TowerWords.Sum(t => t.Overlaps.Count) / 2;

                return mass - overlaps;
            }
        }

        public int MinZ => TowerWords.Min(t => t.MinZ);
    }
}
