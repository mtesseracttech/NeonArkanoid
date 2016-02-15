﻿using System.Drawing;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Physics;

namespace NeonArkanoid.Physics
{
    public class Ball : Canvas
    {
        //public but still readonly, can only be assigned once and cannot be overwritten after this
        public readonly int radius;

        private Vec2 _acceleration;
        private Color _ballColor;
        private Vec2 _position;
        private Vec2 _velocity;

        private AnimationSprite _spriteOverlay;
        float _currentFrame = 0;
        private float _currentSpeed = 10f; // change the speed of animation
         /**
		 * Creates a ball with a radius, a start position, a start velocity, and optionally a color.
		 * Note the Color? this means that pColor can be null (which is not possible normally for structs since they are value types).
		 */

        public Ball(int pRadius, Vec2 position = null, Vec2 velocity = null, Vec2 acceleration = null, bool 
            physics = false, Color? pColor = null) 
            : base(pRadius*2, pRadius*2)
        {
            Physics = physics;
            radius = pRadius;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;

            _spriteOverlay = new AnimationSprite("../assets/sprite/player/ball.png", 13, 1);
            AddChild(_spriteOverlay);
            _spriteOverlay.SetXY(-170,-50);


            //?? means: assign pColor unless pColor is null then take Color.Blue instead
            //basically this is short for if (pColor == null) { _ballColor = Color.Blue} else { _ballColor = Color.Blue }
            //another short way to write this is _ballColor = (pColor == null?Color.Blue:pColor);
            //use whatever you feel comfortable with and are able to explain.
            // _ballColor = pColor ?? Color.Blue;

            _ballColor = pColor ?? Color.Blue;

            Draw();
            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            Step();
        }

        void Update()
        {

            // if (_spriteOverlay.currentFrame <= _spriteOverlay.frameCount)
            // {
            //_spriteOverlay.currentFrame ++;
            _currentFrame += _currentSpeed / 50;
            _currentFrame %= _spriteOverlay.frameCount;
            _spriteOverlay.SetFrame((int)_currentFrame);
            //   }
            //else _spriteOverlay.currentFrame = 0;
        }

        public Vec2 Gravity { get; }

        public Vec2 Position
        {
            set { _position = value ?? Vec2.zero; }
            get { return _position; }
        }

        public Vec2 Velocity
        {
            set { _velocity = value ?? Vec2.zero; }
            get { return _velocity; }
        }

        public Vec2 Acceleration
        {
            set { _acceleration = value ?? Vec2.zero; }
            get { return _acceleration; }
        }

        public bool Physics { set; get; }

        public Color BallColor
        {
            get { return _ballColor; }
            set
            {
                _ballColor = value;
                Draw();
            }
        }

        private void Draw()
        {
            SetOrigin(radius, radius);

            graphics.FillEllipse(
                new SolidBrush(_ballColor),
                0, 0, 2*radius, 2*radius
                );
        }

        public void Step()
        {
            if (Physics)
            {
                _velocity.Add(_acceleration);
                _position.Add(_velocity);
            }
            x = _position.x;
            y = _position.y;
        }
    }
}