public class Tile
{
    public int X;
    public int Y;
    public bool Walkable;
    public bool IsBreakableWall;

    public Tile(int x, int y, bool walkable = true, bool isBreakableWall = false)
    {
        X = x;
        Y = y;
        Walkable = walkable;
        IsBreakableWall = isBreakableWall;
    }
}