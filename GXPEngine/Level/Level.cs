using System;
using System.Collections.Generic;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using NeonArkanoid.Physics;
using TiledParser;
using Polygon = NeonArkanoid.Physics.Polygon;

namespace NeonArkanoid.Level
{
    internal class Level : GameObject
    {
        private readonly Map _map;
        private readonly List<Polygon> _polyList;
        private string _levelName; //useless for now
        private string _tilesheetName;


        public Level(string filename)
        {
            var tmxParser = new TMXParser();
            _map = tmxParser.Parse(filename);

            _tilesheetName = _map.TileSet.Image.Source;

            for (var i = 0; i < _map.ObjectGroup.Length; i++)
            {
                if (_map.ObjectGroup[i].Name == "polygons")
                {
                    _polyList = new List<Polygon>();
                    CreatePolygons(_map.ObjectGroup[i]);
                }
            }
            foreach (var polygon in _polyList)
            {
                AddChild(polygon);
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
                        vectorArray[i] = new Vec2(int.Parse(pointCoords[0]), int.Parse(pointCoords[1]));
                    }

                    string polyID = null;
                    if (tiledObject.Properties != null)
                    {
                        foreach (var property in tiledObject.Properties)
                        {
                            if (property.Property.Name == "id")
                            {
                                polyID = property.Property.Value;
                            }
                        }
                        if (polyID != null)
                        {
                            Polygon poly = new Polygon(vectorArray, polyID);
                            poly.SetXY(tiledObject.X, tiledObject.Y);
                            _polyList.Add(poly);
                        }
                        else
                        {
                            Console.WriteLine("NO ID WAS GIVEN FOR THE POLYGON");
                        }
                    }
                }
            }
        }

        public string GetLevelName()
        {
            return _levelName;
        }
    }
}