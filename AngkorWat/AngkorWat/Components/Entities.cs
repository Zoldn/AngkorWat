using AngkorWat.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    /// <summary>
    /// Выстрел из катапульты
    /// </summary>
    internal class Shot
    {
        public Dictionary<int, int> ColorCodes { get; set; }
        public double VAngle { get; set; }
        public double HAngle { get; set; }
        public double Power { get; set; }
        public Shot()
        {
            ColorCodes = new();
            VAngle = 0.0d;
            HAngle = 0.0d;
            Power = 0.0d;
        }
    }
}
