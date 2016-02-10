using System;
using NeonArkanoid.GXPEngine;

namespace NeonArkanoid.Physics
{
    internal class Polygon : GameObject
    {
        private readonly Vec2[] _points;
        private LineSegment[] _lines;

        public Polygon(Vec2[] points)
        {
            if (points.Length >= 3)
            {
                _points = points;
                _lines = new LineSegment[_points.Length];
                CreateLines();
            }
            else
            {
                Console.WriteLine("TOO FEW POINTS TO CREATE POLY!");
            }
        }

        void CreateLines()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                if (i > 1) _lines[i] = new LineSegment(_points[i - 1], _points[i]);
                else _lines[i] = new LineSegment(_points[_points.Length], _points[0]);
            }
        }

        public string GetPoints()
        {
            var returnstring = "";
            foreach (var point in _points)
            {
                returnstring += point + "\n";
            }
            return returnstring;
        }
    }
}