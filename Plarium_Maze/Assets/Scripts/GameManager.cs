using System.Collections.Generic;
using System.Linq;
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
        public GameObject TestObjectForNavigation;
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
        private Transform _instanceOfTestObjectNav;
        private Node[,] _graph;
        private List<Node> _currentPath;

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

            //AddItemIntoMaze(Player);
            AddItemIntoMaze(TestObjectForNavigation);
            _instanceOfTestObjectNav = GameObject.Find("Test Object(Clone)").transform;
            _instanceOfTestObjectNav.GetComponent<Unit>().tileX = (int)_instanceOfTestObjectNav.position.x;
            _instanceOfTestObjectNav.GetComponent<Unit>().tileY = (int)_instanceOfTestObjectNav.position.y;
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

        // class for pathfinding graph
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
            _graph = new Node[XSize * 2, YSize * 2];

            for (var i = 0; i < YSize*2; i++)
            {
                for (var j = 0; j < XSize*2; j++)
                {
                    _graph[j, i] = new Node
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
                        _graph[j, i]._neighbours.Add(_graph[j - 1, i]);
                    }
                    if (j < XSize * 2 - 1)
                    {
                        _graph[j, i]._neighbours.Add(_graph[j + 1, i]);
                    }
                    if (i > 0)
                    {
                        _graph[j, i]._neighbours.Add(_graph[j, i - 1]);
                    }
                    if (i < YSize * 2 - 1)
                    {
                        _graph[j, i]._neighbours.Add(_graph[j, i + 1]);
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
                        var ct = tempTile.GetComponent<ClickableTile>();
                        ct.X = i;
                        ct.Y = j;
                        ct.Man = this;
                    }
                    tempTile.transform.parent = _mazeHolder.transform;
                }
            }
        }

        public Vector2 TileCoordToWorldCoord(int x, int y)
        {
            return new Vector2(x, y);
        }

        public void GenerateTheShortestPath(int x, int y)
        {
            _instanceOfTestObjectNav.GetComponent<Unit>().CurrentPath = null;

            var dist = new Dictionary<Node, float>();
            var prev = new Dictionary<Node, Node>();

            var unvisited = new List<Node>();

            Node source =
                _graph[
                    _instanceOfTestObjectNav.GetComponent<Unit>().tileX,
                    _instanceOfTestObjectNav.GetComponent<Unit>().tileY];
            Node target = _graph[x, y];

            dist[source] = 0;
            prev[source] = null;

            // init everything to INFINITE.
            foreach (var node in _graph)
            {
                if (node != source)
                {
                    dist[node] = Mathf.Infinity;
                    prev[node] = null;
                }

                unvisited.Add(node);
            }

            while (unvisited.Count > 0)
            {
                Node u = null;

                foreach (var node in unvisited)
                {
                    if (u == null || dist[node] < dist[u])
                    {
                        u = node;
                    }
                }

                if (u == target)
                {
                    break;
                }

                unvisited.Remove(u);

                foreach (var neighbour in u._neighbours)
                {
                    float alt;
                    if (_graph[neighbour.X, neighbour.Y].Walkable)
                    {
                        alt = dist[u] + u.DistanceTo(neighbour);
                    }
                    else
                    {
                        alt = dist[u] + Mathf.Infinity;
                    }

                    if (alt < dist[neighbour])
                    {
                        dist[neighbour] = alt;
                        prev[neighbour] = u;
                    }
                }
            }

            if (prev[target] == null)
            {
                return;
            }

            _currentPath = new List<Node>();

            Node currentNode = target;

            while (currentNode != null)
            {
                _currentPath.Add(currentNode);
                currentNode = prev[currentNode];
            }

            _currentPath.Reverse();

            _instanceOfTestObjectNav.GetComponent<Unit>().CurrentPath = _currentPath;
            _instanceOfTestObjectNav.GetComponent<Unit>().tileX = x;
            _instanceOfTestObjectNav.GetComponent<Unit>().tileY = y;
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