using System;
using NeonArkanoid.GXPEngine;

namespace NeonArkanoid.Physics
{
    internal class Polygon : GameObject
    {
        private readonly Vec2[] _points;
        private LineSegment[] _lines;
        private string _id;

        public Polygon(Vec2[] points, string id)
        {
            Console.WriteLine("Creating Polygon: " + id);
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
            _id = id;
        }

        void CreateLines()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                if (i < _points.Length -1)
                {
                    Console.WriteLine("Creating Line " + i + " with coords: " + _points[i] + _points[i + 1]);
                    _lines[i] = new LineSegment(_points[i], _points[i + 1]);
                }
                else
                {
                    Console.WriteLine("Creating Line " + i + " with coords: " + _points[_points.Length - 1] + _points[0]);
                    _lines[i] = new LineSegment(_points[_points.Length - 1], _points[0]);
                }
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