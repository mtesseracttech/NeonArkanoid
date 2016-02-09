using System;
using System.Drawing;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;


namespace NeonArkanoid.Physics
{
    public class Box : Canvas
    {
        private NeonArkanoidGame _neon;
        private Color _boxCollor;

        private int _height;
        private int _width;
        private float _xSpeed = 4f;
        private float _ySpeed = 4f;
        
        private void Update()
        {
            movement();
            boundaries();
        }

        public Box(NeonArkanoidGame pNeon, int pWidth, int pHeight, Color pColor) : base(pWidth, pHeight) 
        {

            _boxCollor = pColor;
            _neon = pNeon;
            _height = pHeight;
            _width = pWidth;
            SetOrigin(_width, _height);


        }

        private void draw()
        {
            graphics.FillEllipse(
                new SolidBrush(_boxCollor),
                0, 0, 2 * _width, 2 * _height);
        }

        private void boundaries()
        {
            y = Utils.Clamp(y, height / 2, game.height - height / 2);
        }

        private void movement()
        {
            if (Input.GetKey(Key.LEFT)) Move(-4, 0);
            if (Input.GetKey(Key.RIGHT)) Move(4, 0);
        }



    }
}
