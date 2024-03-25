using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Player.Controls
{
    public class PlayerJump : MonoBehaviour
    {
        [NonSerialized] private PlayerCrouch _playerCrouch;
        [NonSerialized] private PlayerMovements _playerMovements;
        [NonSerialized] private PlayerKnockback _playerKnockback;
        [NonSerialized] private Player _player;

        [SerializeField] public LayerMask whatIsGround;
        [NonSerialized] private Transform _groundCheck;
        [NonSerialized] public bool Grounded;
        [NonSerialized] private Vector2 _groundCheckSize;
        [SerializeField] private float jumpForce;
        [NonSerialized] public bool RecentlyJumped = false;
        [NonSerialized] public bool CanJumpBoost = false;
        [NonSerialized] private float _previousAngle;
        [Range(0, .1f)] [SerializeField] private float coyoteTimeDuration;
        [NonSerialized] private float _coyoteTime;
        [Range(0, .3f)] [SerializeField] private float jumpBufferDuration;
        [NonSerialized] private float _jumpBuffer;
        [NonSerialized] private bool _jumpInput;
        [NonSerialized] private Rigidbody2D _rigidbody2D;

        [Range(0, 1f)] [SerializeField] private float jumpHoldTime = 0.2f;
        [SerializeField] private float jumpHoldForce = 2f;

        private float _jumpHold;
        private bool _holdingJump;

        private void Awake()
        {
            _playerCrouch = GetComponent<PlayerCrouch>();
            _playerMovements = GetComponent<PlayerMovements>();
            _playerKnockback = GetComponent<PlayerKnockback>();
            _player = GetComponent<Player>();

            _groundCheck = ObjectSearch.FindChild(transform, "GroundCheck");
            _groundCheckSize = new Vector2(GetComponent<BoxCollider2D>().size.x - 0.05f, 0.1f);
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!_player.Dead)
            {
                _holdingJump = Input.GetButton("Jump");
                if (Input.GetButtonDown("Jump"))
                {
                    _jumpInput = true;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    _jumpHold = 0;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!_player.Dead)
            {
                JumpControl(_jumpInput);
                _jumpInput = false;
                Grounded = false;
                if (!RecentlyJumped)
                {
                    Collider2D[] colliders =
                        Physics2D.OverlapBoxAll(_groundCheck.position, _groundCheckSize, 0, whatIsGround);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].gameObject != gameObject)
                        {
                            Grounded = true;
                            _playerKnockback.ShotDirection = "none";
                            _playerMovements.PreviousHorizontalInput = 0;
                        }
                    }
                }
            }
        }

        public void JumpControl(bool jumpInput)
        {
            if (jumpInput)
            {
                if (Grounded || _coyoteTime > 0) //check for ground or remaining coyote time
                {
                    _jumpHold = jumpHoldTime;
                    Jump();
                }
                else
                {
                    _jumpBuffer = jumpBufferDuration;
                }
            }
            else if (Grounded)
            {
                _coyoteTime = -1f;
                if (_jumpBuffer > 0) //check for remaining jump buffer
                {
                    if (_holdingJump)
                    {
                        _jumpHold = jumpHoldTime;
                    }

                    Jump();
                }
            }
            else
            {
                if (_coyoteTime > 0)
                {
                    _coyoteTime = Mathf.Max(_coyoteTime - Time.fixedDeltaTime, 0);
                }
                else if (_coyoteTime < 0) //leaves ground
                {
                    _coyoteTime = coyoteTimeDuration;
                }

                if (_jumpBuffer > 0)
                {
                    _jumpBuffer = Mathf.Max(_jumpBuffer - Time.fixedDeltaTime, 0);
                }
            }

            if (_jumpHold > 0)
            {
                // Adjust the jumpHold value to avoid negative or undefined logarithm values
                float adjustedJumpHold = _jumpHold + 1;

                // Apply the logarithmic jump force
                float appliedJumpForce = jumpHoldForce * Mathf.Log(adjustedJumpHold) / jumpHoldTime;
                _rigidbody2D.AddForce(new Vector2(0f, appliedJumpForce));

                // Decrement the jumpHold value
                _jumpHold = Mathf.Max(_jumpHold - Time.fixedDeltaTime, 0);
            }
        }

        private void Jump()
        {
            Grounded = false;
            RecentlyJumped = true;
            CanJumpBoost = true;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0f);
            _rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            _coyoteTime = 0;
            _jumpBuffer = 0;
            StartCoroutine(JumpCoroutine());
        }

        private IEnumerator JumpCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            RecentlyJumped = false;
            CanJumpBoost = false;
        }
    }
}