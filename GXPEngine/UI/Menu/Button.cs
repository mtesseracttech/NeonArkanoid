﻿using NeonArkanoid.GXPEngine;
using NeonArkanoid.Utility;

namespace NeonArkanoid.UI.Menu
{
    internal class Button : AnimationSprite
    {
        private readonly Sound _selectSound;
        private readonly string _state;

        public Button(string filename, int rows, int x, int y, string state) : base(filename, 1, rows)
        {
            DeSelect();
            SetOrigin(game.width/2 - 60, height/2 - 30);
            SetXY(x, y);
            _state = state;

            _selectSound = new Sound(UtilStrings.SoundsMenu + "sound_click.wav");
        }

        public void Selected()
        {
            if (scaleX != 1f)
                SetScaleXY(1f, 1f);
                    //Only made it scan for scaleX because if that one is not right, the other won't be either
            SetFrame(0);
        }

        public void DeSelect()
        {
            if (scaleX != 1f) SetScaleXY(1f, 1f);
            SetFrame(1);
        }


        //Should get called when the button is being pressed, it returns the gamestate to which the game should be set
        public string Pressed()
        {
            _selectSound.Play();
            return _state;
        }
    }
}