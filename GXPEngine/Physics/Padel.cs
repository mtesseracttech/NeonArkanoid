using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;

namespace NeonArkanoid.Physics
{
    public class Padel : AnimSprite
    {
        private Vec2 _position;
        private LineSegment _line;
        private float _currentFrame = 0;
        private float _currentSpeed = 5f; // change the speed of animation


        public Padel(Vec2 position = null) : base("../assets/sprite/player/tab.png", 12, 1)
        {
            _position = position;

            //SetXY(game.width/2, 700);
            SetScaleXY(0.7f,0.7f);

            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            _line = new LineSegment(width + 80, 13, height - 25, 13, 0xffffffff, 100);
            AddChild(_line);
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


    }
}
