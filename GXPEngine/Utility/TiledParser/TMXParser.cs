using System;
using System.IO;
using System.Xml.Serialization;

namespace TiledParser
{
    class TMXParser
    {
        public TMXParser()
        {
        }

        public Map Parse(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Map));

            TextReader reader = new StreamReader(filename);
            Map map = serializer.Deserialize(reader) as Map;
            reader.Close();
            Console.WriteLine(map);
            return map;

        }
    }
}
