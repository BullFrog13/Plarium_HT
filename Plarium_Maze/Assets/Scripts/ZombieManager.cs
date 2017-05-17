using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class ZombieManager : MonoBehaviour
    {
        public float Speed;

        private bool _facingRight;
        private bool _followPlayer;
        private float _speedX;
        private float _speedY;
        private Vector3 _cachedPosition;
        private Transform _updatedPosition;
        private Dictionary<int, bool> _possiblePathes;
        private Rigidbody2D _rb;

        private void Start()
        {
            _cachedPosition = transform.position;
            _possiblePathes = new Dictionary<int, bool>
            {
                { 1, true },
                { 2, true },
                { 3, true },
                { 4, true }
            };
            _rb = GetComponent<Rigidbody2D>();
            _facingRight = true;
        }

        private void Update()
        {
            if (CheckIfZombieIsInTheCellCenter())
            {

            }
            FlipZombie();
            MoveZombie(_speedX, _speedY);
        }

        private void ZombieFollowPlayerMove()
        {
            
        }

        private void FlipZombie()
        {
            if (_speedX > 0 && !_facingRight || _speedX < 0 && _facingRight)
            {
                _facingRight = !_facingRight;
                Vector2 temp = transform.localScale;
                temp.x *= -1;
                transform.localScale = temp;
            }
        }

        private void MoveZombie(float horizontalSpeed, float verticalSpeed)
        {
            _rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);
        }

        private bool CheckIfZombieIsInTheCellCenter()
        {
            return transform.position.x * 2 % 1 == 0 && transform.position.y * 2 % 1 == 0;
        }
    }
}