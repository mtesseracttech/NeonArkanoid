﻿using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using System.Drawing;

namespace NeonArkanoid.UI.Menu
{
    public class Background : Sprite
    {
        private bool _flicker;

        public Background(string filename, bool flicker) : base(filename)
        {
            SetOrigin(game.width, game.height);
            SetXY(width, height);
            _flicker = flicker;
        }

        private void Update()
        {
            
            if (_flicker)
            {
                float r = Mathf.Abs(Mathf.Sin(Time.time / 700));
                float g = Mathf.Abs(Mathf.Sin(Time.time / 800));
                float b = Mathf.Abs(Mathf.Sin(Time.time / 900));
                color = (uint)(((int)(r * 255)) << 16 | ((int)(g * 255)) << 8 | ((int)(b * 255)));
            }
        }
    }
}
