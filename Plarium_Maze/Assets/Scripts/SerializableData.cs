using System.IO;
using System.Xml.Linq;

namespace Assets.Scripts
{
    public class SerializableData
    {
        public static void SaveData(string path)
        {
            XDocument xDocument;
            var data = new XElement("Item",
                 new XElement("Name", MazeData.Name),
                 new XElement("Score", MazeData.Score),
                 new XElement("SecondSpent", MazeData.SecondsSpent),
                 new XElement("GameStarted", MazeData.GameStarted),
                 new XElement("FinishReason", MazeData.FinishReason)
             );
            if (!File.Exists(path))
            {
                xDocument = new XDocument();
                xDocument.Add(new XElement("Items", data));
                xDocument.Save(path);
            }
            else
            {
                xDocument = XDocument.Load(path);
                var root = xDocument.Root;
                if (root != null)
                {
                    root.Add(data);
                }
            }

            xDocument.Save(path);
        }
    }
}