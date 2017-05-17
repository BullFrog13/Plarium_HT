using UnityEngine;

namespace Assets.Scripts
{
    public class Test : MonoBehaviour
    {
        public GameObject Follow;

        private void LateUpdate()
        {
            transform.position = Follow.transform.position;
        }
    }
}