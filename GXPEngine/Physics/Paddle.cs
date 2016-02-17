using System;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Utility;

namespace NeonArkanoid.Physics
{
    internal class Paddle : AnimationSprite
    {
        private readonly float _currentSpeed = 10f; // change the speed of animation
        private readonly Level.Level _level;
        private new float _currentFrame;
        private LineSegment[] _lines;
        private Vec2[] _lineVecs;
        private Vec2 _position;


        public Paddle(Level.Level level, Vec2 position = null) : base("../assets/sprite/player/player.png", 26, 1)
        {
            _level = level;
            _position = position;
            if (UtilitySettings.DebugMode) alpha = 0;

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
            _lineVecs = new[]
            {
                new Vec2(-120, 20),
                new Vec2(-80, 5),
                new Vec2(-40, 0),
                new Vec2(0, 0),
                new Vec2(40, 0),
                new Vec2(80, 5),
                new Vec2(120, 20)
            };

            _lines = new LineSegment[_lineVecs.Length-1];
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i] = new LineSegment(_position.Add(_lineVecs[i]), _position.Add(_lineVecs[i + 1]), 0xFF00FF00);
                _level.AddChild(_lines[i]);
                Console.WriteLine("Creating line with startpoint: " + _lines[i].Start + " and endpoint: " + _lines[i].End);
            }
            
        }

        public void Step()
        {
            float xDiff = x - _position.x;
            float yDiff = y - _position.y;

            Console.WriteLine(xDiff + " , " + yDiff);

            x = _position.x;
            y = _position.y;

            Console.WriteLine(Position);

            /*
            _lines[0].Start.SetXY(x + , y + 10);
            _lines[1].Start.SetXY(x + width - 120, y + 5);
            _lines[2].Start.SetXY(x + width - 60, y + 5);
            _lines[3].Start.SetXY(x +);
            _lines[4].Start.SetXY(x +);
            _lines[0].End.SetXY(x + width / 2 + 30, y + 5);
            _lines[1].End.SetXY(x + width / 2 - 90, y + 10);
            _lines[2].End.SetXY(x + width / 2 - 30, y + 5);
            _lines[3].End.SetXY();
            _lines[4].End.SetXY();
            */
            /*
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].Start.SetXY(_position.x + _lineVecs[i].x, _position.y + _lineVecs[i].y);
                _lines[i].End.SetXY(_position.x + _lineVecs[i + 1].x, _position.y + _lineVecs[i].y);
            }
            */
            /*
            _lines[0].Start.SetXY(x + width, y + 10);
            _lines[1].Start.SetXY(x + width - 120, y + 5);
            _lines[2].Start.SetXY(x + width - 60, y + 5);
            _lines[0].End.SetXY(x + width/2 + 30, y + 5);
            _lines[1].End.SetXY(x + width/2 - 90, y + 10);
            _lines[2].End.SetXY(x + width/2 - 30, y + 5);
            */
            
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].Start.SetXY(_lines[i].Start.x + xDiff, _lines[i].Start.y + yDiff);
                _lines[i].End.SetXY(_lines[i].End.x + xDiff, _lines[i].End.y + yDiff);
            }

            foreach (var lineSegment in _lines)
            {
                Console.WriteLine(lineSegment.Start + " " + lineSegment.End);
            }
        }
    }
}