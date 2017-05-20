using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {
        // this a test class that emulates objects that will findp player
        public int tileX;
        public int tileY;

        public List<GameManager.Node> CurrentPath = null;

        private void Update()
        {
            if (CurrentPath != null)
            {
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
    }
}