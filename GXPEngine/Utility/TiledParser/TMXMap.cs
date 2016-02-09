using System.Xml.Serialization;

namespace TiledParser
{
    [XmlRoot("map")]
    public class Map
    {
        [XmlAttribute("height")] public int Height;

        [XmlElement("tileset")] public TileSet TileSet;

        [XmlElement("objectgroup")] public ObjectGroup[] ObjectGroup;

        [XmlAttribute("version")] public string Version;

        [XmlAttribute("orientation")] public string Orientation;

        [XmlAttribute("renderorder")] public string RenderOrder;

        [XmlAttribute("tileheight")] public int TileHeight;

        [XmlAttribute("tilewidth")] public int TileWidth;

        [XmlAttribute("width")] public int Width;

        public override string ToString()
        {
            var returnString =
                "Map Version: " + Version +
                "\nMap Orientation: " + Orientation +
                "\nMap Render Order: " + RenderOrder +
                "\nMap Height: " + Height +
                "\nMap Width: " + Width + 
                "\nMap Tilesize: " + TileHeight + "x" + TileWidth +
                "\n\nTileset Name: " + TileSet.Name +
                "\nTileset Source File: " + TileSet.Image.Source + 
                "\nTileset FirstGID: " + TileSet.FirstGID + 
                "\nTileset Tile Size: " + TileSet.TileHeight + "x" + TileSet.TileWidth + "\n";

            returnString += "Tiles: ";

            returnString += "\n\nObjects: " + "\n";
            foreach (var objectGroup in ObjectGroup)
            {
                returnString += objectGroup.Name + "\n";
                foreach (var tiledObject in objectGroup.TiledObjects)
                {
                    returnString += "\nName: " + tiledObject.Name +
                                    "\nGroup ID: "+  tiledObject.GID +
                                    "\nID: " + tiledObject.ID +
                                    "\nDimensions: " + tiledObject.Height + "x" + tiledObject.Width +
                                    "\nCoordinates: " + tiledObject.X + "," + tiledObject.Y +
                                    "\nRotation: " + tiledObject.Rotation +
                                    "\n";

                    if (tiledObject.Polygon != null) returnString += "\nPolyPoints: " + tiledObject.Polygon.Points;
                    if (tiledObject.Properties != null)
                    {
                        foreach (Properties properties in tiledObject.Properties)
                        {
                            returnString += properties.Property.Name + " = " + properties.Property.Value + "\n";
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

        [XmlAttribute("name")] public string Name;

        [XmlElement("image")] public Image Image;

        [XmlAttribute("tileheight")] public int TileHeight;

        [XmlAttribute("tilewidth")] public int TileWidth;
    }

    [XmlRoot("image")]
    public class Image
    {
        [XmlAttribute("source")]
        public string Source;
        [XmlAttribute("width")]
        public int Width;
        [XmlAttribute("height")]
        public int Height;
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
        [XmlAttribute("name")] public string Name;

        [XmlAttribute("gid")] public int GID;

        [XmlAttribute("height")] public int Height;

        [XmlAttribute("id")] public int ID;

        [XmlElement("properties")] public Properties[] Properties;

        [XmlAttribute("rotation")] public float Rotation;

        [XmlAttribute("width")] public int Width;

        [XmlAttribute("x")] public int X;

        [XmlAttribute("y")] public int Y;

        [XmlElement("polygon")]
        public Polygon Polygon;
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