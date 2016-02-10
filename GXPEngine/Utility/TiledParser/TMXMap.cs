﻿using System.Xml.Serialization;

namespace TiledParser
{
    [XmlRoot("map")]
    public class Map
    {
        [XmlElement("tileset")] public TileSet TileSet;

        [XmlElement("objectgroup")] public ObjectGroup[] ObjectGroup;

        [XmlAttribute("version")] public string Version;

        public override string ToString()
        {
            string returnString = "";
            foreach (var objectGroup in ObjectGroup)
            {
                returnString += "\n\nObjectlayer: " + objectGroup.Name + "\n";
                foreach (var tiledObject in objectGroup.TiledObjects)
                {
                    returnString += "\nName: " + tiledObject.Name +
                                    "\nGroup ID: " + tiledObject.GID +
                                    "\nID: " + tiledObject.ID +
                                    "\nDimensions: " + tiledObject.Height + "x" + tiledObject.Width +
                                    "\nCoordinates: " + tiledObject.X + "," + tiledObject.Y +
                                    "\nRotation: " + tiledObject.Rotation +
                                    "\n";

                    if (tiledObject.Polygon != null)
                    {
                        returnString += "\nPolyPoints: " + tiledObject.Polygon.Points;
                    }

                    if (tiledObject.Properties != null)
                    {
                        foreach (var properties in tiledObject.Properties)
                        {
                            returnString += "\n" + properties.Property.Name + " = " + properties.Property.Value + "\n";
                        }
                    }
                }
            }
            return returnString;
        }
    }

    [XmlRoot("tileset")]
    public class TileSet
    {
        [XmlAttribute("firstgid")] public int FirstGID;

        [XmlElement("image")] public Image Image;

        [XmlAttribute("name")] public string Name;

        [XmlAttribute("tileheight")] public int TileHeight;

        [XmlAttribute("tilewidth")] public int TileWidth;
    }

    [XmlRoot("image")]
    public class Image
    {
        [XmlAttribute("height")] public int Height;

        [XmlAttribute("source")] public string Source;

        [XmlAttribute("width")] public int Width;
    }

    [XmlRoot("objectgroup")]
    public class ObjectGroup
    {
        [XmlAttribute("name")] public string Name;

        [XmlElement("object")] public TiledObject[] TiledObjects;
    }

    [XmlRoot("object")]
    public class TiledObject
    {
        [XmlAttribute("gid")] public int GID;

        [XmlAttribute("height")] public int Height;

        [XmlAttribute("id")] public int ID;

        [XmlAttribute("name")] public string Name;

        [XmlElement("properties")] public Properties[] Properties;

        [XmlElement("polygon")] public Polygon Polygon;

        [XmlAttribute("rotation")] public float Rotation;

        [XmlAttribute("width")] public int Width;

        [XmlAttribute("x")] public float X;

        [XmlAttribute("y")] public float Y;
    }


    [XmlRoot("polygon")]
    public class Polygon
    {
        [XmlAttribute("points")] public string Points;
    }

    [XmlRoot("properties")]
    public class Properties
    {
        [XmlElement("property")] public Property Property;
    }

    [XmlRoot("property")]
    public class Property
    {
        [XmlAttribute("name")] public string Name;

        [XmlAttribute("value")] public string Value;
    }
}