using System;
using System.Drawing;
using GXPEngine.Utility;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Utility;

namespace NeonArkanoid.Physics
{
    internal class Polygon : GameObject
    {
        private uint _color;
        private Vec2[] _points;
        private string _id;
        private Level.Level _level;
        private LineSegment[] _lines;
        private float _realPosX;
        private float _realPosY;
        
        
        public Polygon(Vec2[] points, uint color, Level.Level level, float realPosX, float realPosY)
        {
            _points = points;
            _level = level;
            _color = color;
            _realPosX = realPosX;
            _realPosY = realPosY;
            DrawOnCanvas();
            _lines = new LineSegment[_points.Length];
            CreateLines();
            foreach (var line in _lines) AddChild(line);
        }

        public void DrawOnCanvas()
        {
            PointF[] pointFs = new PointF[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                pointFs[i] = _points[i].Vec2toPointF();
                pointFs[i].X += _realPosX;
                pointFs[i].Y += _realPosY;
            }
            Console.WriteLine("Drawing polygon at coords: " + _realPosX + "," + _realPosY);
            if (!UtilitySettings.DebugMode) _level.graphics.DrawPolygon(new Pen(ColorUtils.UIntToColor(_color)), pointFs);
            if (!UtilitySettings.DebugMode) _level.graphics.FillPolygon(new SolidBrush(ColorUtils.UIntToColor(_color)), pointFs);
        }

        private void CreateLines()
        {
            for (var i = 0; i < _points.Length; i++)
            {
                if (i < _points.Length - 1)
                {
                    Console.WriteLine("Creating Line " + i + " with coords: " + _points[i] + _points[i + 1]);
                    _lines[i] = new LineSegment(_points[i], _points[i + 1], this, _color);
                    if(!UtilitySettings.DebugMode) _lines[i].Color = 0x00000000;

                }
                else
                {
                    Console.WriteLine("Creating Line " + i + " with coords: " + _points[_points.Length - 1] + _points[0]);
                    _lines[i] = new LineSegment(_points[_points.Length - 1], _points[0], this, _color);
                    if (!UtilitySettings.DebugMode) _lines[i].Color = 0x00000000;
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

        public void RemovePoly()
        {
            _level.RemovePoly(this);
        }

    }
}