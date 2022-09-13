using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            //AddWordUp(tower, "интернационализирование", 10, 0, 0);
            //AddWordUp(tower, "переосвидетельствование", 12, 0, 0);
            AddWordUp(tower, "человеконенавистничество", 14, 0, 0);

            return tower;
        }

        private void AddWordUp(Tower tower, string word, int x, int y, int z)
        {
            Words[word] = true;

            tower.UsedWords.Add(word);

            for (int i = 0; i < word.Length; i++)
            {
                tower.Points.Add(new Point() 
                {
                    X = x,
                    Y = y,
                    Z = z + i,
                    C = word[i],
                });
            }
        }

        private void AddWordRight(Tower tower, string word, int x, int y, int z)
        {
            Words[word] = true;

            tower.UsedWords.Add(word);

            for (int i = 0; i < word.Length; i++)
            {
                tower.Points.Add(new Point()
                {
                    X = x,
                    Y = y + i,
                    Z = z,
                    C = word[i],
                });
            }
        }
    }
}
