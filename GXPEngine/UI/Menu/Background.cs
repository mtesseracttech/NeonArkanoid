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
                float g = Mathf.Abs(Mathf.Cos(Time.time / 1000.0f));
                float b = Mathf.Abs(Mathf.Cos(Time.time / 1900.0f));
                color = (uint)(((int)(r * 300)) << 16 | ((int)(g * 300)) << 8 | ((int)(b * 300)));
                speed += 1.0f;
            }
        }
    }
}
