using UnityEngine;

namespace Assets.Scripts
{
    public class ClickableTile : MonoBehaviour
    {
        public int X;
        public int Y;
        public GameManager Man;

        void OnMouseUp()
        {
            Debug.Log("Clicked" + X + "" + Y);
            Man.GenerateTheShortestPath(X, Y);
        }
    }
}