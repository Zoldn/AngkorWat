using AngkorWat.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms.CBrewer.Components
{
    internal class AvailableColorRecord
    {
        public Color Color { get; private set; }
        public int R { get; private set; }
        public int G { get; private set; }
        public int B { get; private set; }
        public int Amount { get; set; }
        public int ColorCode { get; }
        public AvailableColorRecord(int colorCode, int amount)
        {
            ColorCode = colorCode;

            Color = ColorHelper.CodeToRGB(colorCode);
            R = Color.R;
            G = Color.G;
            B = Color.B;

            Amount = amount;
        }
    }
}
