using System;
using NeonArkanoid.GXPEngine;

namespace NeonArkanoid.Physics
{
    internal class Paddle : AnimSprite
    {
        private readonly float _currentSpeed = 10f; // change the speed of animation
        private readonly Level.Level _level;
        private new float _currentFrame;
        private LineSegment[] _lines;
        private Vec2 _position;


        public Paddle(Level.Level level, Vec2 position = null) : base("../assets/sprite/player/player.png", 26, 1)
        {
            _level = level;
            _position = position;

            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }
            CreateLines();
        }

        public Vec2 Position
        {
            set { _position = value ?? Vec2.zero; }
            get { return _position; }
        }

        public LineSegment[] GetLines()
        {
            return _lines;
        }

        private void Update()
        {
            _currentFrame += _currentSpeed/50;
            _currentFrame %= frameCount;
            SetFrame((int) _currentFrame);
        }

        private void CreateLines()
        {
            _lines = new LineSegment[3];
            _lines[0] = new LineSegment(x + width, y + 10, x + width/2 + 30, y + 5, 0xff8888ff, 10);
            _lines[1] = new LineSegment(x + width - 120, y + 5, x + width/2 - 90, y + 10, 0xff8888ff, 10);
            _lines[2] = new LineSegment(x + width - 60, y + 5, x + width/2 - 30, y + 5, 0xff0000ff, 10);
            foreach (var line in _lines) _level.AddChild(line);
        }

        public void Step()
        {
            x = _position.x;
            y = _position.y;

            Console.WriteLine(Position);
            _lines[0].Start.SetXY(x + width, y + 10);
            _lines[1].Start.SetXY(x + width - 120, y + 5);
            _lines[2].Start.SetXY(x + width - 60, y + 5);
            _lines[0].End.SetXY(x + width/2 + 30, y + 5);
            _lines[1].End.SetXY(x + width/2 - 90, y + 10);
            _lines[2].End.SetXY(x + width/2 - 30, y + 5);

            foreach (var lineSegment in _lines)
            {
                Console.WriteLine(lineSegment.Start + " " + lineSegment.End);
            }
        }
    }
}