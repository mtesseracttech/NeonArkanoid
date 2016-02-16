﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq.Expressions;
using GXPEngine;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.GXPEngine.Utils;
using NeonArkanoid.Utility;
using NeonArkanoid.Physics;
using NeonArkanoid.UI.Menu;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private Background _background1;
        private readonly Ball _ball;
        private readonly NeonArkanoidGame _game;
        private readonly string _levelName; //useless for now
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private List<LineSegment> _borderList; 
        private Paddle _paddle;
        private readonly float maxspeed = 10;
        private bool _gameEnded;
        private int _endTimer;
        private Vec2 acceleration = new Vec2(0, 0.1f); //Gravity

        private float _leftXBoundary;
        private float _rightXBoundary;
        private float _topYBoundary;
        private float _bottomYBoundary;

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            _gameEnded = false;
            
            BoundaryCreator();
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

            _paddle = new Paddle(this, new Vec2(game.width/2, game.height-100));
            AddChild(_paddle);
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
            if (_polyList.Count > 0) //IN THIS BLOCK, ALL THE CODE THAT HAPPENS WHILE THE GAME PLAYS FITS IN
            {
                Controls();
                LimitBallSpeed();
                ApplyForces();
                CollisionDetections();
                DebugInfo();
            }
            else
            {
                EndRound();
            }
            
        }

        private void DebugInfo()
        {
            Console.WriteLine(_ball.Velocity.Length());
        }

        private void ApplyForces()
        {
            _ball.Acceleration = acceleration;
            _ball.Velocity.Add(_ball.Acceleration);
            _ball.Position.Add(_ball.Velocity);
            _ball.Step();
        }

        private void CollisionDetections()
        {
            //Ball velocity gets choppped into pieces to make sure that the hit detection works on higher speeds
            for (var i = 0; i < _ball.Velocity.Length(); i++)
            {
                _ball.Position.Add(_ball.Velocity.Clone().Normalize());
                _ball.Step();

                //ALL COLLISION DETECTIONS COME AFTER THIS POINT

                //Collisions with the vector polygons
                if (_polyList.Count > 0)
                {
                    for (var p = 0; p < _polyList.Count; p++)
                    {
                        for (var l = 0; l < _polyList[p].GetLines().Length; l++)
                        {
                            if (LineCollisionTest(_polyList[p].GetLines()[l], 0.8f))
                            {
                                _polyList[p].RemovePoly();
                                break;
                            }
                        }
                    }
                }


                //Collisions with the paddle
                foreach (var line in _paddle.GetLines())
                {
                    LineCollisionTest(line, 1f);
                }


                //Temporary collisions with the borders of the game
                foreach (var line in _borderList)
                {
                    LineCollisionTest(line, 1f);
                }


                //AND BEFORE THIS ONE
            }
        }

        private void EndRound()
        {
            //Triggers the end of the game and sets counter until game pops to different state/does something
            if (_gameEnded == false) 
            {
                _endTimer = Time.now;
                _gameEnded = true;
            }
            //Sets the game to the main menu after the set time is over
            if (_gameEnded && _endTimer + 2000 < Time.now)
            {
                _game.SetState("MainMenu");
            }
        }

        private void Controls()
        {
            if (Input.GetKey(Key.UP)) _ball.Velocity.y += -1;
            if (Input.GetKey(Key.DOWN)) _ball.Velocity.y += 1;

            if (Input.GetKey(Key.LEFT)) _ball.Velocity.x += -1;
            if (Input.GetKey(Key.RIGHT)) _ball.Velocity.x += 1;

            if (Input.GetKeyDown(Key.R)) if (_polyList.Count > 0) RemovePolyAt(0);

            if (Input.GetKeyDown(Key.T)) _game.SetState("Level1", true);

            if (Input.GetKey(Key.D))
            {
                _paddle.Position.x += 10;
                _paddle.Step();
            }
            if (Input.GetKey(Key.A))
            {
                _paddle.Position.x -= 10;
                _paddle.Step();
            }
        }

        private bool LineCollisionTest(LineSegment line, float reflectionStrength)
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
                    var normal = difference.Clone().Normalize();
                    var separation = _ball.radius - difference.Length();
                    _ball.Position.Add(normal.Clone().Scale(separation));
                    _ball.Velocity.Reflect(normal, reflectionStrength);
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


	
        public void BoundaryCreator()
        {
            float border = 1;
            _leftXBoundary = border;
            _rightXBoundary = width - border;
            _topYBoundary = border;
            _bottomYBoundary = height - border;

            _borderList = new List<LineSegment>();
            CreateVisualXBoundary(_leftXBoundary);
            CreateVisualXBoundary(_rightXBoundary);
            CreateVisualYBoundary(_topYBoundary);
            CreateVisualYBoundary(_bottomYBoundary);
            foreach (var lineSegment in _borderList)
            {
                AddChild(lineSegment);
            }
        }

        private void CreateVisualXBoundary(float xBoundary)
        {
            _borderList.Add(new LineSegment(xBoundary, 0, xBoundary, height, 0xffffffff, 1));
        }

        private void CreateVisualYBoundary(float yBoundary)
        {
            _borderList.Add(new LineSegment(0, yBoundary, width, yBoundary, 0xffffffff, 1));
        }

        private void SetBackground()
        {
            _background1 = new Background(UtilStrings.SpritesMenu + "background4.jpg", true);
            AddChild(_background1);
        }



    }
}