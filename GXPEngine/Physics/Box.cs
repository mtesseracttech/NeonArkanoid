using System;
using System.Drawing;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;


namespace NeonArkanoid.Physics
{
    public class Box : Canvas
    {
        private NeonArkanoidGame _game;
        private Color _boxCollor;

        private readonly int _height;
        private readonly int _width;
        private float _xSpeed = 4f;
        private float _ySpeed = 4f;
        
        private void Update()
        {
            movement();
            boundaries();
        }

        public Box(NeonArkanoidGame game, int pWidth, int pHeight, Color color) : base(pWidth, pHeight) 
        {
            _boxCollor = color;
            _game = game;
            _height = height;
            _width = width;
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
