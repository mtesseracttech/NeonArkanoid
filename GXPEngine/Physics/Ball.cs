using System.Drawing;
using NeonArkanoid.GXPEngine;
using System;

namespace NeonArkanoid.Physics
{
    public class Ball : AnimSprite
    {

        private Vec2 _position;
        public Vec2 _velocity;
        private Color _ballColor;
        //public but still readonly, can only be assigned once and cannot be overwritten after this
        public readonly int Radius;
        public Vec2 _acceleration = new Vec2 (0,0);
        public Vec2 gravity = new Vec2(0, 0.0f);
        float frame = 0.5f;
        int firstFrame = 0;
        int lastFrame = 12;

        /**
		 * Creates a ball with a radius, a start position, a start velocity, and optionally a color.
		 * Note the Color? this means that pColor can be null (which is not possible normally for structs since they are value types).
		 /**/

         /**
            public Ball(int pRadius, Vec2 position = null, Vec2 velocity = null, Vec2 acceleration = null,
            bool physics = false,
            Color? pColor = null)
            : base("../assets/sprite/player/ball.png") //(pRadius*2, pRadius*2)
          /**/
              public Ball(int pRadius, Vec2 position = null, Vec2 velocity = null, Color? pColor = null): base("../assets/sprite/player/ball.png",13,1) //(pRadius*2, pRadius*2)
        {
            //Physics = physics;
            Radius = pRadius;
            SetOrigin(width / 2, height / 2);

            Position = position ?? Vec2.zero;
            _ballColor = pColor ?? Color.Blue;
            Velocity = velocity ?? Vec2.zero;

           // Acceleration = acceleration;
            //?? means: assign pColor unless pColor is null then take Color.Blue instead
            //basically this is short for if (pColor == null) { _ballColor = Color.Blue} else { _ballColor = Color.Blue }
            //another short way to write this is _ballColor = (pColor == null?Color.Blue:pColor);
            //use whatever you feel comfortable with and are able to explain.
           // _ballColor = pColor ?? Color.Blue;

            /**/
            //Draw();
            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            /**/
            
        }
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
               // Draw();
            }
        }
        /**
        private void Draw()
        {
            SetOrigin(Radius, Radius);
            graphics.FillEllipse(
                new SolidBrush(_ballColor),
                0, 0, 2*Radius, 2*Radius
                );
        }
        /**/

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

        

        public void UpdateAnimtaion()
        {
            frame += 0.3f;

            if (frame >= lastFrame + 1) frame = firstFrame;
            if (frame < firstFrame)     frame = firstFrame;

            SetFrame((int)frame);
        }

        public void SetAnimtationRange(int first, int last)
        {
            firstFrame = first;
            lastFrame = last;
<<<<<<< HEAD
           
=======
>>>>>>> origin/dev
        }

    }
}