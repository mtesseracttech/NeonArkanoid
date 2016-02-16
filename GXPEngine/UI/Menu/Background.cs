using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using System.Drawing;

namespace NeonArkanoid.UI.Menu
{
    public class Background : Sprite
    {
        private bool _flicker;
        private int _choice;//choice of how fast the speed will be

        public Background(string filename, bool flicker, int choice) : base(filename)
        {
            SetOrigin(game.width, game.height);
            SetXY(width, height);
            _flicker = flicker;
            _choice = choice;
        }

        private void Update()
        {
            
            if (_flicker)
            {
                switch (_choice)
                {
                    case 0:
                        break;
                    case 1:
                        BackgroundFLickering1();
                        break;
                    case 2:
                        BackgroundFlickering2();
                        break;
                }
            }
        }

        private void BackgroundFLickering1()
        {
            float r = Mathf.Abs(Mathf.Sin(Time.time / 1100));
            float g = Mathf.Abs(Mathf.Sin(Time.time / 1500));
            float b = Mathf.Abs(Mathf.Sin(Time.time / 1700));
            color = (uint)(((int)(r * 255)) << 16 | ((int)(g * 255)) << 8 | ((int)(b * 255)));
        }

        private void BackgroundFlickering2()
        {
            float r = Mathf.Abs(Mathf.Sin(Time.time / 100));
            float g = Mathf.Abs(Mathf.Sin(Time.time / 300));
            float b = Mathf.Abs(Mathf.Sin(Time.time / 500));
            color = (uint)(((int)(r * 255)) << 16 | ((int)(g * 255)) << 8 | ((int)(b * 255)));
        }
    }
}
