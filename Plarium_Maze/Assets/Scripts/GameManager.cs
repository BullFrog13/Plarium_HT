using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Coin;
        public GameObject Player;
        public GameObject Zombie;
        public GameObject Mummy;
        public int CurrentCointCount;
        public int CollectedCoins;

        private const int MaxCoinCount = 10;
        private const float CoinAddingRangeTime = 5f;
        private const int CoinsNeededForSecondZombieSpawn = 5;
        private const int CoinsNeededForMummySpawn = 10;
        private bool _mummyIsEnabled;
        private bool _secondZombieIsEnabled;
        private float _coinTimer;
        private MazeGenerator.Maze _maze;

        private void Start()
        {
            _maze = GameObject.Find("MazeGenerator").GetComponent<MazeGenerator>().maze;
            CreatePlayer();
            CreateZombie();
            _secondZombieIsEnabled = false;
            _mummyIsEnabled = false;
            _coinTimer = CoinAddingRangeTime;
            CollectedCoins = 0;
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

            if (CollectedCoins == CoinsNeededForSecondZombieSpawn && !_secondZombieIsEnabled)
            {
                CreateZombie();
                _secondZombieIsEnabled = true;
            }

            if (CollectedCoins == CoinsNeededForMummySpawn && !_mummyIsEnabled)
            {
                CreateMummy();
                _mummyIsEnabled = true;
            }
        }

        private void AddCoin()
        {
            var randomCoinPos = new Vector3(_maze.InitialPos.x + _maze.MazeWallLength/2 + Random.Range(0, _maze.XSize), 0,
                _maze.InitialPos.y + _maze.MazeWallLength/2 + Random.Range(0, _maze.YSize));
            Instantiate(Coin, randomCoinPos, Quaternion.AngleAxis(90, Vector3.right));
        }

        private void CreatePlayer()
        {
            var randomPlayerPos = new Vector3(
                _maze.InitialPos.x + _maze.MazeWallLength/2 + Random.Range(0, _maze.XSize),
                0,
                _maze.InitialPos.y + _maze.MazeWallLength/2 + Random.Range(0, _maze.YSize));
            Instantiate(Player, randomPlayerPos, Quaternion.AngleAxis(90, Vector3.right));
        }

        private void CreateZombie()
        {
            var randomZombiePos = new Vector3(
                _maze.InitialPos.x + _maze.MazeWallLength / 2 + Random.Range(0, _maze.XSize),
                0,
                _maze.InitialPos.y + _maze.MazeWallLength / 2 + Random.Range(0, _maze.YSize));
            Instantiate(Zombie, randomZombiePos, Quaternion.AngleAxis(90, Vector3.right));
        }

        private void CreateMummy()
        {
            var randomMummyPos = new Vector2(
                _maze.InitialPos.x + _maze.MazeWallLength / 2 + Random.Range(0, _maze.XSize),
                _maze.InitialPos.y + _maze.MazeWallLength / 2 + Random.Range(0, _maze.YSize));
            Instantiate(Mummy, randomMummyPos, Quaternion.identity);
        }
    }
}