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
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private string _levelName; //useless for now
        private NeonArkanoidGame _game;
        private Ball _ball = new Ball(30, new Vec2(400, 400), null, Color.BlueViolet);

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {

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
            AddChild(_ball);

        }

        void Update()
        {
               
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
                                //string polyString = new string(chars.ToArray());
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

        private void DoCollisionCheck()
        {
            foreach (var gameObject in GetCollisions())
            {
                if (gameObject is Polygon)
                {
                    gameObject.Destroy();
                }
            }
        }
    }
}