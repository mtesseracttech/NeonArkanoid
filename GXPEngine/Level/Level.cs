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

            _ball = new Ball(30, new Vec2(game.width/4, game.height/4));
            AddChild(_ball);
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
                                //var chars = property.Property.Value.ToLower().ToCharArray().ToList();
                                //chars.Insert(2, 'f');
                                //chars.Insert(2, 'f');
                                //polyColor = Convert.ToUInt32(new string(chars.ToArray()), 16);
                                polyColor = Convert.ToUInt32(property.Property.Value, 16) + 0xFF000000;
                            }
                        }
                        var poly = new Polygon(vectorArray, polyColor, this, tiledObject.ID ,tiledObject.X, tiledObject.Y);
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
            if (Input.GetKey(Key.UP)) _ball.Velocity.y = -1;
            else if (Input.GetKey(Key.DOWN)) _ball.Velocity.y = 1;
            else _ball.Velocity.y = 0;
            if (Input.GetKey(Key.LEFT)) _ball.Velocity.x = -1;
            else if (Input.GetKey(Key.RIGHT)) _ball.Velocity.x = 1;
            else _ball.Velocity.x = 0;
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

            LimitBallSpeed();
            
            for (int i = 0; i < _ball.Velocity.Length(); i++)
            {

                _ball.Position.Add(_ball.Velocity.Clone().Normalize());
                _ball.x = _ball.Position.x;
                _ball.y = _ball.Position.y;

                if (_polyList.Count > 0)
                {
                    for (int p = 0; p < _polyList.Count; p++)
                    {
                        for (int l = 0; l < _polyList[p].GetLines().Length; l++)
                        {
                            LineCollisionTest(_polyList[p].GetLines()[l]); //TODO LAST POLYGON IN GAME GIVES EXCEPTION, MOST LIKELY BECAUSE IT TRIES TO SCAN FOR AN ITEM WHILE THERE ARE NO MORE.
                        }
                    }
                }
            }
        }

        void LineCollisionTest(LineSegment line)
        {
            
            Vec2 lineVector = line.End.Clone().Subtract(line.Start);
            Vec2 lineVectorNormalized = lineVector.Clone().Normalize();
            Vec2 lineNormal = lineVector.Clone().Normal();
            float lineLength = lineVector.Length();
            Vec2 ball2Line = _ball.Position.Clone().Subtract(line.Start);

            float ballDistance = ball2Line.Dot(lineNormal);
            if (Math.Abs(ballDistance) < _ball.radius)
            {
                
                float ballDistanceAlongLine = ball2Line.Dot(lineVectorNormalized);
                if (ballDistanceAlongLine < 0) ballDistanceAlongLine = 0;

                if (ballDistanceAlongLine > lineLength) ballDistanceAlongLine = lineLength;

                Vec2 closestPointOnLine = line.Start.Clone().Add(lineVectorNormalized.Scale(ballDistanceAlongLine));
                Vec2 difference = _ball.Position.Clone().Subtract(closestPointOnLine);
                Console.WriteLine((int)difference.Length());
                if (difference.Length() < _ball.radius)
                {
                    if (line.GetOwner() is Polygon)
                    {
                        Polygon owner = line.GetOwner() as Polygon;
                        owner.RemovePoly();
                        
                    }
                    
                    Vec2 normal = difference.Clone().Normalize();
                    float separation = _ball.radius - difference.Length();
                    _ball.Position.Add(normal.Clone().Scale(separation));
                    _ball.Velocity.Reflect(normal, 0.5f);
                    _ball.x = _ball.Position.x;
                    _ball.y = _ball.Position.y;
                }
            }
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
    }
}