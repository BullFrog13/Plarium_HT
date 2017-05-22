using System;

namespace Assets.Scripts
{
    public class MazeData
    {
        public const int WallLength = 1;

        public static int XSize;

        public static int YSize;

        public static int CurrentCointCount = 0;

        public static string Name;

        public static int Score;

        public static float SecondsSpent;

        public static DateTime GameStarted;

        public static string FinishReason;

        public static Node[,] Graph;
    }
}