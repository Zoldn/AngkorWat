using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class TowerProject
    {
        public List<TowerFloor> Floors { get; set; }
        public HashSet<string> UsedWords { get; set; }
        public TowerProject()
        {
            Floors = new List<TowerFloor>();
            UsedWords = new();
        }

        internal int Height
        {
            get
            {
                int height = 0;

                if (Floors.Count == 1)
                {
                    return Floors[0].ColumnWords.First().Value.Word.Length;
                }

                height = 1;

                foreach (var floor in Floors)
                {
                    height += floor.ColumnWords.First().Value.Word.Length - 1;
                }

                return height;
            }
        }

        internal int GetTotalMass()
        {
            int mass = 0;

            for (int i = 0; i < Floors.Count; i++)
            {
                if (i == 0)
                {
                    mass += Floors[i].ColumnWords.Sum(w => w.Value.Word.Length);
                }
                else
                {
                    mass += Floors[i].PlankWords.Sum(w => w.Value.Word.Length) - Floors[i].PlankWords.Count + 1;
                    mass -= Floors[i - 1].ColumnWords.Count;
                    mass += Floors[i].ColumnWords.Sum(w => w.Value.Word.Length - 1);
                }
            }

            return mass;
        }
    }
}
