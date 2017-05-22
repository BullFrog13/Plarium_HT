using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainMenuManager : MonoBehaviour
    {
        public Text ScorePanel;
        public InputField InputField;

        private void Start()
        {
            if (!string.IsNullOrEmpty(MazeData.Name))
            {
                InputField.text = MazeData.Name;
            }
            InputField.onValueChanged.AddListener(delegate { ChangeName(); });
        }

        public void StartGame(string gameSceneName)
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void LoadScores()
        {
            ScorePanel.text = "";
            ScorePanel = ScorePanel.GetComponent<Text>();
            if (File.Exists("data.xml"))
            {
                var scores = XDocument.Load("data.xml");

                // serialization ceased to work

                if (scores.Root != null)
                {
                    var test = from datas in scores.Root.Elements("Item")
                        select new SerializableData
                        {
                            Name = (string) datas.Element("Name"),
                            Score = (int) datas.Element("Score"),
                            SecondSpent = (float) datas.Element("SecondSpent"),
                            GameStarted = (DateTime) datas.Element("GameStarted"),
                            FinishReason = (string) datas.Element("FinishReason")
                        };
                    var list = test.OrderByDescending(x => x.GameStarted).ToList();

                    foreach (var item in list)
                    {
                        var sb = new StringBuilder();
                        sb.AppendFormat(
                            "\nPlayer {0} started at {1} and played {2} scored {3} and exited because was {4}\n",
                            item.Name, item.GameStarted, item.SecondSpent, item.Score, item.FinishReason);
                        ScorePanel.text += sb;
                    }
                }
            }
            else
            {
                ScorePanel.text += "No games yet";
            }
        }

        private void ChangeName()
        {
            MazeData.Name = InputField.text;
        }
    }
}