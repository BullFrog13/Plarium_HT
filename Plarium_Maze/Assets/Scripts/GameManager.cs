using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Coin;
        public GameObject Player;
        public GameObject Zombie;
        public GameObject Mummy;

        private const int MaxCoinCount = 10;
        private const float CoinAddingRangeTime = 5f;
        private const int CoinsNeededForSecondZombieSpawn = 5;
        private const int CoinsNeededForMummySpawn = 10;
        private bool _mummyIsEnabled;
        private bool _secondZombieIsEnabled;
        private float _coinTimer;

        private void Start()
        {
            AddItemIntoMaze(Player);
            AddItemIntoMaze(Zombie);
            _secondZombieIsEnabled = false;
            _mummyIsEnabled = false;
            _coinTimer = CoinAddingRangeTime;
        }

        private void Update()
        {
            if (MazeData.CurrentCointCount < MaxCoinCount)
            {
                _coinTimer -= Time.deltaTime;

                if (_coinTimer <= 0)
                {
                    AddItemIntoMaze(Coin);
                    _coinTimer = CoinAddingRangeTime;
                    MazeData.CurrentCointCount++;
                }
            }

            if (MazeData.CollectedCoins == CoinsNeededForSecondZombieSpawn && !_secondZombieIsEnabled)
            {
                AddItemIntoMaze(Mummy);
                _secondZombieIsEnabled = true;
            }

            if (MazeData.CollectedCoins == CoinsNeededForMummySpawn && !_mummyIsEnabled)
            {
                AddItemIntoMaze(Mummy);
                _mummyIsEnabled = true;
            }
        }

        private void AddItemIntoMaze(GameObject objectToAdd)
        {
            var chosenTile = GetRandomTileForMazeItems();
            Instantiate(objectToAdd, chosenTile.transform.position, Quaternion.identity);
        }

        private GameObject GetRandomTileForMazeItems()
        {
            var availableTiles = GameObject.FindGameObjectsWithTag("ground");
            var randomTileIndex = Random.Range(0, availableTiles.Length);
            return availableTiles[randomTileIndex];
        }
    }
}