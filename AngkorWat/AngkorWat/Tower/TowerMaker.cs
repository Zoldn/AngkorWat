using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngkorWat.IO;
using Newtonsoft.Json;

namespace AngkorWat.Tower
{
    internal class TowerMaker
    {
        public Dictionary<string, bool> Words { get; set; }
        public TowerMaker(List<string> words)
        {
            Words = words
                .Select(x => x.ToUpper())
                .Distinct()
                .ToDictionary(
                    x => x,
                    x => false
                );
        }

        internal Tower MakeTower()
        {
            var tower = new Tower();

            //AddWordRight(tower, "СЫРЬЕ".ToUpper(), 29, 0, 22);

            AddWordUp(tower, "интернационализирование".ToUpper(), 30, 0, 0);
            AddWordUp(tower, "переосвидетельствование".ToUpper(), 32, 0, 0);
            AddWordUp(tower, "человеконенавистничество".ToUpper(), 34, 0, 0);

            //var wW = FindHorizontalWord(new Dictionary<int, char>(){
            //    { 0, 'И' },
            //    { 2, 'П' },
            //    { 4, 'Е' },
            //});

            AddWordRight(tower, "ДИСПЛЕЙ".ToUpper(), 29, 0, 22);

            AddWordUp(tower, "ДУПЛЕКС-ПРОЦЕСС", 31, 0, 22);
            AddWordUp(tower, "ТУРБОЭЛЕКТРОХОД", 29, 0, 22);

            //var t = Words
            //    .Where(e => e.Key[e.Key.Length-1] == 'Д' || e.Key[e.Key.Length - 1] == 'С')
            //    .Select(e => e.Key)
            //    .OrderByDescending(e => e.Length)
            //    .ToList();

            var t = Words
                .Where(e => e.Key[e.Key.Length - 1] == 'О')
                .Select(e => e.Key)
                .OrderByDescending(e => e.Length)
                .ToList();

            var w = FindHorizontalWord(new Dictionary<int, char>()
            {
                {0, 'Т' },
                {2, 'Д' },
            });

            AddWordRight(tower, "Истод".ToUpper(), 27, 0, 36);

            AddWordUp(tower, "ПРИСТАНОДЕРЖАТЕЛЬСТВО", 30, 0, 36);

            //AddWordUp(tower, "ЛЮБОСТЯЖАТЕЛЬНИЦА", 30, 0, 41);
            //AddWordUp(tower, "ЦЕНТРИФУГИРОВАНИЕ", 32, 0, 41);

            //AddWordRight(tower, "АВЕНЮ", 30, 0, 58);

            //AddWordUp(tower, "ВОЗДУХОНЕПРОНИЦАЕМОСТЬ", 31, 0, 58);

            AddWordRight(tower, "ПАЙ".ToUpper(), 30, 0, 56);

            AddWordUp(tower, "ПРИТОНОСОДЕРЖАТЕЛЬНИЦА", 31, 0, 56);

            PrintTower(tower);

            tower.UpdatePoints();

            return tower;
        }

        public void PrintTower(Tower tower)
        {
            var minx = tower.NewPoints.Min(kv => kv.Key.Item1);
            var maxx = tower.NewPoints.Max(kv => kv.Key.Item1);

            var minz = tower.NewPoints.Min(kv => kv.Key.Item3);
            var maxz = tower.NewPoints.Max(kv => kv.Key.Item3);

            for (int z = minz; z <= maxz; z++) 
            { 
                for (int x = minx; x <= maxx; x++)     
                {
                    if (tower.NewPoints.TryGetValue((x, 0, z), out var c))
                    {
                        Console.Write(c);
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }

                Console.WriteLine();
            }
        }

        public string FindHorizontalWord(Dictionary<int, char> poses)
        {
            var maxPos = poses.Max(e => e.Key);

            var words = new List<string>();

            foreach (var (word, isUsed) in Words)
            {
                if (isUsed)
                {
                    continue;
                }

                if (word.Length < 5)
                {
                    continue;
                }

                for (int shift = 0; shift < word.Length; shift++)
                {
                    if (shift + maxPos >= word.Length)
                    {
                        continue;
                    }

                    bool isOk = true;

                    foreach (var (pos, c) in poses)
                    {
                        if (word[pos + shift] != c)
                        {
                            isOk = false;
                            break;
                        }
                    }

                    if (isOk)
                    {
                        words.Add(word);
                    }
                }
            }

            return words.OrderBy(e => e.Length).FirstOrDefault();
        }

        private void AddWordUp(Tower tower, string word, int x, int y, int z)
        {
            Words[word] = true;

            tower.UsedWords.Add(word);

            var t = word.Reverse().ToList();

            for (int i = 0; i < t.Count; i++)
            {
                if (tower.NewPoints.ContainsKey((x, y, z + i)))
                {
                    continue;
                }
                else
                {
                    tower.NewPoints.Add((x, y, z + i), t[i]);
                }
                
                //tower.Points.Add(new Point() 
                //{
                //    X = x,
                //    Y = y,
                //    Z = z + i,
                //    C = word[i],
                //});
            }
        }

        private void AddWordRight(Tower tower, string word, int x, int y, int z)
        {
            Words[word] = true;

            tower.UsedWords.Add(word);

            for (int i = 0; i < word.Length; i++)
            {
                //tower.Points.Add(new Point()
                //{
                //    X = x,
                //    Y = y + i,
                //    Z = z,
                //    C = word[i],
                //});

                if (tower.NewPoints.ContainsKey((x + i, y, z)))
                {
                    continue;
                }
                else
                {
                    tower.NewPoints.Add((x + i, y, z), word[i]);
                }
            }
        }
    }
}
