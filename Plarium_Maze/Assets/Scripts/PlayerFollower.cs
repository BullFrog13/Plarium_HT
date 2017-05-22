using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerFollower : MonoBehaviour
    {
        public GameObject Player;

        private void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            gameObject.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -20);
        }

        private void Update()
        {
            gameObject.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, -20);
        }
    }
}