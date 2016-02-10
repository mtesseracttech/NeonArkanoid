using System;
using System.Collections.Generic;
using GXPEngine.Utility.TiledParser;
using NeonArkanoid.GXPEngine;
using TiledParser;

namespace NeonArkanoid.Level
{
    internal class Level : GameObject
    {
        private string _tilesheetName;
        private string _levelName; //useless for now
        private Map _map;
        private List<Polygon> _polyList; 


        public Level(string filename)
        {
            var tmxParser = new TMXParser();
            _map = tmxParser.Parse(filename);

            _tilesheetName = _map.TileSet.Image.Source;

            for (int i = 0; i < _map.ObjectGroup.Length; i++)
            {
                if (_map.ObjectGroup[i].Name == "polygons")
                {
                    CreatePolygons(_map.ObjectGroup[i]);
                }

                if (_map.ObjectGroup[i].Name == "images")
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void CreatePolygons(ObjectGroup objectGroup)
        {
            foreach (var tiledObject in objectGroup.TiledObjects)
            {
                if (tiledObject.Polygon != null)
                {
                    throw new NotImplementedException();

                }
            }
        }


        public string GetLevelName()
        {
            return _levelName;
        }
    }
}