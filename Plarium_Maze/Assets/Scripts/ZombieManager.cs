using UnityEngine;

namespace Assets.Scripts
{
    public class ZombieManager : MonoBehaviour
    {
        public float Speed;

        private bool _facingRight;
        private bool _followPlayer;
        private float _speedX;
        private float _sppedY;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _facingRight = true;
        }

        private void Update()
        {
            FlipZombie();
            MoveZombie(_speedX, _sppedY);
        }

        private void ZombieRandomMove()
        {
            
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
    }
}