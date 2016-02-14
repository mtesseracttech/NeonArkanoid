using NeonArkanoid.GXPEngine;

namespace NeonArkanoid.Physics
{
    public class Padel : AnimSprite
    {
        private Vec2 _position;
        private Vec2 _velocity;
        private Vec2 _acceleration;
        private float _currentFrame = 0;
        private float _currentSpeed = 5f; // change the speed of animation


        public Padel(Vec2 position = null, Vec2 velocity = null, Vec2 acceleration = null, bool physics = false) : base("../assets/sprite/player/tab.png", 12, 1)
        {
            _position = position;
            _velocity = velocity;
            _acceleration = acceleration;
            Physics = physics;
            //SetXY(game.width/2, 700);
            SetScaleXY(0.7f,0.7f);

            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            Step();
        }

        void Update()
        {
            _currentFrame += _currentSpeed / 50;
            _currentFrame %= frameCount;
            SetFrame((int)_currentFrame);
              
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
