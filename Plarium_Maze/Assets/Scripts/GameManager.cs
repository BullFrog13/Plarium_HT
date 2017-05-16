using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Coin;
        private float CoinAddingRangeTime = 1;
        public int CurrentCointCount;

        private const int MaxCoinCount = 10;
        private float _coinTimer;

        private MazeGenerator.Maze _maze;


        private void Start()
        {
            _maze = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>().maze;
            _coinTimer = CoinAddingRangeTime;
        }

        private void Update()
        {
            if (CurrentCointCount < MaxCoinCount)
            {
                _coinTimer -= Time.deltaTime;

                if (_coinTimer <= 0)
                {
                    AddCoin();
                    _coinTimer = CoinAddingRangeTime;
                    CurrentCointCount++;
                }
            }
        }

        private void AddCoin()
        {
            var randomCoinPos = new Vector2(_maze.InitialPos.x + _maze.MazeWallLength / 2 + Random.Range(0, _maze.XSize),
               _maze.InitialPos.y + _maze.MazeWallLength / 2 + Random.Range(0, _maze.YSize));
            Instantiate(Coin, randomCoinPos, Quaternion.identity);
        }
    }
}