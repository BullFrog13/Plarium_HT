using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerManager : MonoBehaviour
    {
        public float Speed;

        private bool _facingRight;
        private float _speedX;
        private float _speedY;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _facingRight = true;
        }

        private void Update()
        {
            MovePlayer(_speedX, _speedY);
            FlipPlayer();

            if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _speedY = Speed;
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                _speedY = 0;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _speedX = -Speed;
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                _speedX = 0;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                _speedX = Speed;
            }
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                _speedX = 0;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                _speedY = -Speed;
            }
            if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                _speedY = 0;
            }
        }

        private void FlipPlayer()
        {
            if (_speedX > 0 && !_facingRight || _speedX < 0 && _facingRight)
            {
                _facingRight = !_facingRight;
                Vector2 temp = transform.localScale;
                temp.x *= -1;
                transform.localScale = temp;
            }
        }

        private void MovePlayer(float horizontalSpeed, float verticalSpeed)
        {
            _rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag.Equals("coin"))
            {
                MazeData.CurrentCointCount--;
                MazeData.CollectedCoins++;
                Destroy(other.gameObject);
            }
        }
    }
}