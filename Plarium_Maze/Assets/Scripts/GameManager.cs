using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("Textures settings")]
        public GameObject Coin;
        public GameObject Player;
        public GameObject Zombie;
        public GameObject Mummy;
        public GameObject GroundTile;
        public GameObject WallTile;

        [Header("UI settings")]
        public GameObject GameEndPanel;

        [Header("Maze logic settings")]
        public int MaxCoinCount;
        public float CoinAddingRangeTime;
        public int CoinsNeededForSecondZombieSpawn;
        public int CoinsNeededForMummySpawn;

        private MazeGenerator _mazeGenerator;
        private GameObject _mazeHolder;
        private bool _mummyIsEnabled;
        private bool _secondZombieIsEnabled;
        private float _coinTimer;

        private void Awake()
        {
            // If case we want to test it.
            if (MazeData.XSize == 0 || MazeData.YSize == 0)
            {
                MazeData.XSize = 5;
                MazeData.YSize = 5;
            }

            ResetValues();
            GameEndPanel.SetActive(false);

            _mazeHolder = new GameObject { name = "Maze holder" };
           _mazeGenerator = new MazeGenerator();
            _mazeGenerator.GenerateMaze(MazeData.XSize, MazeData.YSize);
            GeneratePathfindingGraph();
            VisualizeLabyrinth();

            _secondZombieIsEnabled = false;
            _mummyIsEnabled = false;
            _coinTimer = CoinAddingRangeTime;

            AddItemIntoMaze(Player);
            AddItemIntoMaze(Zombie);
        }

        private void Start()
        {
            MazeData.GameStarted = DateTime.Now;
        }

        private void Update()
        {
            MazeData.SecondsSpent += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                MazeData.FinishReason = "Was too weak to fight further";
                SerializableData.SaveData("data.xml");
                Application.Quit();
            }
            if (PlayerManager.IsDead)
            {
                if (!GameEndPanel.activeInHierarchy)
                {
                    ShowEndGamePanel();
                    SerializableData.SaveData("data.xml");
                }
            }

            if (MazeData.CurrentCointCount < MaxCoinCount)
            {
                _coinTimer -= Time.deltaTime;

                if (_coinTimer <= 0)
                {
                    AddItemIntoMaze(Coin);
                    _coinTimer = CoinAddingRangeTime;
                    MazeData.CurrentCointCount++;
                }
            }

            if (MazeData.Score >= CoinsNeededForSecondZombieSpawn && !_secondZombieIsEnabled)
            {
                AddItemIntoMaze(Zombie);
                _secondZombieIsEnabled = true;
            }

            if (MazeData.Score >= CoinsNeededForMummySpawn && !_mummyIsEnabled)
            {
                AddItemIntoMaze(Mummy);
                _mummyIsEnabled = true;
            }
        }

        private void ShowEndGamePanel()
        {
            GameEndPanel.SetActive(true);
        }

        private void GeneratePathfindingGraph()
        {
            MazeData.Graph = new Node[MazeData.XSize * 2, MazeData.YSize * 2];

            for (var i = 0; i < MazeData.YSize *2; i++)
            {
                for (var j = 0; j < MazeData.XSize *2; j++)
                {
                    MazeData.Graph[j, i] = new Node
                    {
                        X = j,
                        Y = i,
                        Walkable = _mazeGenerator.Tiles[i, j].Walkable
                    };

                }
            }

            for (var i = 0; i < MazeData.YSize * 2; i++)
            {
                for (var j = 0; j < MazeData.XSize * 2; j++)
                {
                    // adding nodes
                    if (j > 0)
                    {
                        MazeData.Graph[j, i].Neighbours.Add(MazeData.Graph[j - 1, i]);
                    }

                    if (j < MazeData.XSize * 2 - 1)
                    {
                        MazeData.Graph[j, i].Neighbours.Add(MazeData.Graph[j + 1, i]);
                    }

                    if (i > 0)
                    {
                        MazeData.Graph[j, i].Neighbours.Add(MazeData.Graph[j, i - 1]);
                    }

                    if (i < MazeData.YSize * 2 - 1)
                    {
                        MazeData.Graph[j, i].Neighbours.Add(MazeData.Graph[j, i + 1]);
                    }
                }
            }
        }

        private void VisualizeLabyrinth()
        {
            for (var i = 0; i <= MazeData.YSize * 2; i++)
            {
                for (var j = 0; j <= MazeData.XSize * 2; j++)
                {
                    bool isWalkable = _mazeGenerator.Tiles[j, i].Walkable;
                    var tempTile = Instantiate(isWalkable ? GroundTile : WallTile, new Vector3(i, j, 0), Quaternion.identity);
                    if (isWalkable)
                    {
                        var ct = new Tile(j, i)
                        {
                            X = i,
                            Y = j
                        };
                    }
                    tempTile.transform.parent = _mazeHolder.transform;
                }
            }
        }

        private void AddItemIntoMaze(GameObject objectToAdd)
        {
            var chosenTile = GetRandomTileForMazeItems();
            Instantiate(objectToAdd, chosenTile.transform.position, Quaternion.identity);
        }

        private GameObject GetRandomTileForMazeItems()
        {
            var availableTiles = GameObject.FindGameObjectsWithTag("ground");
            var randomTileIndex = Random.Range(0, availableTiles.Length);
            return availableTiles[randomTileIndex];
        }

        private void ResetValues()
        {
            MazeData.Score = 0;
            MazeData.CurrentCointCount = 0;
            MazeData.SecondsSpent = 0;
            MazeData.FinishReason = "Unknown";
            PlayerManager.IsDead = false;
        }
    }
}