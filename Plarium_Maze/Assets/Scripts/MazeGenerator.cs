using System;
using System.Collections.Generic;
using System.Linq;
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
            public Cell[] Cells;

            public Maze(int xsize, int ysize)
            {
                XSize = xsize;
                YSize = ysize;
                CellsCount = XSize * YSize;
                Cells = new Cell[CellsCount];
            }
        }

        [Serializable]
        public class Cell
        {
            public bool Visited;

            public GameObject North;
            public int NorthWallIndex;

            public GameObject East;
            public int EastWallIndex;

            public GameObject West;
            public int WestWallIndex;

            public GameObject South;
            public int SouthWallIndex;
        }

        public Maze maze;
        public GameObject Wall;
        public GameObject Floor;
        public int xSize;
        public int ySize;

        private GameObject _staticWallHolder;
        private GameObject _cellsHoder;
        private GameObject _breakableWallsHolder;
        private Cell[] _cells;
        private GameObject[] _setOfBreakableWalls;

        private enum Directions
        {
            North = 1,
            West = 2,
            East = 3,
            South = 4
        }

        private void Start()
        {
            _staticWallHolder = new GameObject { name = "Static wall holder" };
            _cellsHoder = new GameObject { name = "Cells holder" };
            _breakableWallsHolder =  new GameObject { name = "Breakable walls holder" };
            maze = new Maze(xSize, ySize);

            CreateNotBreakableWalls();
            CreateStaticFloor();
            CreateBreakableWalls();
            CreateCells();
            CreateLabyrinth();
        }

        private void CreateNotBreakableWalls()
        {
            // iteration for Y
            for (var i = 0; i <= maze.YSize * 2; i++)
            {
                // iteration for X.
                for (var j = 0; j <= maze.XSize * 2; j++)
                {
                    // check for outside walls
                    if (j == 0 || i == 0 || j == maze.XSize * 2 || i == maze.YSize * 2)
                    {
                        var outsideWallCreatePos = new Vector2(j, i);
                        var tempOutsideWall = Instantiate(Wall, outsideWallCreatePos, Quaternion.identity);
                        tempOutsideWall.transform.parent = _staticWallHolder.transform;
                    }
                    // concerning that walls are squares there are some walls inside that cannot be broken
                    if (j != 0 && i != 0 && j % 2 == 0 && i % 2 == 0)
                    {
                        var outsideWallCreatePos = new Vector2(j, i);
                        var tempOutsideWall = Instantiate(Wall, outsideWallCreatePos, Quaternion.identity);
                        tempOutsideWall.transform.parent = _staticWallHolder.transform;
                    }
                }
            }
        }

        private void CreateStaticFloor()
        {
            // instantiating cells
            _cells = new Cell[maze.CellsCount];
            var cellsCounter = 0;
            for (var i = 0; i < maze.YSize * 2; i++)
            {
                for (var j = 0; j < maze.XSize * 2; j++)
                {
                    if (i != 0 && j != 0 && i % 2 != 0 && j % 2 != 0)
                    {
                        _cells[cellsCounter] = new Cell();
                        var cellCreatePos = new Vector2(j, i);
                        var tempOutsideWall = Instantiate(Floor, cellCreatePos, Quaternion.identity);
                        tempOutsideWall.transform.parent = _cellsHoder.transform;
                        cellsCounter++;
                    }
                }
            }
        }

        private void CreateBreakableWalls()
        {
            // basically creating breakable walls inside of our maze
            for (var i = 0; i < maze.YSize * 2; i++)
            {
                for (var j = 0; j < maze.XSize * 2; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        if (i % 2 != 0 && j % 2 == 0 || i % 2 == 0 && j % 2 != 0)
                        {
                            var cellCreatePos = new Vector2(j, i);
                            var tempOutsideWall = Instantiate(Wall, cellCreatePos, Quaternion.identity);
                            tempOutsideWall.transform.parent = _breakableWallsHolder.transform;
                        }
                    }
                }
            }
        }

        private void CreateCells()
        {
            // this method is just for assigning right walls for right cells for A* Depth algorithm needs

            var processingRow = 0;
            var rowCounterCheck = 1;

            _setOfBreakableWalls = new GameObject[_breakableWallsHolder.transform.childCount];

            for (var i = 0; i < _setOfBreakableWalls.Length; i++)
            {
                _setOfBreakableWalls[i] = _breakableWallsHolder.transform.GetChild(i).gameObject;
            }

            for (var i = 0; i < maze.CellsCount; i++)
            {
                // for east wall
                if (rowCounterCheck != maze.XSize)
                {
                    _cells[i].East = _setOfBreakableWalls[i + processingRow * (maze.XSize - 1)];

                    // need all those indexes later to understand where to brake wall and build floor
                    _cells[i].EastWallIndex = i + processingRow * (maze.XSize - 1);
                }

                // for north wall
                if (processingRow != maze.YSize - 1)
                {
                    _cells[i].North = _setOfBreakableWalls[i + (maze.XSize - 1) * (processingRow + 1)];
                    _cells[i].NorthWallIndex = i + (maze.XSize - 1)*(processingRow + 1);
                }

                // for west wall
                if (rowCounterCheck != 1)
                {
                    _cells[i].West = _setOfBreakableWalls[i + processingRow * (maze.XSize - 1) - 1];
                    _cells[i].WestWallIndex = i + processingRow*(maze.XSize - 1) - 1;
                }

                // for south wall
                if (processingRow != 0)
                {
                    _cells[i].South = _setOfBreakableWalls[i + (maze.XSize - 1) * (processingRow - 1) - 1];
                    _cells[i].SouthWallIndex = i + (maze.XSize - 1)*(processingRow - 1) - 1;
                }

                // increase row counter
                if (rowCounterCheck == maze.XSize)
                {
                    processingRow++;
                    rowCounterCheck = 0;
                }

                rowCounterCheck++;
            }
        }

        private void CreateLabyrinth()
        {
            // implementation of an A* Depth algorithm is beneath

            var currentCell = PickRandomIntialCell();
            var stack = new Stack<int>();
            stack.Push(currentCell);
            var countOfVisitedCells = 1;

            while (countOfVisitedCells <= maze.CellsCount)
            {
                var randomChosenNeighbour = PickRandomNotVisitedCell(stack.Peek());
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

        private int PickRandomNotVisitedCell(int currentCellIndex)
        {
            var cell = _cells[currentCellIndex];
            var possibleDirections = new List<Directions>();

            if (cell.North != null && !_cells[currentCellIndex + maze.XSize].Visited)
            {
                possibleDirections.Add(Directions.North);
            }

            if (cell.West != null && !_cells[currentCellIndex - 1].Visited)
            {
                possibleDirections.Add(Directions.West);
            }

            if (cell.East != null && !_cells[currentCellIndex + 1].Visited)
            {
                possibleDirections.Add(Directions.East);
            }

            if (cell.South != null && !_cells[currentCellIndex - maze.XSize].Visited)
            {
                possibleDirections.Add(Directions.South);
            }

            if (possibleDirections.Any())
            {
                var randomDirectionIndex = Random.Range(0, possibleDirections.Count);
                switch (possibleDirections[randomDirectionIndex])
                {
                    case Directions.North:
                        cell.North = null;
                        Destroy(_setOfBreakableWalls[cell.NorthWallIndex]);
                        Instantiate(Floor, _setOfBreakableWalls[cell.NorthWallIndex].transform.position,
                            Quaternion.identity);
                        return currentCellIndex + maze.XSize;
                    case Directions.West:
                        cell.West = null;
                        Destroy(_setOfBreakableWalls[cell.WestWallIndex]);
                        Instantiate(Floor, _setOfBreakableWalls[cell.WestWallIndex].transform.position,
                            Quaternion.identity);
                        return currentCellIndex - 1;
                    case Directions.East:
                        cell.East = null;
                        Destroy(_setOfBreakableWalls[cell.EastWallIndex]);
                        Instantiate(Floor, _setOfBreakableWalls[cell.EastWallIndex].transform.position,
                            Quaternion.identity);
                        return currentCellIndex + 1;
                    case Directions.South:
                        cell.South = null;
                        Destroy(_setOfBreakableWalls[cell.SouthWallIndex]);
                        Instantiate(Floor, _setOfBreakableWalls[cell.SouthWallIndex].transform.position,
                            Quaternion.identity);
                        return currentCellIndex - maze.XSize;
                }
            }

            return -1;
        } 

        private int PickRandomIntialCell()
        {
            return Random.Range(0, maze.CellsCount);
        }
    }
}