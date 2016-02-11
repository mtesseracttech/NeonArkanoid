using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Physics;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : Canvas
    {
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private string _levelName; //useless for now
        private NeonArkanoidGame _game;
        private Ball _ball = new Ball(30, new Vec2 (400, 400), null);
        

        public Level(string filename, NeonArkanoidGame game) : base(game.width, game.height)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            _game = game;
            var tmxParser = new TMXParser();
            _map = tmxParser.Parse(filename);

            //_tilesheetName = _map.TileSet.Image.Source;

            for (var i = 0; i < _map.ObjectGroup.Length; i++)
            {
                if (_map.ObjectGroup[i].Name.ToLower() == "polygons" || _map.ObjectGroup[i].Name.ToLower() == "polygon")
                {
                    _polyList = new List<Polygon>();
                    CreatePolygons(_map.ObjectGroup[i]);
                }
            }
            if (_polyList != null) foreach (var polygon in _polyList) AddChild(polygon);

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
                            if (property.Property.Name.ToLower() == "color" || property.Property.Name.ToLower() == "colour")
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


        
        

    }
}