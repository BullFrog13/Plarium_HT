using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public int XSize;
        public int YSize;
        public GameObject Coin;
        public GameObject Player;
        public GameObject Zombie;
        public GameObject Mummy;
        public GameObject GroundTile;
        public GameObject WallTile;

        private MazeGenerator _mazeGenerator;
        private GameObject _mazeHolder;
        private const int MaxCoinCount = 10;
        private const float CoinAddingRangeTime = 5f;
        private const int CoinsNeededForSecondZombieSpawn = 5;
        private const int CoinsNeededForMummySpawn = 10;
        private bool _mummyIsEnabled;
        private bool _secondZombieIsEnabled;
        private float _coinTimer;

        private void Awake()
        {
            _mazeHolder = new GameObject { name = "Maze holder" };
           _mazeGenerator = new MazeGenerator();
            _mazeGenerator.GenerateMaze(XSize, YSize);
            VisualizeLabyrinth();

            _secondZombieIsEnabled = false;
            _mummyIsEnabled = false;
            _coinTimer = CoinAddingRangeTime;

            AddItemIntoMaze(Player);
            AddItemIntoMaze(Zombie);
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
                AddItemIntoMaze(Zombie);
                _secondZombieIsEnabled = true;
            }

            if (MazeData.CollectedCoins == CoinsNeededForMummySpawn && !_mummyIsEnabled)
            {
                AddItemIntoMaze(Mummy);
                _mummyIsEnabled = true;
            }
        }

        private void VisualizeLabyrinth()
        {
            for (var i = 0; i <= YSize * 2; i++)
            {
                for (var j = 0; j <= XSize * 2; j++)
                {
                    bool isWalkable = _mazeGenerator.Tiles[i, j].Walkable;
                    var tempTile = Instantiate(isWalkable ? GroundTile : WallTile, new Vector3(i, j, 0), Quaternion.identity);
                    tempTile.transform.parent = _mazeHolder.transform;
                }
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