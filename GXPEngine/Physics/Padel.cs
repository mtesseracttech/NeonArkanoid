using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;

namespace NeonArkanoid.Physics
{
    public class Padel : AnimSprite
    {
        private Vec2 _position;
        private LineSegment _line1;
        public LineSegment _line2;
        private LineSegment _line3;
        private float _currentFrame = 0;
        private float _currentSpeed = 10f; // change the speed of animation


        public Padel(Vec2 position = null) : base("../assets/sprite/player/player.png", 26, 1)
        {
            _position = position;

            //SetXY(game.width/2, 700);
           // SetScaleXY(0.7f,0.7f);

            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
           CreateLines();

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

        private void CreateLines()
        {
            _line1 = new LineSegment(width, 10, width /2 + 30, 5, 0xff8888ff, 10);
            AddChild(_line1);
            _line2 = new LineSegment(width - 120, 5, width/2 - 90, 10, 0xff8888ff, 10);
            AddChild(_line2);
            _line3 = new LineSegment(width -60, 5, width/ 2 - 30, 5, 0xff0000ff, 10);
            AddChild(_line3);

        }
    }
}
