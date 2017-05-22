using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ZombieManager : MonoBehaviour
    {
        public float Speed;
        public int CoinsToFollowPlayer;
        public float SpeedIncreasePercent;

        private GameManager _gm;
        private GameObject _chosenTileToFollow;
        private GameObject _player;

        private int _tileX;
        private int _tileY;
        private float _horizontalSpeed;
        private float _verticalSpeed;
        private bool _followPlayer;
        private bool _movingRight;
        private bool _movingLeft;
        private bool _movingUp;
        private bool _movingDown;
        private int _cachedScore;
        private bool _facingRight;

        public List<Node> CurrentPath;

        public void Start()
        {
            _facingRight = false;
            _followPlayer = false;
            _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _tileX = (int)transform.position.x;
            _tileY = (int)transform.position.y;
        }

        private void Update()
        {
            if (MazeData.Score == CoinsToFollowPlayer && !_followPlayer)
            {
                _followPlayer = true;
                _cachedScore = MazeData.Score;
            }

            if (MazeData.Score > CoinsToFollowPlayer && MazeData.Score > _cachedScore)
            {
                Speed += Speed / 100 * SpeedIncreasePercent;
                _cachedScore++;
            }

            if (_followPlayer)
            {
               FollowObject(_player);
               MoveObject();
            }
            else
            {
                if (CurrentPath == null)
                {
                    _chosenTileToFollow = GetRandomTileToMove();

                }

                FollowObject(_chosenTileToFollow);
                Flip();
                MoveObject();
            }
        }

        private void MoveObject()
        {

            var actualSpeedX = _horizontalSpeed * Speed;
            var actualSpeedY = _verticalSpeed * Speed;
            GetComponent<Rigidbody2D>().velocity = new Vector2(actualSpeedX, actualSpeedY); 
        }

        private void Flip()
        {
            if (_horizontalSpeed > 0 && !_facingRight || _horizontalSpeed < 0 && _facingRight)
            {
                _facingRight = !_facingRight;
                Vector2 temp = transform.localScale;
                temp.x *= -1;
                transform.localScale = temp;
            }
        }

        private void FollowObject(GameObject targetGameObject)
        {
            GenerateTheShortestPath((int)Math.Round(targetGameObject.transform.position.x), (int)Math.Round(targetGameObject.transform.position.y));

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

                Node nextTile = null;
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
                    if (_movingRight && _horizontalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(_tileX, transform.position.y, 0);
                        _movingRight = false;
                    }
                }
                if (_movingLeft && _horizontalSpeed == 0)
                {
                    if (_movingLeft && _horizontalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(_tileX, transform.position.y, 0);
                        _movingLeft = false;
                    }
                }
                if (_movingUp && _verticalSpeed == 0)
                {
                    if (_movingUp && _verticalSpeed == 0)
                    {
                        CurrentPath.Remove(nextTile);
                        transform.position = new Vector3(transform.position.x, _tileY, 0);
                        _movingUp = false;
                    }
                }
                if (_movingDown && _verticalSpeed == 0)
                {
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

            var dist = new Dictionary<Node, float>();
            var prev = new Dictionary<Node, Node>();

            var unvisited = new List<Node>();

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

                foreach (var neighbour in u.Neighbours)
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

            CurrentPath = new List<Node>();

            Node currentNode = target;

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