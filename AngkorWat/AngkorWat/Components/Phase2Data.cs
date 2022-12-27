using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class Phase2Data
    {
        public int MaxCOST => 100000;
        public List<Phase2Child> Children { get; set; }
        public List<Phase2Gift> Gifts { get; set; }

        public Phase2Data()
        {
            Children = new List<Phase2Child>();
            Gifts = new List<Phase2Gift>();
        }
    }
}
