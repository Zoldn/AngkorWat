using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Algorithms
{
    internal static class ColorUtils
    {
        public static int ColorDiffL1(Color color1, Color color2)
        {
            return Math.Abs(color1.R - color2.R) 
                + Math.Abs(color1.G - color2.G)
                + Math.Abs(color1.B - color2.B);
        }

        public static int ColorDiffL0(Color color1, Color color2)
        {
            return Math.Max(
                Math.Max(Math.Abs(color1.R - color2.R), Math.Abs(color1.G - color2.G)),
                Math.Abs(color1.B - color2.B)
                );
        }
    }
}
