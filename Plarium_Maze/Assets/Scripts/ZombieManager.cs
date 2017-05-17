using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ZombieManager : MonoBehaviour
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
            _speedX = 1;
        }

        private void FixedUpdate()
        {
            if (_speedX == 1)
            {
                var hit = Physics2D.Raycast(transform.position, Vector2.right, 0.5f);
                if (hit.collider != null && hit.transform.tag == "wall")
                {
                    var choosenDirection = ChooseRandomDirection();
                    switch (choosenDirection)
                    {
                        case 0:
                            _speedX = 0;
                            _speedY = 1;
                            break;
                        case 1:
                            _speedX = -1;
                            _speedY = 0;
                            break;
                        case 2:
                            _speedX = 1;
                            _speedY = 0;
                            break;
                        case 3:
                            _speedX = 0;
                            _speedY = -1;
                            break;
                    }
                }
            }
            else if (_speedX == -1)
            {
                var hit = Physics2D.Raycast(transform.position, Vector2.left, 0.5f);
                if (hit.collider != null && hit.transform.tag == "wall")
                {
                    var choosenDirection = ChooseRandomDirection();
                    switch (choosenDirection)
                    {
                        case 0:
                            _speedX = 0;
                            _speedY = 1;
                            break;
                        case 1:
                            _speedX = -1;
                            _speedY = 0;
                            break;
                        case 2:
                            _speedX = 1;
                            _speedY = 0;
                            break;
                        case 3:
                            _speedX = 0;
                            _speedY = -1;
                            break;
                    }
                }
            }
            else if (_speedY == 1)
            {
                var hit = Physics2D.Raycast(transform.position, Vector2.up, 0.5f);
                if (hit.collider != null && hit.transform.tag == "wall")
                {
                    var choosenDirection = ChooseRandomDirection();
                    switch (choosenDirection)
                    {
                        case 0:
                            _speedX = 0;
                            _speedY = 1;
                            break;
                        case 1:
                            _speedX = -1;
                            _speedY = 0;
                            break;
                        case 2:
                            _speedX = 1;
                            _speedY = 0;
                            break;
                        case 3:
                            _speedX = 0;
                            _speedY = -1;
                            break;
                    }
                }
            }
            else if (_speedY == -1)
            {
                var hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
                if (hit.collider != null && hit.transform.tag == "wall")
                {
                    var choosenDirection = ChooseRandomDirection();
                    switch (choosenDirection)
                    {
                        case 0:
                            _speedX = 0;
                            _speedY = 1;
                            break;
                        case 1:
                            _speedX = -1;
                            _speedY = 0;
                            break;
                        case 2:
                            _speedX = 1;
                            _speedY = 0;
                            break;
                        case 3:
                            _speedX = 0;
                            _speedY = -1;
                            break;
                    }
                }
            }
            FlipZombie();
            MoveZombie(_speedX, _speedY);
        }

        private void FlipZombie()
        {
            if (_speedX < 0 && !_facingRight || _speedX > 0 && _facingRight)
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

        private List<int> ChooseAvailableDirections()
        {
            var result = new List<int>();
            var hit2 = Physics2D.Raycast(transform.position, Vector2.up, 0.5f);
            if (hit2.collider == null)
            {
                result.Add(0);
            }
            var hit1 = Physics2D.Raycast(transform.position, Vector2.right, 0.5f);
            if (hit1.collider == null)
            {
                result.Add(2);
            }
            var hit3 = Physics2D.Raycast(transform.position, Vector2.left, 0.5f);
            if (hit3.collider == null)
            {
                result.Add(1);
            }
            var hit4 = Physics2D.Raycast(transform.position, Vector2.down, 0.5f);
            if (hit4.collider == null)
            {
                result.Add(3);
            }

            return result;
        }

        private int ChooseRandomDirection()
        {
            var ava = ChooseAvailableDirections();
            var randomIndex = Random.Range(0, ava.Count);
            return ava[randomIndex];
        }
    }
}