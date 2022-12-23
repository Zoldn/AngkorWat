using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal class AllData
    {
        public List<Child> Children { get; set; }
        public List<SnowArea> SnowAreas { get; set; }
        public List<Gift> Gifts { get; set; }
        public AllData()
        {
            Children = new();
            SnowAreas = new();
            Gifts = new();
        }
    }
}
