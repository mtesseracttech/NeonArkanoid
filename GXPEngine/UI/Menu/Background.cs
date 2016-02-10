using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using System.Drawing;

namespace NeonArkanoid.UI.Menu
{
     public class Background : Sprite
    {
        private float speed;
        private bool _flicker;

        public Background(string filename, bool flicker) : base(filename)
        {
            SetOrigin(game.width, game.height);
            SetXY(width, height);
            _flicker = flicker;
        }

        void Update()
        {
            if (_flicker)
            {
                float r = Mathf.Abs(Mathf.Cos(Time.time / 100.0f));
                float g = Mathf.Abs(Mathf.Cos(Time.time / 1100.0f));
                float b = Mathf.Abs(Mathf.Cos(Time.time / 1700.0f));
                color = (uint)(((int)(r * 255)) << 16 | ((int)(g * 255)) << 8 | ((int)(b * 255)));
                speed += 1.0f;
            }
        }
    }
}
