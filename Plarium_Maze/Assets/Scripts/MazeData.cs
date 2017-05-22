using System;
using System.IO;
using System.Xml.Serialization;

namespace Assets.Scripts
{
    [XmlRoot("PlayersData")]
    public class MazeData
    {
        public const int WallLength = 1;

        public static int XSize;

        public static int YSize;

        public static int CurrentCointCount = 0;

        [XmlElement("Name")]
        public static string Name;

        [XmlElement("Score")]
        public static int Score;

        [XmlElement("GameTime")]
        public static float SecondsSpent;

        [XmlElement("StartDate")]
        public static DateTime GameStarted;

        [XmlElement("FinishReason")]
        public static string FinishReason;

        public void SaveData(string path)
        {
            var serializer = new XmlSerializer(typeof(MazeData));
            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static MazeData Load(string path)
        {
            var serializer = new XmlSerializer(typeof(MazeData));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as MazeData;
            }
        }
    }
}