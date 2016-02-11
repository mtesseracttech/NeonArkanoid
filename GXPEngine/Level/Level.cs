using System;
using System.Collections.Generic;
using System.Globalization;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Physics;
using TiledParser;
using System.Drawing.Drawing2D;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using System.Drawing;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private string _levelName; //useless for now
        private NeonArkanoidGame _game;
        private Ball _ball;

        private float maxspeed = 3f;
        private LineSegment _lineA;

        private float _leftXBoundary;
        private float _rightXBoundary;
        private float _topYBoundary;

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            BoundaryCreator();
            _game = game;
            var tmxParser = new TMXParser();
            _map = tmxParser.Parse(filename);

            //_tilesheetName = _map.TileSet.Image.Source;

            for (var i = 0; i < _map.ObjectGroup.Length; i++)
            {
                if (_map.ObjectGroup[i].Name.ToLower() == "polygons")
                {
                    _polyList = new List<Polygon>();
                    CreatePolygons(_map.ObjectGroup[i]);
                }
            }
            foreach (var polygon in _polyList)
            {
                AddChild(polygon);
            }

            _lineA = new LineSegment(Vec2.zero,Vec2.zero, 0x00000000, 2, true);
            AddChild(_lineA);

            _ball = new Ball(30, new Vec2(400, 400),null, Color.BlueViolet);
            AddChild(_ball);

        }

        public void Update()
        {
            BallMovement();
            
        }
        
        private void CreatePolygons(ObjectGroup objectGroup)
        {
            foreach (var tiledObject in objectGroup.TiledObjects)
            {
                if (tiledObject.Polygon != null)
                {
                    var pointArr = tiledObject.Polygon.Points.Split(' ');
                    var vectorArray = new Vec2[pointArr.Length];
                    for (var i = 0; i < pointArr.Length; i++)
                    {
                        var pointCoords = pointArr[i].Split(',');
                        vectorArray[i] = new Vec2(
                            float.Parse(pointCoords[0], CultureInfo.InvariantCulture.NumberFormat),
                            float.Parse(pointCoords[1], CultureInfo.InvariantCulture.NumberFormat));
                    }

                    uint polyColor = 0;
                    if (tiledObject.Properties != null)
                    {
                        foreach (var property in tiledObject.Properties)
                        {
                            if (property.Property.Name.ToLower() == "colour")
                            {
                                polyColor = Convert.ToUInt32(property.Property.Value, 16) + 0xFF000000;
                            }
                        }
                        var poly = new Polygon(vectorArray, polyColor, this, tiledObject.X, tiledObject.Y);
                        poly.SetXY(tiledObject.X, tiledObject.Y);
                        _polyList.Add(poly);
                    }
                }
            }
        }

        public string GetLevelName ()
        {
            return _levelName;
        }

        private void BoundaryCreator()
        {
            // y = Utils.Clamp(y, height/2, game.height/2 - height/2);
            // x = Utils.Clamp(x, width / 2, game.width / 2 - width / 2);
            float border = -1;
            _leftXBoundary = border;
            _rightXBoundary = width - border;
            _topYBoundary = border;

            CreateVisualXBoundary(_leftXBoundary);
            CreateVisualXBoundary(_rightXBoundary);
            CreateVisualYBoundary(_topYBoundary);

        }

        private void CreateVisualXBoundary(float xBoundary)
        {
            AddChild(new LineSegment(xBoundary, 0, xBoundary, height, 0xffffffff, 1));
        }

        private void CreateVisualYBoundary(float yBoundary)
        {
            AddChild(new LineSegment(0, yBoundary, width, yBoundary, 0xffffffff, 1));
        }

        private void BallMovement()
        {
            _ball.x += maxspeed;
            if (_ball.Velocity.x < -maxspeed)
            {
                _ball.Velocity.x = -maxspeed;
            }
            if (_ball.Velocity.x > maxspeed)
            {
                _ball.Velocity.x = maxspeed;
            }
            if (_ball.Velocity.y > maxspeed)
            {
                _ball.Velocity.y = maxspeed;
            }
            if (_ball.Velocity.y < -maxspeed)
            {
                _ball.Velocity.y = -maxspeed;
            }


            for (int i = 0; i < _ball._acceleration.Length(); i++)
            {
                _ball.Velocity.Add(_ball._acceleration.Clone().Normalize());
            }
            for (int g = 0; g < _ball.gravity.Length(); g++)
            {
                _ball.Velocity.Add(_ball.gravity);
            }
            
            
        }

        private void CheckBallCollisons()
        {
            var leftHit = _ball.Position.x - _ball.Radius < _leftXBoundary - _ball.Velocity.x;
            var rightHit = _ball.Position.x + _ball.Radius > _rightXBoundary - _ball.Velocity.x;
            var topHit = _ball.Position.y - _ball.Radius < _topYBoundary - _ball.Velocity.y;

            ReflectBallBack(leftHit, rightHit, topHit);
        }

        private void ReflectBallBack(bool leftHit, bool righyHit, bool topHit)
        {
            if (leftHit)
            {
                _ball.Position.x = _leftXBoundary + _ball.Radius;
                _ball.Velocity.SetXY(_ball._velocity.x, _ball.Velocity.y);
            }
            if (righyHit)
            {
                _ball.Position.x = _rightXBoundary - _ball.Radius;
                _ball.Velocity.SetXY(_ball._velocity.x, _ball.Velocity.y);
            }
            if (topHit)
            {
                _ball.Position.y = _topYBoundary + _ball.Radius;
                _ball.Velocity.SetXY(_ball.Velocity.x, -_ball.Velocity.y);
            }

        }

       


    }
}