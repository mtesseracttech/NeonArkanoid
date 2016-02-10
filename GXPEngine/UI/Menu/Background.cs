using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using System.Drawing;

namespace GXPEngine.UI.Menu
{
     public class Background : Canvas
    {
        public Background(int width, int height, Color color) : base(width, height)
        {
            graphics.FillRectangle(new SolidBrush(color), new Rectangle(0, 0, width, height));
        }

        private void drawBackground()
        {
            
        }
    }
}
