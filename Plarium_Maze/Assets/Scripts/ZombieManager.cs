using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class ZombieManager : MonoBehaviour
    {
        public float Speed;

        private bool _facingRight;
        private bool _followPlayer;
        private float _speedX;
        private float _speedY;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _facingRight = true;
            _followPlayer = true;
            _speedX = 1;
        }

        private void FixedUpdate()
        {
            /*if (_followPlayer)
            {

            }
            else
            {*/
            RaycastHit hiy;
            if (_speedX == 1)
                {
                    
                    var hit = Physics.Raycast(transform.position, Vector3.right, out hiy, 0.5f);
                    if (hiy.collider != null && hiy.transform.tag == "wall")
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
                var hit = Physics.Raycast(transform.position, Vector3.left, out hiy, 0.5f);
                if (hiy.collider != null && hiy.transform.tag == "wall")
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
                var hit = Physics.Raycast(transform.position, Vector3.forward, out hiy, 0.5f);
                if (hiy.collider != null && hiy.transform.tag == "wall")
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
                var hit = Physics.Raycast(transform.position, Vector3.back, out hiy, 0.5f);
                if (hiy.collider != null && hiy.transform.tag == "wall")
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
                //}
            }
            FlipZombie();
            MoveZombie(_speedX, _speedY);
        }

        private void FlipZombie()
        {
            if (_speedX < 0 && !_facingRight || _speedX > 0 && _facingRight)
            {
                _facingRight = !_facingRight;
                Vector3 temp = transform.localScale;
                temp.x *= -1;
                transform.localScale = temp;
            }
        }

        private void MoveZombie(float horizontalSpeed, float verticalSpeed)
        {
            _rb.velocity = new Vector3(horizontalSpeed, 0, verticalSpeed);
        }

        private List<int> ChooseAvailableDirections()
        {
            RaycastHit hiy;
            var result = new List<int>();
            var hit = Physics.Raycast(transform.position, Vector3.forward, out hiy, 0.5f);
            if (hiy.collider == null)
            {
                result.Add(0);
            }
            var hit1 = Physics.Raycast(transform.position, Vector3.right, out hiy, 0.5f);
            if (hiy.collider == null)
            {
                result.Add(2);
            }
            var hit2 = Physics.Raycast(transform.position, Vector3.left, out hiy, 0.5f);
            if (hiy.collider == null)
            {
                result.Add(1);
            }
            var hi5t = Physics.Raycast(transform.position, Vector3.down, out hiy, 0.5f);
            if (hiy.collider == null)
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