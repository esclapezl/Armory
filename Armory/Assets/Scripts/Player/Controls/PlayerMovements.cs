using System;
using Effects;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Weapons;

namespace Player.Controls
{
    public class PlayerMovements : MonoBehaviour
    {
        [NonSerialized] private PlayerCrouch _playerCrouch;
        [NonSerialized] private PlayerJump _playerJump;
        [NonSerialized] private PlayerKnockback _playerKnockback;
        [NonSerialized] private Player _player;
        [NonSerialized] private WeaponMovements _weaponMovements;

        [NonSerialized] private Transform _playerDirection;
        [NonSerialized] public bool FacingRight = true;
        [NonSerialized] public bool Armed = true;
        [SerializeField] private int runSpeed;
        [Range(0, .3f)] [SerializeField] private float enterMovementSmoothing;
        [Range(0, .3f)] [SerializeField] private float exitMovementSmoothing;
        [NonSerialized] public int HorizontalInput;
        [NonSerialized] public int HorizontalMove;
        [NonSerialized] public int PreviousHorizontalInput = 0;
        [SerializeField] public bool canAirControl;
        [SerializeField] public float airControl;
        [NonSerialized] private Rigidbody2D _rigidbody2D;
        [NonSerialized] private Vector3 _velocity;
        
        [NonSerialized] private Squishable _squishable;
        [SerializeField] private float runSquishSpeed;
        [NonSerialized] private float _runSquishSpeedTimer;

        private void Awake()
        {
            _playerCrouch = GetComponent<PlayerCrouch>();
            _playerJump = GetComponent<PlayerJump>();
            _playerKnockback = GetComponent<PlayerKnockback>();
            _player = GetComponent<Player>();
            _weaponMovements = GetComponent<WeaponMovements>();

            _playerDirection = ObjectSearch.FindChild(transform, "PlayerDirection");
            _rigidbody2D = GetComponent<Rigidbody2D>();
            
            _squishable = GetComponent<Squishable>();
        }

        private void Update()
        {
            if (!_player.Dead)
            {
                HorizontalInput = (int)Input.GetAxisRaw("Horizontal");
                HorizontalMove = (HorizontalInput * runSpeed);
                if (HorizontalInput != 0 && HorizontalInput != PreviousHorizontalInput)
                {
                    _playerKnockback.ShotDirection = "none";
                }

                PreviousHorizontalInput = HorizontalInput != 0 ? HorizontalInput : PreviousHorizontalInput;
            }
        }

        private void FixedUpdate()
        {
            if (!_player.Dead)
            {
                MoveControl(HorizontalMove * Time.fixedDeltaTime, Armed);
            }
        }

        public void MoveControl(float moveInput, bool armed)
        {
            _runSquishSpeedTimer -= Time.deltaTime;
            if (_playerJump.Grounded || canAirControl)
            {
                if (_playerCrouch.Crouching)
                {
                    moveInput *= _playerCrouch.crouchSpeed;
                }

                if ((_playerJump.Grounded && _playerKnockback.ShotDirection != "down")
                    ||
                    _playerKnockback.ShotDirection == "none"
                    ||
                    (!(_playerKnockback.ShotDirection == "left" && PreviousHorizontalInput == 1)
                     && !(_playerKnockback.ShotDirection == "right" && PreviousHorizontalInput == -1)
                     && HorizontalInput != 0)
                   ) //au sol ou pas dans la direction du knockback
                {
                    float smoothingModifier = _playerJump.Grounded ? 1 : airControl; // CONTROL IN AIR
                    Vector3 targetVelocity = new Vector2(moveInput * 10f, _rigidbody2D.velocity.y);
                    if (moveInput == 0)
                    {
                        smoothingModifier *= exitMovementSmoothing;
                    }
                    else
                    {
                        smoothingModifier *= enterMovementSmoothing;
                        if (_runSquishSpeedTimer <= 0 && _playerJump.Grounded)
                        {
                            StartCoroutine(_squishable.GroundedSquish(runSquishSpeed, 0.5f, false));
                            _runSquishSpeedTimer = runSquishSpeed;
                        }
                    }

                    _rigidbody2D.velocity = Vector3.SmoothDamp(
                        _rigidbody2D.velocity,
                        targetVelocity,
                        ref _velocity,
                        smoothingModifier
                    );
                }

                if (!armed && ((moveInput > 0 && !FacingRight) || (moveInput < 0 && FacingRight)))
                {
                    FlipPlayer();
                    _weaponMovements.FlipWeapon();
                }
            }
        }


        public void FlipPlayer()
        {
            FacingRight = !FacingRight;
            Vector3 theScale = _playerDirection.localScale;
            theScale.x *= -1;
            _playerDirection.localScale = theScale;
        }
    }
}