using System.IO;

namespace NeonArkanoid.Utility
{
    internal class FileReader
    {
        public static int[,] levelMaker(int levelnr, int width, int height)
        {
            var level = new int[height, width];

            var reader = new StreamReader("level_" + levelnr + ".txt");
            var fileData = reader.ReadToEnd();
            reader.Close();

            var lines = fileData.Split('\n');
            for (var i = 0; i < height; i++)
            {
                var columns = lines[i].Split(',');
                for (var j = 0; j < width; j++)
                {
                    level[i, j] = int.Parse(columns[j]);
                }
            }
            return level;
        }
    }
}