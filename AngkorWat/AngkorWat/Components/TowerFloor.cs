using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class HorizontalWord
    {
        public Plank Plank { get; set; }
        public int Shift { get; set; }
        public string Word { get; set; }
        public HorizontalWord(Plank plank, int shift, string word)
        {
            Plank = plank;
            Shift = shift;
            Word = word;
        }
        public override string ToString()
        {
            return $"Plank {Plank.PlankId}, shift {Shift}: {Word}";
        }
    }

    internal class VerticalWord
    {
        public Plank Plank { get; set; }
        public int Shift { get; set; }
        public string Word { get; set; }
        public VerticalWord(Plank plank, int shift, string word)
        {
            Plank = plank;
            Shift = shift;
            Word = word;
        }
        public override string ToString()
        {
            return $"Plank {Plank.PlankId}, shift {Shift}: {Word}";
        }
    }

    internal class TowerFloor
    {
        public bool IsFirst { get; set; }
        /// <summary>
        /// Сдвиг слова относительно первой точки Планки
        /// </summary>
        public Dictionary<Plank, HorizontalWord> PlankWords { get; set; }
        /// <summary>
        /// Положение вертикального слова на планке относительно стартовой точки
        /// </summary>
        public Dictionary<Plank, VerticalWord> ColumnWords { get; set; }
        public TowerFloor()
        {
            IsFirst = true;
            PlankWords = new();
            ColumnWords = new();
        }

        public override string ToString()
        {
            return $"PlankWords: {string.Join(',', PlankWords.Select(e => e.Value.Word))}\n" +
                $"ColumnWords: {string.Join(',', ColumnWords.Select(e => e.Value.Word))}";
        }

        internal int GetPlankMass()
        {
            if (PlankWords.Count == 0)
            {
                return 0;
            }

            return PlankWords.Sum(kv => kv.Value.Word.Length) - (PlankWords.Count - 1);
        }
    }
}
