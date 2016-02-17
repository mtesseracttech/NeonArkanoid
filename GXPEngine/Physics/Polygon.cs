using System;
using System.Drawing;
using GXPEngine.Utility;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Core;
using NeonArkanoid.GXPEngine.OpenGL;
using NeonArkanoid.Utility;

namespace NeonArkanoid.Physics
{
    internal class Polygon : GameObject
    {
        private readonly uint _color;
        private readonly Level.Level _level;
        private readonly LineSegment[] _lines;
        private readonly Vec2[] _points;
        private readonly float _realPosX;
        private readonly float _realPosY;
        private int _id;


        public Polygon(Vec2[] points, uint color, Level.Level level, int id, float realPosX, float realPosY)
        {
            _points = points;
            _realPosX = realPosX;
            _realPosY = realPosY;
            _level = level;
            _color = color;
            _id = id;
            DrawOnCanvas();
            _lines = new LineSegment[_points.Length];
            CreateLines();
            foreach (var line in _lines) _level.AddChild(line);
        }

        public void DrawOnCanvas()
        {
            var pointFs = new PointF[_points.Length];
            for (var i = 0; i < _points.Length; i++)
            {
                pointFs[i] = _points[i].Vec2ToPointF();
                pointFs[i].X += _realPosX;
                pointFs[i].Y += _realPosY;
            }
            if (UtilitySettings.DebugMode)
                Console.WriteLine("Drawing polygon at coords: " + _realPosX + "," + _realPosY);
            if (!UtilitySettings.DebugMode)
                _level.GetPolyField().graphics.DrawPolygon(new Pen(ColorUtils.UIntToColor(_color)), pointFs);
            if (!UtilitySettings.DebugMode)
                _level.GetPolyField().graphics.FillPolygon(new SolidBrush(ColorUtils.UIntToColor(_color)), pointFs);
        }

        private void CreateLines()
        {
            var relativePoints = new Vec2[_points.Length];
            for (var i = 0; i < _points.Length; i++)
            {
                relativePoints[i] = new Vec2();
                relativePoints[i].x = _points[i].x + _realPosX;
                relativePoints[i].y = _points[i].y + _realPosY;
            }
            for (var i = 0; i < relativePoints.Length; i++)
            {
                if (i < relativePoints.Length - 1)
                {
                    if (UtilitySettings.DebugMode)
                        Console.WriteLine("Creating Line " + i + " with coords: " + relativePoints[i] +
                                          relativePoints[i + 1]);
                    _lines[i] = new LineSegment(relativePoints[i], relativePoints[i + 1], this, _color);
                    if (!UtilitySettings.DebugMode) _lines[i].Color = 0x00000000;
                }
                else
                {
                    if (UtilitySettings.DebugMode)
                        Console.WriteLine("Creating Line " + i + " with coords: " +
                                          relativePoints[relativePoints.Length - 1] + relativePoints[0]);
                    _lines[i] = new LineSegment(relativePoints[relativePoints.Length - 1], relativePoints[0], this,
                        _color);
                    if (!UtilitySettings.DebugMode) _lines[i].Color = 0x00000000;
                }
            }
        }

        public LineSegment[] GetLines()
        {
            return _lines;
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
            foreach (var lineSegment in _lines)
            {
                lineSegment.Destroy();
            }
            _level.RemovePoly(this);
        }
    }
}