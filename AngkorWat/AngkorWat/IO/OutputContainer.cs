using AngkorWat.Tower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class OutputContainer
    {
        public string towerId { get; set; }
        public List<JsonPoint> letters { get; set; }
        public OutputContainer(string towerName)
        {
            letters = new();
            towerId = towerName;
        }
    }
}
