using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Utility
{
    class ColorUtils
    {
        public static Color UIntToColor(uint input)
        {
            byte a = (byte)(input >> 24);
            byte r = (byte)(input >> 16);
            byte g = (byte)(input >> 8);
            byte b = (byte)(input >> 0);
            return Color.FromArgb(a, r, g, b);
        }

        public static uint ColorToUInt(Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) |
                          (color.G << 8) | (color.B << 0));
        }
    }
}
