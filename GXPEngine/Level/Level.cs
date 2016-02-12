using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using GXPEngine;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Physics;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private Ball _ball;
        private readonly NeonArkanoidGame _game;
        private readonly string _levelName; //useless for now
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private Arrow ballArrow;
        private readonly float maxspeed = 5;

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            _levelName = filename.Remove(filename.Length - 4);
            Console.WriteLine(_levelName);
            _game = game;
            graphics.SmoothingMode = SmoothingMode.HighQuality;

            var tmxParser = new TMXParser();
            _map = tmxParser.Parse(filename);

            for (var i = 0; i < _map.ObjectGroup.Length; i++)
            {
                if (_map.ObjectGroup[i].Name.ToLower() == "polygons" || _map.ObjectGroup[i].Name.ToLower() == "polygon")
                {
                    _polyList = new List<Polygon>();
                    CreatePolygons(_map.ObjectGroup[i]);
                }
            }
            foreach (var polygon in _polyList)
            {
                AddChild(polygon);
            }

            _ball = new Ball(30, new Vec2(game.width/2, game.height/2));
            AddChild(_ball);
        }

        public void Update()
        {
            if (Input.GetKey(Key.UP)) _ball.y--;
            if (Input.GetKey(Key.DOWN)) _ball.y++;
            if (Input.GetKey(Key.LEFT)) _ball.x--;
            if (Input.GetKey(Key.RIGHT)) _ball.x++;
            if (Input.GetKeyDown(Key.R))
            {
                if (_polyList.Count > 0)
                {
                    RemovePolyAt(0);
                }
            }
            if (Input.GetKeyDown(Key.T))
            {
                _game.SetState("Level1", true);
            }

           // LimitBallSpeed();

            //Applying Acceleration in For loop, so collision system will be less likely to mess up
            //for (int i = 0; i < _ball.Acceleration.Length(); i++) _ball.Velocity.Add(_ball.Acceleration.Clone().Normalize());

           // for (int i = 0; i < _ball.Gravity.Length(); i++) _ball.Velocity.Add(_ball.Gravity);
        }

        private void LimitBallSpeed()
        {
            if (_ball.Velocity.x > maxspeed) _ball.Velocity.x = maxspeed;
            if (_ball.Velocity.x < -maxspeed) _ball.Velocity.x = -maxspeed;
            if (_ball.Velocity.y > maxspeed) _ball.Velocity.y = maxspeed;
            if (_ball.Velocity.y < -maxspeed) _ball.Velocity.y = -maxspeed;
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
                                var chars = property.Property.Value.ToLower().ToCharArray().ToList();
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

        public void RemovePoly(Polygon poly)
        {
            poly.Destroy();
            _polyList.Remove(poly);
            Redraw();
        }

        public void RemovePolyAt(int index)
        {
            _polyList[index].Destroy();
            _polyList.RemoveAt(index);
            Redraw();
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

        public string GetLevelName()
        {
            return _levelName;
        }
    }
}