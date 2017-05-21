using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MummyManager : MonoBehaviour {
        public float Speed;
        public GameObject Player;
        private GameManager _gm;
        private bool _followPlayer;
        private GameObject _chosenTileToFollow;

        private int _tileX;
        private int _tileY;
        private float _horizontalSpeed;
        private float _verticalSpeed;
        private bool _movingRight;
        private bool _movingLeft;
        private bool _movingUp;
        private bool _movingDown;
        private int _cachedCoinsCount;

        public List<GameManager.Node> CurrentPath;

        public void Start()
        {
            _followPlayer = false;
            _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
            Player = GameObject.Find("Player(Clone)");
            _tileX = (int)transform.position.x;
            _tileY = (int)transform.position.y;
        }

        private void Update()
        {
            if (MazeData.CollectedCoins == 10)
            {
                _followPlayer = true;
            }

            if (MazeData.CollectedCoins > 10 && MazeData.CollectedCoins != _cachedCoinsCount)
            {
                Speed *= 1.05F;
                _cachedCoinsCount++;
            }

            if (_followPlayer)
            {
                FollowObject(Player);
                MoveObject();
            }
            else
            {
                if (CurrentPath == null)
                {
                    _chosenTileToFollow = GetRandomTileToMove();

                }

                FollowObject(_chosenTileToFollow);
                MoveObject();
            }
        }

        private void MoveObject()
        {
            var actualSpeedX = _horizontalSpeed * Speed;
            var actualSpeedY = _verticalSpeed * Speed;
            GetComponent<Rigidbody2D>().velocity = new Vector2(actualSpeedX, actualSpeedY);
        }

        private void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.tag == "zombie" || collision2D.gameObject.tag == "mummy")
            {
                Physics2D.IgnoreCollision(collision2D.collider, GetComponent<Collider2D>());
            }
        }

        private void FollowObject(GameObject targetGameObject)
        {
            GenerateTheShortestPath((int)Math.Round(targetGameObject.transform.position.x), (int)Math.Round(targetGameObject.transform.position.y));
            var rb = GetComponent<Rigidbody2D>();

            if (CurrentPath != null)
            {
                if (_tileX < CurrentPath[1].X)
                {
                    _movingRight = true;
                }
                if (_tileX > CurrentPath[1].X)
                {
                    _movingLeft = true;
                }
                if (_tileY < CurrentPath[1].Y)
                {
                    _movingUp = true;
                }
                if (_tileY > CurrentPath[1].Y)
                {
                    _movingDown = true;
                }

                if (_movingLeft || _movingDown)
                {
                    _tileX = (int)Math.Ceiling(transform.position.x);
                    _tileY = (int)Math.Ceiling(transform.position.y);
                }
                if (_movingRight || _movingUp)
                {
                    _tileX = (int)Mathf.Floor(transform.position.x);
                    _tileY = (int)Mathf.Floor(transform.position.y);
                }

                GameManager.Node nextTile = null;
                if (CurrentPath != null)
                {
                    nextTile = CurrentPath[1];
                }

                if (_tileX < nextTile.X)
                {
                    _verticalSpeed = 0;
                    _horizontalSpeed = 1;
                    _movingRight = true;
                    _movingDown = false;
                    _movingLeft = false;
                    _movingUp = false;
                }
                else if (_tileY < nextTile.Y)
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = 1;
                    _movingRight = false;
                    _movingDown = false;
                    _movingLeft = false;
                    _movingUp = true;
                }
                else if (_tileX > nextTile.X)
                {
                    _verticalSpeed = 0;
                    _horizontalSpeed = -1;
                    _movingRight = false;
                    _movingDown = false;
                    _movingLeft = true;
                    _movingUp = false;
                }
                else if (_tileY > nextTile.Y)
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = -1;
                    _movingRight = false;
                    _movingDown = true;
                    _movingLeft = false;
                    _movingUp = false;
                }
                else
                {
                    _horizontalSpeed = 0;
                    _verticalSpeed = 0;
                }

                if (_movingRight)
                {
                    //if(Math.Abs(transform.position.x - nextTile.X) <= Math.Abs(transform.position.x * .001))
                    if (_movingRight && _horizontalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(_tileX, transform.position.y, 0);
                        _movingRight = false;
                    }
                }
                if (_movingLeft && _horizontalSpeed == 0)
                {
                    //if (Math.Abs(transform.position.x - nextTile.X) <= Math.Abs(transform.position.x * .001))
                    if (_movingLeft && _horizontalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(_tileX, transform.position.y, 0);
                        _movingLeft = false;
                    }
                }
                if (_movingUp && _verticalSpeed == 0)
                {
                    //if (Math.Abs(transform.position.y - nextTile.Y) <= Math.Abs(transform.position.y * .001))
                    if (_movingUp && _verticalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(transform.position.x, _tileY, 0);
                        _movingUp = false;
                    }
                }
                if (_movingDown && _verticalSpeed == 0)
                {
                    //if (Math.Abs(transform.position.y - nextTile.Y) <= Math.Abs(transform.position.y * .001))
                    if (_movingDown && _verticalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(transform.position.x, _tileY, 0);
                        _movingDown = false;
                    }
                }

                var currentNode = 0;

                while (currentNode < CurrentPath.Count - 1)
                {
                    var start = new Vector3(CurrentPath[currentNode].X, CurrentPath[currentNode].Y);
                    var end = new Vector3(CurrentPath[currentNode + 1].X, CurrentPath[currentNode + 1].Y);

                    Debug.DrawLine(start, end, Color.green);

                    currentNode++;
                }
            }
        }

        public void GenerateTheShortestPath(int x, int y)
        {
            CurrentPath = null;

            var dist = new Dictionary<GameManager.Node, float>();
            var prev = new Dictionary<GameManager.Node, GameManager.Node>();

            var unvisited = new List<GameManager.Node>();

            var source = _gm.Graph[_tileX, _tileY];
            var target = _gm.Graph[x, y];

            dist[source] = 0;
            prev[source] = null;

            // init everything to INFINITE.
            foreach (var node in _gm.Graph)
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
                GameManager.Node u = null;

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
                    if (_gm.Graph[neighbour.X, neighbour.Y].Walkable)
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

            CurrentPath = new List<GameManager.Node>();

            GameManager.Node currentNode = target;

            while (currentNode != null)
            {
                CurrentPath.Add(currentNode);
                currentNode = prev[currentNode];
            }

            CurrentPath.Reverse();
        }

        private GameObject GetRandomTileToMove()
        {
            var availableTiles = GameObject.FindGameObjectsWithTag("ground");
            var randomTileIndex = Random.Range(0, availableTiles.Length);
            return availableTiles[randomTileIndex];
        }
    }
}