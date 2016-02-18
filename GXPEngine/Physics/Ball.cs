using System.Drawing;
using System.Drawing.Drawing2D;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Utility;

namespace NeonArkanoid.Physics
{
    public class Ball : Canvas
    {
        //public but still readonly, can only be assigned once and cannot be overwritten after this
        public readonly int radius;

        private Vec2 _acceleration;
        private Color _ballColor;
        private float _currentFrame;
        private readonly float _currentSpeed = 10f; // change the speed of animation
        private Vec2 _position;

        private readonly AnimationSprite _spriteOverlay;
        private Vec2 _velocity;
        /**
		 * Creates a ball with a radius, a start position, a start velocity, and optionally a color.
		 * Note the Color? this means that pColor can be null (which is not possible normally for structs since they are value types).
		 */

        public Ball(int pRadius, Vec2 position = null, Vec2 velocity = null, Vec2 acceleration = null, Color? pColor = null)
        : base(pRadius*2, pRadius*2)
        {
            graphics.SmoothingMode= SmoothingMode.HighQuality;
            radius = pRadius;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            _ballColor = pColor ?? Color.Blue;

            //Draw();
            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            Step();
        }

        public Ball(int pRadius, AnimationSprite sprite, Vec2 position = null, Vec2 velocity = null, Vec2 acceleration = null)
        : base(pRadius * 2, pRadius * 2)
        {
            _spriteOverlay = sprite;
            _spriteOverlay.SetOrigin(_spriteOverlay.width / 2, _spriteOverlay.height / 2);
            AddChild(_spriteOverlay);
            radius = pRadius;
            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;

            _ballColor = Color.FromArgb(0x000000);

            Draw();
            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            Step();
        }

        public AnimationSprite GetSprite()
        {
            return _spriteOverlay;
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
                Draw();
            }
        }

        private void Update()
        {
            if (_spriteOverlay != null)
            {
                _currentFrame += _currentSpeed / 50;
                _currentFrame %= _spriteOverlay.frameCount;
                _spriteOverlay.SetFrame((int)_currentFrame);
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
            x = _position.x;
            y = _position.y;
        }
    }
}