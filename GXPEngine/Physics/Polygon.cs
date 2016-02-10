using System;
using System.Drawing;
using NeonArkanoid.GXPEngine;

namespace NeonArkanoid.Physics
{
    internal class Polygon : GameObject
    {
        private uint _color;
        private Vec2[] _points;
        private string _id;
        private Level.Level _level;
        private LineSegment[] _lines;
        
        
        public Polygon(Vec2[] points, uint color)
        {
            _color = color;
            _points = points;
            _lines = new LineSegment[_points.Length];
            CreateLines();
            foreach (var line in _lines) AddChild(line);
        }
        
        private void CreateLines()
        {
            for (var i = 0; i < _points.Length; i++)
            {
                if (i < _points.Length - 1)
                {
                    Console.WriteLine("Creating Line " + i + " with coords: " + _points[i] + _points[i + 1]);
                    _lines[i] = new LineSegment(_points[i], _points[i + 1], _color);
                }
                else
                {
                    Console.WriteLine("Creating Line " + i + " with coords: " + _points[_points.Length - 1] + _points[0]);
                    _lines[i] = new LineSegment(_points[_points.Length - 1], _points[0], _color);
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