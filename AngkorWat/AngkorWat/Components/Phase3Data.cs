using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Phase3Data
    {
        public int MaxCOST => 100000;
        public List<SnowArea> SnowAreas { get; set; }
        public List<Phase3Child> Children { get; set; }
        public List<Phase3Gift> Gifts { get; set; }

        public Phase3Data()
        {
            Children = new List<Phase3Child>();
            Gifts = new List<Phase3Gift>();
            SnowAreas = new List<SnowArea>();
        }
    }
}
