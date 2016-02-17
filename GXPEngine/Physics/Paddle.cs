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
        private int _xLimit;
        private int _border = 20;

        public Paddle(Level.Level level, Vec2 position = null) : base("../assets/sprite/player/player.png", 26, 1)
        {
            SetOrigin(width/2, 0);
            _level = level;
            _position = position;
            if (UtilitySettings.DebugMode) alpha = 0;

            if (Position != null)
            {
                x = Position.x;
                y = Position.y;
            }



            CreateLines();

            CreateLimit();
        }

        private void CreateLimit()
        {
            foreach (var lineVec in _lineVecs)
            {
                if (lineVec.x > _xLimit) _xLimit = (int)lineVec.x;
            }
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

            LimitMovement();
        }

        private void LimitMovement()
        {
            if (Position.x < _xLimit + _border) Position.x = _xLimit + _border;
            if (Position.x > game.width - _xLimit - _border) Position.x = game.width - _xLimit - _border;
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

            _lines = new LineSegment[_lineVecs.Length - 1];
            for (var i = 0; i < _lines.Length; i++)
            {
                _lines[i] = new LineSegment(_position.Clone().Add(_lineVecs[i]), _position.Clone().Add(_lineVecs[i + 1]),
                    0xFF00FF00);
                _level.AddChild(_lines[i]);
                Console.WriteLine("Creating line with startpoint: " + _lines[i].Start + " and endpoint: " +
                                  _lines[i].End);
            }
        }

        public void Step()
        {
            var xDiff = _position.x - x;
            var yDiff = _position.y - y;

            x = _position.x;
            y = _position.y;

            for (var i = 0; i < _lines.Length; i++)
            {
                _lines[i].Start.SetXY(_lines[i].Start.x + xDiff, _lines[i].Start.y + yDiff);
                _lines[i].End.SetXY(_lines[i].End.x + xDiff, _lines[i].End.y + yDiff);
            }
        }
    }
}