﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
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
        private readonly Ball _ball;
        private readonly NeonArkanoidGame _game;
        private readonly string _levelName; //useless for now
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private readonly Vec2 gravity = new Vec2(1, 0);
        private readonly Padel _padel;
        private readonly float maxspeed = 5;
        private bool _gameEnded;
        private int _endTimer;

        private float _leftXBoundary;
        private float _rightXBoundary;
        private float _topYBoundary;
        private float _bottomYBoundary;

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            _gameEnded = false;
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

            _padel = new Padel(new Vec2(game.width/2, 700));
            AddChild(_padel);
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
                            if (property.Property.Name.ToLower() == "colour" || property.Property.Name.ToLower() == "color")
                            {
                                //var chars = property.Property.Value.ToLower().ToCharArray().ToList();
                                //chars.Insert(2, 'f');
                                //chars.Insert(2, 'f');
                                //polyColor = Convert.ToUInt32(new string(chars.ToArray()), 16);
                                polyColor = Convert.ToUInt32(property.Property.Value, 16) + 0xFF000000;
                            }
                        }
                        var poly = new Polygon(vectorArray, polyColor, this, tiledObject.ID, tiledObject.X,
                            tiledObject.Y);
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

        public void Update()
        {
            if (_polyList.Count > 0)
            //--------BALL MOVEMENT-----------//
            if (Input.GetKey(Key.UP))_ball.Velocity.y--;
            else if (Input.GetKey(Key.DOWN))_ball.Velocity.y++;
            else _ball.Velocity.y = 0;

            if (Input.GetKey(Key.LEFT)) _ball.Velocity.x--;
            else if (Input.GetKey(Key.RIGHT))  _ball.Velocity.x++;
            else _ball.Velocity.x = 0;
            //-------------------------------//

            //--------Pedal MOVEMENT-----------//
            if (Input.GetKey(Key.A)) _padel.x -= 5f;
            else if (Input.GetKey(Key.D)) _padel.x += 5f;
            //-------------------------------//

            if (Input.GetKeyDown(Key.R))
            {
                Controls();
                LimitBallSpeed();



                for (var i = 0; i < _ball.Velocity.Length(); i++)
                {
                    _ball.Position.Add(_ball.Velocity.Clone().Normalize());
                    _ball.Step();

                    if (_polyList.Count > 0)
                        for (var p = 0; p < _polyList.Count; p++)
                        {
                            for (var l = 0; l < _polyList[p].GetLines().Length; l++)
                            {
                                bool collision = LineCollisionTest(_polyList[p].GetLines()[l]);
                                if (collision) break;
                            }
                        }
                }
            }
            else
            {
                EndRound();
            }
            
        }

        private void EndRound()
        {
            if (_gameEnded == false)
            {
                _endTimer = Time.now;
                _gameEnded = true;
                _ball.Position.Add(_ball.Velocity.Clone().Normalize());
                _ball.x =  _ball.Position.x;
                _ball.y = _ball.Position.y;

                if (_polyList.Count > 0)
                    for (var p = 0; p < _polyList.Count; p++)
                    {
                        for (var l = 0; l < _polyList[p].GetLines().Length; l++) if (LineCollisionTest(_polyList[p].GetLines()[l])) break;
                    }
            }
            if (_gameEnded && _endTimer + 1000 < Time.now) _game.SetState("MainMenu");
        }

        private void Controls()
        {
            if (Input.GetKey(Key.UP)) _ball.Velocity.y = -1;
            else if (Input.GetKey(Key.DOWN)) _ball.Velocity.y = 1;
            else _ball.Velocity.y = 0;

            if (Input.GetKey(Key.LEFT)) _ball.Velocity.x = -1;
            else if (Input.GetKey(Key.RIGHT)) _ball.Velocity.x = 1;
            else _ball.Velocity.x = 0;

            if (Input.GetKeyDown(Key.R)) if (_polyList.Count > 0) RemovePolyAt(0);

            if (Input.GetKeyDown(Key.T)) _game.SetState("Level1", true);
        }

        private bool LineCollisionTest(LineSegment line)
        {
            var lineVector = line.End.Clone().Subtract(line.Start);
            var lineVectorNormalized = lineVector.Clone().Normalize();
            var lineNormal = lineVector.Clone().Normal();
            var lineLength = lineVector.Length();
            var ballToLine = _ball.Position.Clone().Subtract(line.Start);
            var ballDistance = ballToLine.Dot(lineNormal);
            if (Math.Abs(ballDistance) < _ball.radius)
            {
                var ballDistanceAlongLine = ballToLine.Dot(lineVectorNormalized);
                if (ballDistanceAlongLine < 0) ballDistanceAlongLine = 0;
                if (ballDistanceAlongLine > lineLength) ballDistanceAlongLine = lineLength;
                var closestPointOnLine = line.Start.Clone().Add(lineVectorNormalized.Scale(ballDistanceAlongLine));
                var difference = _ball.Position.Clone().Subtract(closestPointOnLine);
                if (difference.Length() < _ball.radius)
                {
                    if (line.GetOwner() is Polygon)
                    {
                        var owner = line.GetOwner() as Polygon;
                        owner.RemovePoly();
                    }
                    var normal = difference.Clone().Normalize();
                    var separation = _ball.radius - difference.Length();
                    _ball.Position.Add(normal.Clone().Scale(separation));
                    _ball.Velocity.Reflect(normal, 0.5f);
                    _ball.Step();
                    return true;
                }
            }
            return false;
        }

        private void LimitBallSpeed()
        {
            if (_ball.Velocity.x > maxspeed) _ball.Velocity.x = maxspeed;
            if (_ball.Velocity.x < -maxspeed) _ball.Velocity.x = -maxspeed;
            if (_ball.Velocity.y > maxspeed) _ball.Velocity.y = maxspeed;
            if (_ball.Velocity.y < -maxspeed) _ball.Velocity.y = -maxspeed;
        }

        public string GetLevelName()
        {
            return _levelName;
        }

        public void BoundryCreator()
        {
            float border = 1;
            _leftXBoundary = border;
            _rightXBoundary = width - border;
            _topYBoundary = border;
            _bottomYBoundary = height - border;

            CreateVisualXBoundary(_leftXBoundary);
            CreateVisualXBoundary(_rightXBoundary);
            CreateVisualYBoundary(_topYBoundary);
            CreateVisualYBoundary(_bottomYBoundary);
        }

        private void CreateVisualXBoundary(float xBoundary)
        {
            AddChild(new LineSegment(xBoundary, 0, xBoundary, height, 0xffffffff, 1));
        }

        private void CreateVisualYBoundary(float yBoundary)
        {
            AddChild(new LineSegment(0, yBoundary, width, yBoundary, 0xffffffff, 1));
        }




    }
}