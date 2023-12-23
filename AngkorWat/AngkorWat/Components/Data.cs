using Google.OrTools.Sat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    /// <summary>
    /// Внутреннее представление входных данных
    /// </summary>
    public class Data
    {
        public Map Map { get; set; }
        public Scan CurrentScan { get; set; } = new Scan();

        public Data(Map map)
        {
            Map = map;
        }
    }
}
