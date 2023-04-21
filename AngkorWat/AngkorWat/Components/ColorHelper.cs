using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngkorWat.Components
{
    internal static class ColorHelper
    {
        public static Color CodeToRGB(int color)
        {
            return Color.FromArgb(color / 256 / 256, color / 256 % 256, color % 256);
        }
    }
}
