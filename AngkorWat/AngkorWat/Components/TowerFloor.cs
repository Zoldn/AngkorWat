using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal enum WordDirection
    {
        X,
        Y,
        Z,
    }

    internal class TowerWord
    {
        public string Word { get; set; }
        public int X0 { get; set; } 
        public int Y0 { get; set; }
        public int Z0 { get; set; }
        public WordDirection Direction { get; set; }
        /// <summary>
        /// Пересечения с другими словами
        /// </summary>
        public List<TowerWord> Overlaps { get; set; }
        public int MinZ 
        {
            get
            {
                return Direction == WordDirection.Z ? Z0 - (Word.Length - 1) : Z0;
            }
        }

        public TowerWord(string word, WordDirection direction, int x = 0, int y = 0, int z = 0)
        {
            Word = word;
            X0 = x;
            Y0 = y;
            Z0 = z;
            Direction = direction;

            Overlaps = new ();
        }

        public override string ToString()
        {
            return $"{Word} in ({X0}, {Y0}, {Z0}) for {Direction})";
        }
    }


}
