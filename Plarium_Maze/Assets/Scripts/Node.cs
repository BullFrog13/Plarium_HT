using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Node
    {
        public List<Node> Neighbours;
        public int X;
        public int Y;
        public bool Walkable;

        public Node()
        {
            Neighbours = new List<Node>();
        }

        public float DistanceTo(Node otherNode)
        {
            return Vector2.Distance(new Vector2(X, Y), new Vector2(otherNode.X, otherNode.Y));
        }
    }
}