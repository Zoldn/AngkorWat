using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.IO
{
    internal class DerPunkt
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public DerPunkt() { }
    }

    internal class OutputContainer
    {
        public List<DerPunkt> Punkte { get; set; }
        public OutputContainer() 
        {
            Punkte = new();
        }
    }
}
