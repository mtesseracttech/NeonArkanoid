using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Physics;
using TiledParser;
using System.Drawing.Drawing2D;
using System.Drawing;
using NeonArkanoid.GXPEngine.Utils;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private readonly Map _map;
        private List<Polygon> _polyList;
        private string _levelName; //useless for now
        private NeonArkanoidGame _game;
        private Ball _ball;
        private LineSegment _lineA;
        private float _leftXBoundary;
        private float _rightXBoundary;
        private float _topYBoundary;

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            BoundaryCreator();
       
            
            graphics.SmoothingMode = SmoothingMode.HighQuality;
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
            _ball.Position = new Vec2(Input.mouseX, Input.mouseY);
            _ball.Step();
            //BallMovement();
            CheckCollisions();

        }

        private void CheckCollisions()
        {
            foreach (GameObject other in _ball.GetCollisions())
            {
                if (other is Polygon)
                {
                    throw new Exception("EXPLOSION");
                    Polygon poly = other as Polygon;
                    poly.RemovePoly();
                }
            }
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
                                List<char> chars = property.Property.Value.ToLower().ToCharArray().ToList();
                                chars.Insert(2, 'f');
                                chars.Insert(2, 'f');
                                polyColor = Convert.ToUInt32(new string(chars.ToArray()), 16);
                                //polyColor = Convert.ToUInt32(property.Property.Value, 16) + 0xFF000000;
                            }
                        }
                        var poly = new Polygon(vectorArray, polyColor, this, tiledObject.X, tiledObject.Y);
                        poly.SetXY(tiledObject.X, tiledObject.Y);
                        _polyList.Add(poly);
                    }
                }
            }
        }

        public void RemoveFromPolyList(Polygon poly)
        {
            _polyList.Remove(poly);
        }

        public void Redraw()
        {
            graphics.Clear(Color.Black);
            foreach (var polygon in _polyList)
            {
                polygon.DrawOnCanvas();
            }
        }



        /*
        void lineCollisionTest(LineSegment line)
        {
            int type = 0;

            if (line.color == 0xffffff00)
            {
                type = 1;
            }
            else if (line.color == 0xffff0000)
            {
                type = 2;
            }

            Vec2 lineVector = line.end.Clone().Sub(line.start);
            Vec2 lineVectorNormalized = lineVector.Clone().Normalize();
            Vec2 lineNormal = lineVector.Clone().Normal();
            float lineLength = lineVector.Length();
            Vec2 ball2Line = _ball.position.Clone().Sub(line.start);

            float ballDistance = ball2Line.Dot(lineNormal);
            if (Math.Abs(ballDistance) < _ball.radius)
            {
                float ballDistanceAlongLine = ball2Line.Dot(lineVectorNormalized);
                if (ballDistanceAlongLine < 0)
                    ballDistanceAlongLine = 0;
                if (ballDistanceAlongLine > lineLength)
                    ballDistanceAlongLine = lineLength;
                Vec2 closestPointOnLine = line.start.Clone().Add(lineVectorNormalized.Scale(ballDistanceAlongLine));
                Vec2 difference = _ball.position.Clone().Sub(closestPointOnLine);
                if (difference.Length() < _ball.radius)
                {
                    Vec2 normal = difference.Clone().Normalize();
                    float separation = _ball.radius - difference.Length();
                    _ball.position.Add(normal.Clone().Scale(separation));
                    _ball.velocity.Reflect(normal, 0.5f);
                    _ball.x = _ball.position.x;
                    _ball.y = _ball.position.y;
                    if (type == 2)
                    {
                        if (Math.Abs(_ball.velocity.Clone().Length()) < 3)
                        {
                            collisionCounter++;
                            if (collisionCounter >= 50)
                            {
                                Respawn();
                                _ball.position = mainTP.position.Clone();
                                collisionCounter = 0;
                            }
                        }
                    }
                    if (Math.Abs(_ball.velocity.Clone().Length()) > 3)
                    {
                        SoundManager.PlaySound(SoundEffect.MSCOLLIDE, 1f, 0);
                        if (line.color == 0xffff0000)
                        {
                            SoundManager.PlaySound(SoundEffect.BUMPER, 1, 0);
                            Settings.score += 50 * multi;
                        }
                    }
                }
            }
        }
        */

        public string GetLevelName ()
        {
            return _levelName;
        }

        private void BoundaryCreator()
        {
            // y = Utils.Clamp(y, height/2, game.height/2 - height/2);
            // x = Utils.Clamp(x, width / 2, game.width / 2 - width / 2);
            float border = 1;
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



            
            //_ball.x += maxspeed;
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
                _ball.Velocity.SetXY(_ball.Velocity.x, _ball.Velocity.y);
            }
            if (righyHit)
            {
                _ball.Position.x = _rightXBoundary - _ball.Radius;
                _ball.Velocity.SetXY(_ball.Velocity.x, _ball.Velocity.y);
            }
            if (topHit)
            {
                _ball.Position.y = _topYBoundary + _ball.Radius;
                _ball.Velocity.SetXY(_ball.Velocity.x, -_ball.Velocity.y);
            }

        }

        private void CheckBallCollisons()
        {
            var leftHit = _ball.Position.x - _ball.Radius < _leftXBoundary - _ball.Velocity.x;
            var rightHit = _ball.Position.x + _ball.Radius > _rightXBoundary - _ball.Velocity.x;
            var topHit = _ball.Position.y - _ball.Radius < _topYBoundary - _ball.Velocity.y;

            ReflectBallBack(leftHit, rightHit, topHit);
        }
    }
}