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
        public OutputContainer(string towerName)
        {
            towerId = towerName;
        }
    }
}
