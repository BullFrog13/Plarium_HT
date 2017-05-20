using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MazeGenerator : MonoBehaviour
    {
        public class Tile
        {
            public int X;
            public int Y;
            public int TileType;

            public Tile(int x, int y, int tileType)
            {
                X = x;
                Y = y;
                TileType = tileType;
            }
        }

        public TileType[] TileTypes;
        private Tile[,] _tiles;

        private class Cell
        {
            public bool Visited;

            public int X;
            public int Y;
        }

        public GameObject Wall;
        public GameObject Floor;
        public int XSize;
        public int YSize;

        private Cell[] _cells;
        private int _cellsCount;
        private GameObject _mazeHolder;

        private enum Directions
        {
            North = 1,
            West = 2,
            East = 3,
            South = 4
        }

        private void Awake()
        {
            _mazeHolder = new GameObject { name = "Maze holder" };
            MazeData.XSize = XSize;
            MazeData.YSize = YSize;
            _cellsCount = XSize*YSize;

            _tiles = new Tile[XSize * 2 + 1, YSize * 2 + 1];

            for (var i = 0; i <= XSize*2; i++)
            {
                for (var j = 0; j <= YSize*2; j++)
                {
                    _tiles[j, i] = new Tile(j, i, 0);
                }
            }

            CreateNotBreakableWalls();
            CreateBreakableWalls();
            CreateCells();
            CreateLabyrinth();
            InstantiateMap();
        }

        private void InstantiateMap()
        {
            for (var i = 0; i <= YSize * 2; i++)
            {
                for (var j = 0; j <= XSize * 2; j++)
                {
                    TileType tt = TileTypes[_tiles[i, j].TileType];
                    var tempTile = Instantiate(tt.TileVisualPrefab, new Vector3(i, j, 0), Quaternion.identity);
                    tempTile.transform.parent = _mazeHolder.transform;
                }
            }
        }

        private void CreateNotBreakableWalls()
        {
            // iteration for Y
            for (var i = 0; i <= MazeData.YSize * 2; i++)
            {
                // iteration for X.
                for (var j = 0; j <= MazeData.XSize * 2; j++)
                {
                    // check for outside walls
                    if (j == 0 || i == 0 || j == MazeData.XSize * 2 || i == MazeData.YSize * 2)
                    {
                        // 1 represents wall for now
                        _tiles[j, i] = new Tile(j, i, 1);
                    }
                    // concerning that walls are squares there are some walls inside that cannot be broken
                    if (j != 0 && i != 0 && j % 2 == 0 && i % 2 == 0)
                    {
                        _tiles[j, i] = new Tile(j, i, 1);
                    }
                }
            }
        }

        private void CreateBreakableWalls()
        {
            // basically creating breakable walls inside of our maze
            for (var i = 0; i <= MazeData.YSize * 2; i++)
            {
                for (var j = 0; j <= MazeData.XSize * 2; j++)
                {
                    if (i != 0 && j != 0 && i != XSize * 2 && j != YSize * 2)
                    {
                        if (i % 2 != 0 && j % 2 == 0 || i % 2 == 0 && j % 2 != 0)
                        {
                            _tiles[j, i] = new Tile(j, i, 2);
                        }
                    }
                }
            }
        }

        private void CreateCells()
        {
            // this method is just for assigning right walls for right cells for A* Depth algorithm needs
            _cells = new Cell[XSize * YSize];
            var cellsCounter = 0;
            for (var i = 1; i < YSize * 2; i++)
            {
                for (var j = 1; j < XSize * 2; j++)
                {
                    if (_tiles[j, i].TileType == 0)
                    {
                        _cells[cellsCounter] = new Cell
                        {
                            X = j,
                            Y = i,
                            Visited = false
                        };
                        cellsCounter++;
                    }
                }
            }
        }

        private void CreateLabyrinth()
        {
            // implementation of an A* Depth algorithm is beneath

            var currentCell = PickRandomIntialCell();
            _cells[currentCell].Visited = true;
            var stack = new Stack<int>();
            stack.Push(currentCell);
            var countOfVisitedCells = 1;

            while (countOfVisitedCells < _cells.Length)
            {
                var randomChosenNeighbour = PickRandomNotVisitedCellIndex(stack.Peek());
                if (randomChosenNeighbour != -1)
                {
                    _cells[randomChosenNeighbour].Visited = true;
                    stack.Push(randomChosenNeighbour);
                    countOfVisitedCells++;
                }
                else
                {
                    stack.Pop();
                }
            }
        }

        private int PickRandomNotVisitedCellIndex(int currentCellIndex)
        {
            Cell cell = _cells[currentCellIndex];
            var possibleDirections = new List<Directions>();

            if (currentCellIndex + XSize < _cellsCount &&
                !_cells[currentCellIndex + XSize].Visited && _tiles[cell.X, cell.Y + 1].TileType == 2)
            {
                possibleDirections.Add(Directions.North);
            }

            if (currentCellIndex - 1 >=  0 && !_cells[currentCellIndex - 1].Visited && _tiles[cell.X - 1, cell.Y].TileType == 2)
            {
                possibleDirections.Add(Directions.West);
            }

            if (currentCellIndex + 1 < _cellsCount && !_cells[currentCellIndex + 1].Visited && _tiles[cell.X + 1, cell.Y].TileType == 2)
            {
                possibleDirections.Add(Directions.East);
            }

            if (currentCellIndex - XSize >= 0 &&
                !_cells[currentCellIndex - XSize].Visited && _tiles[cell.X, cell.Y - 1].TileType == 2)
            {
                possibleDirections.Add(Directions.South);
            }

            if (possibleDirections.Any())
            {
                var randomDirectionIndex = Random.Range(0, possibleDirections.Count);
                switch (possibleDirections[randomDirectionIndex])
                {
                    case Directions.North:
                        _tiles[cell.X, cell.Y + 1].TileType = 0;

                        return currentCellIndex + XSize;
                    case Directions.West:
                        _tiles[cell.X - 1, cell.Y].TileType = 0;

                        return currentCellIndex - 1;
                    case Directions.East:
                        _tiles[cell.X + 1, cell.Y].TileType = 0;

                        return currentCellIndex + 1;
                    case Directions.South:
                        _tiles[cell.X, cell.Y - 1].TileType = 0;

                        return currentCellIndex - XSize;
                }
            }

            return -1;
        }

        private int PickRandomIntialCell()
        {
            return Random.Range(0, _cells.Length);
        }
    }
}