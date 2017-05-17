using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MazeGenerator : MonoBehaviour
    {
        [Serializable]
        public class Maze
        {
            public int XSize;
            public int YSize;
            public int CellsCount;
            public Vector3 InitialPos;
            public float MazeWallLength;

            public Maze(int xsize, int ysize)
            {
                XSize = xsize;
                YSize = ysize;
                CellsCount = XSize*YSize;
                MazeWallLength = WallLength;
                InitialPos = new Vector3(-(XSize * MazeWallLength / 2), 0, -(YSize * MazeWallLength / 2));
            }
        }

        private class Cell
        {
            public bool Visited;

            public GameObject North;
            public GameObject East;
            public GameObject West;
            public GameObject South;
        }

        private class Wall
        {
            private readonly GameObject _wallGameObject;

            public Wall(GameObject wall)
            {
                _wallGameObject = wall;
            }

            public GameObject CreateWall(Vector3 pos, bool isHorizontal = true)
            {
                return Instantiate(_wallGameObject, pos, isHorizontal
                    ? Quaternion.AngleAxis(90, Vector3.right)
                    : Quaternion.Euler(90, 0, 90));
            }
        }

        public Maze maze;
        public GameObject wall;
        public GameObject Floor;
        public GameObject NavFloor;
        public GameObject NavWall;
        public int xSize = 5;
        public int ySize = 5;

        private const float WallLength = 1;
        private GameObject _wallHolder;
        private Cell[] _cells;
        private int _currentCell;
        private int _visitedCells;
        private bool _buildStarted;

        private int _currentNeighbourCell;

        private List<int> _remainingCells;
        private int _backingupCell;
        private int _wallToBreak;
        private const int WallCountInCell = 4;
        private bool _updateWasCalled;

        private void Start()
        {
            /*int layer = GameObjectUtility.GetNavMeshAreaFromName("Not Walkable");
            GameObject g = Selection.activeGameObject;
            StaticEditorFlags staticFlags = GameObjectUtility.GetStaticEditorFlags(g);
            if (staticFlags  != StaticEditorFlags.NavigationStatic)
            {
                staticFlags = StaticEditorFlags.NavigationStatic;
                GameObjectUtility.SetStaticEditorFlags(g, staticFlags);
            }
            GameObjectUtility.SetNavMeshArea(g, layer);*/

            maze = new Maze(xSize, ySize);
            _remainingCells = new List<int>();
            CreateWalls();
            CreateFloor();
            Create3DNavPlane();
        }

        private void Update()
        {
            if (!_updateWasCalled)
            {
                Create3DNavWalls();
            }
            _updateWasCalled = true;

        }

        private void CreateWalls()
        {
            _wallHolder = new GameObject
            {
                name = "Maze Structure",
            };

            Vector3 wallCreatePosition;
            GameObject tempWall;

            //For x Axis
            for (var i = 0; i < ySize; i++)
            {
                for (var j = 0; j <= xSize; j++)
                {
                    wallCreatePosition = new Vector3(maze.InitialPos.x + j * WallLength, 0, maze.InitialPos.y + i * WallLength + WallLength / 2);
                    var wallObject = new Wall(wall);
                    tempWall = wallObject.CreateWall(wallCreatePosition);
                    tempWall.transform.parent = _wallHolder.transform;
                }
            }

            //For y Axis
            for (var i = 0; i <= ySize; i++)
            {
                for (var j = 0; j < xSize; j++)
                {
                    wallCreatePosition = new Vector3(maze.InitialPos.x + j * WallLength + WallLength / 2, 0, maze.InitialPos.y + i * WallLength);
                    var wallObject = new Wall(wall);
                    tempWall = wallObject.CreateWall(wallCreatePosition, false);
                    tempWall.transform.parent = _wallHolder.transform;
                }
            }

            CreateCells();
        }

        private void CreateCells()
        {
            var wallsCount = _wallHolder.transform.childCount;
            var allWalls = new GameObject[wallsCount];
            _cells = new Cell[maze.CellsCount];

            var leftRightWallCounter = 0;
            var childProcess = 0;
            var mazeEndCheck = 0;

            for (var i = 0; i < wallsCount; i++)
            {
                allWalls[i] = _wallHolder.transform.GetChild(i).gameObject;
            }

            for (var i = 0; i < _cells.Length; i++)
            {
                _cells[i] = new Cell
                {
                    East = allWalls[leftRightWallCounter],
                    South = allWalls[childProcess + (xSize + 1) * ySize]
                };

                if (mazeEndCheck == xSize)
                {
                    leftRightWallCounter += 2;
                    mazeEndCheck = 0;
                }
                else
                {
                    leftRightWallCounter++;
                }

                mazeEndCheck++;
                childProcess++;

                _cells[i].West = allWalls[leftRightWallCounter];
                _cells[i].North = allWalls[childProcess + (xSize + 1) * ySize + xSize - 1];
            }
            CreateMaze();
        }

        private void CreateMaze()
        {
            while (_visitedCells < maze.CellsCount)
            {
                if (_buildStarted)
                {
                    GiveMeNeighbour();
                    if (!_cells[_currentNeighbourCell].Visited && _cells[_currentCell].Visited)
                    {
                        BreakWall();
                        _cells[_currentNeighbourCell].Visited = true;
                        _visitedCells++;
                        _remainingCells.Add(_currentCell);
                        _currentCell = _currentNeighbourCell;
                        if (_remainingCells.Count > 0)
                        {
                            _backingupCell = _remainingCells.Count - 1;
                        }
                    }
                }
                else
                {
                    _currentCell = Random.Range(0, maze.CellsCount);
                    _cells[_currentCell].Visited = true;
                    _visitedCells++;
                    _buildStarted = true;
                }
            }
        }

        private void BreakWall()
        {
            switch (_wallToBreak)
            {
                case 1:
                    Destroy(_cells[_currentCell].North);
                    break;
                case 2:
                    Destroy(_cells[_currentCell].East);
                    break;
                case 3:
                    Destroy(_cells[_currentCell].West);
                    break;
                case 4:
                    Destroy(_cells[_currentCell].South);
                    break;
            }
        }

        private void GiveMeNeighbour()
        {
            var length = 0;
            var connectingWall = new int[WallCountInCell];
            var neighbours = new int[WallCountInCell];
            var check = ((_currentCell + 1)/xSize - 1) * maze.XSize + xSize;

            if (_currentCell + 1 < maze.CellsCount && (_currentCell + 1) != check)
            {
                if (!_cells[_currentCell + 1].Visited)
                {
                    neighbours[length] = _currentCell + 1;
                    connectingWall[length] = 3;
                    length++;
                }
            }

            if (_currentCell - 1 >= 0 && _currentCell != check)
            {
                if (!_cells[_currentCell - 1].Visited)
                {
                    neighbours[length] = _currentCell - 1;
                    connectingWall[length] = 2;
                    length++;
                }
            }

            if (_currentCell + xSize < maze.CellsCount)
            {
                if (!_cells[_currentCell + xSize].Visited)
                {
                    neighbours[length] = _currentCell + xSize;
                    connectingWall[length] = 1;
                    length++;
                }
            }

            if (_currentCell - xSize >= 0)
            {
                if (!_cells[_currentCell - xSize].Visited)
                {
                    neighbours[length] = _currentCell - xSize;
                    connectingWall[length] = 4;
                    length++;
                }
            }

            if (length != 0)
            {
                var theChosenOne = Random.Range(0, length);
                _currentNeighbourCell = neighbours[theChosenOne];
                _wallToBreak = connectingWall[theChosenOne];
            }
            else
            {
                if (_backingupCell > 0)
                {
                    _currentCell = _remainingCells[_backingupCell];
                    _backingupCell--;
                }
            }
        }

        private void CreateFloor()
        {
            for (var i = 0; i < xSize; i++)
            {
                for (var j = 0; j < ySize; j++)
                {
                    var floorCreatePosition = new Vector3(maze.InitialPos.x + j * WallLength + WallLength / 2, 0, maze.InitialPos.y + i * WallLength + WallLength / 2);
                    var tempFloor = Instantiate(Floor, floorCreatePosition, Quaternion.AngleAxis(90, Vector3.right));
                    tempFloor.transform.parent = _wallHolder.transform;
                }
            }
        }

        private void Create3DNavPlane()
        {
            var navFloorPosition = new Vector3(maze.InitialPos.x + maze.XSize / 2, -1, maze.InitialPos.y + maze.YSize / 2);
            Instantiate(NavFloor, navFloorPosition, Quaternion.identity);
        }

        private void Create3DNavWalls()
        {
            var created2DWall = GameObject.FindGameObjectsWithTag("wall");
            foreach (var wall2D in created2DWall)
            {
                var wall3DPos = wall2D.transform.position;
                wall3DPos.y = -1;
                Instantiate(NavWall, wall3DPos, wall2D.transform.rotation);
            }
        }
    }
}