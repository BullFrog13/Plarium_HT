using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public int XSize;
        public int YSize;
        public GameObject Coin;
        public GameObject Player;
        public GameObject Zombie;
        public GameObject Mummy;
        public GameObject GroundTile;
        public GameObject WallTile;

        public int MaxCoinCount;
        public float CoinAddingRangeTime;
        public int CoinsNeededForSecondZombieSpawn;
        public int CoinsNeededForMummySpawn;

        private MazeGenerator _mazeGenerator;
        private GameObject _mazeHolder;
        private bool _mummyIsEnabled;
        private bool _secondZombieIsEnabled;
        private float _coinTimer;
        public Node[,] Graph;

        private void Awake()
        {
            _mazeHolder = new GameObject { name = "Maze holder" };
           _mazeGenerator = new MazeGenerator();
            _mazeGenerator.GenerateMaze(XSize, YSize);
            GeneratePathfindingGraph();
            VisualizeLabyrinth();

            _secondZombieIsEnabled = false;
            _mummyIsEnabled = false;
            _coinTimer = CoinAddingRangeTime;

            AddItemIntoMaze(Player);
            AddItemIntoMaze(Zombie);
        }

        private void Update()
        {
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

            if (MazeData.CollectedCoins == CoinsNeededForSecondZombieSpawn && !_secondZombieIsEnabled)
            {
                AddItemIntoMaze(Zombie);
                _secondZombieIsEnabled = true;
            }

            if (MazeData.CollectedCoins == CoinsNeededForMummySpawn && !_mummyIsEnabled)
            {
                AddItemIntoMaze(Mummy);
                _mummyIsEnabled = true;
            }
        }

        // TODO use tiles instead of Nodes
        public class Node
        {
            // these are edges remake to array TODO
            public List<Node> _neighbours;
            public int X;
            public int Y;
            public bool Walkable;

            public Node()
            {
                _neighbours = new List<Node>();
            }

            public float DistanceTo(Node otherNode)
            {
                return Vector2.Distance(new Vector2(X, Y), new Vector2(otherNode.X, otherNode.Y));
            }
        }

        private void GeneratePathfindingGraph()
        {
            Graph = new Node[XSize * 2, YSize * 2];

            for (var i = 0; i < YSize*2; i++)
            {
                for (var j = 0; j < XSize*2; j++)
                {
                    Graph[j, i] = new Node
                    {
                        X = j,
                        Y = i,
                        Walkable = _mazeGenerator.Tiles[i, j].Walkable
                    };

                }
            }
            for (var i = 0; i < YSize * 2; i++)
            {
                for (var j = 0; j < XSize * 2; j++)
                {
                    // adding nodes
                    if (j > 0)
                    {
                        Graph[j, i]._neighbours.Add(Graph[j - 1, i]);
                    }
                    if (j < XSize * 2 - 1)
                    {
                        Graph[j, i]._neighbours.Add(Graph[j + 1, i]);
                    }
                    if (i > 0)
                    {
                        Graph[j, i]._neighbours.Add(Graph[j, i - 1]);
                    }
                    if (i < YSize * 2 - 1)
                    {
                        Graph[j, i]._neighbours.Add(Graph[j, i + 1]);
                    }
                }
            }
        }

        private void VisualizeLabyrinth()
        {
            for (var i = 0; i <= YSize * 2; i++)
            {
                for (var j = 0; j <= XSize * 2; j++)
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
    }
}