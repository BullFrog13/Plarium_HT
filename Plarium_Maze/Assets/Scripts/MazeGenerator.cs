using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MazeGenerator
    {
        public Tile[,] Tiles;

        private class Cell
        {
            public bool Visited;

            public int X;
            public int Y;
        }

        private Cell[] _cells;
        private int _cellsCount;

        private enum Directions
        {
            North = 1,
            West = 2,
            East = 3,
            South = 4
        }

        public void GenerateMaze(int xSize, int ySize)
        {
            MazeData.XSize = xSize;
            MazeData.YSize = ySize;
            _cellsCount = xSize * ySize;

            Tiles = new Tile[xSize * 2 + 1, ySize * 2 + 1];

            for (var i = 0; i <= xSize * 2; i++)
            {
                for (var j = 0; j <= ySize * 2; j++)
                {
                    Tiles[j, i] = new Tile(j, i);
                }
            }

            CreateNotBreakableWalls();
            CreateBreakableWalls();
            CreateCells();
            CreateLabyrinth();
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
                        Tiles[j, i] = new Tile(j, i, false);
                    }
                    // concerning that walls are squares there are some walls inside that cannot be broken
                    if (j != 0 && i != 0 && j % 2 == 0 && i % 2 == 0)
                    {
                        Tiles[j, i] = new Tile(j, i, false);
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
                    // avoid outer walls
                    if (i != 0 && j != 0 && i != MazeData.XSize * 2 && j != MazeData.YSize * 2)
                    {
                        if (i % 2 != 0 && j % 2 == 0 || i % 2 == 0 && j % 2 != 0)
                        {
                            Tiles[j, i] = new Tile(j, i, false, true);
                        }
                    }
                }
            }
        }

        private void CreateCells()
        {
            // this method is just for assigning right walls for right cells for A* Depth algorithm needs
            _cells = new Cell[MazeData.XSize * MazeData.YSize];
            var cellsCounter = 0;
            for (var i = 1; i < MazeData.YSize * 2; i++)
            {
                for (var j = 1; j < MazeData.XSize * 2; j++)
                {
                    if (Tiles[j, i].Walkable)
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

            if (currentCellIndex + MazeData.XSize < _cellsCount &&
                !_cells[currentCellIndex + MazeData.XSize].Visited && Tiles[cell.X, cell.Y + 1].IsBreakableWall)
            {
                possibleDirections.Add(Directions.North);
            }

            if (currentCellIndex - 1 >=  0 && !_cells[currentCellIndex - 1].Visited && Tiles[cell.X - 1, cell.Y].IsBreakableWall)
            {
                possibleDirections.Add(Directions.West);
            }

            if (currentCellIndex + 1 < _cellsCount && !_cells[currentCellIndex + 1].Visited && Tiles[cell.X + 1, cell.Y].IsBreakableWall)
            {
                possibleDirections.Add(Directions.East);
            }

            if (currentCellIndex - MazeData.XSize >= 0 &&
                !_cells[currentCellIndex - MazeData.XSize].Visited && Tiles[cell.X, cell.Y - 1].IsBreakableWall)
            {
                possibleDirections.Add(Directions.South);
            }

            if (possibleDirections.Any())
            {
                var randomDirectionIndex = Random.Range(0, possibleDirections.Count);
                switch (possibleDirections[randomDirectionIndex])
                {
                    case Directions.North:
                        Tiles[cell.X, cell.Y + 1].Walkable = true;
                        Tiles[cell.X, cell.Y + 1].IsBreakableWall = false;
                        return currentCellIndex + MazeData.XSize;
                    case Directions.West:
                        Tiles[cell.X - 1, cell.Y].Walkable = true;
                        Tiles[cell.X - 1, cell.Y].IsBreakableWall = false;
                        return currentCellIndex - 1;
                    case Directions.East:
                        Tiles[cell.X + 1, cell.Y].Walkable = true;
                        Tiles[cell.X + 1, cell.Y].IsBreakableWall = false;
                        return currentCellIndex + 1;
                    case Directions.South:
                        Tiles[cell.X, cell.Y - 1].Walkable = true;
                        Tiles[cell.X, cell.Y - 1].IsBreakableWall = false;
                        return currentCellIndex - MazeData.XSize;
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