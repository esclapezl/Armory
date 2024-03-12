using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using ObjectSearch = Utils.ObjectSearch;

namespace Player
{
    public class PlayerMovements : MonoBehaviour
    {
        [SerializeField] public bool dead;

        [Header("Ground Settings")] [SerializeField]
        public LayerMask whatIsGround;

        [NonSerialized] private Transform _groundCheck;
        [NonSerialized] private Transform _ceilingCheck;
        [SerializeField] public bool grounded;
        [NonSerialized] private Vector2 _groundCheckSize;
        const float CeilingRadius = .2f;

        [Header("Crouch Settings")] [SerializeField]
        private Collider2D crouchDisableCollider;

        [Range(0, 1)] [SerializeField] private float crouchSpeed;
        [NonSerialized] public bool crouching;
        private bool _crouchInput;

        [Header("Player Settings")] [NonSerialized]
        private Transform _playerTransform;

        [NonSerialized] private Transform _inventoryTransform;
        [NonSerialized] public bool facingRight = true;
        [NonSerialized] public bool Armed = true;

        [Header("Movement Settings")] [FormerlySerializedAs("_runSpeed")] [SerializeField]
        private int runSpeed;

        [Range(0, .3f)] [SerializeField] private float enterMovementSmoothing;
        [Range(0, .3f)] [SerializeField] private float exitMovementSmoothing;
        [NonSerialized] public int horizontalInput;
        [NonSerialized] public int horizontalMove;
        [NonSerialized] public int previousHorizontalInput = 0;
        [SerializeField] private GameObject boostParticlePrefab;

        [Header("Jump Settings")] [SerializeField]
        private float jumpForce;

        [NonSerialized] private bool _recentlyJumped = false;
        [NonSerialized] public bool CanJumpBoost = false;
        [SerializeField] public bool canAirControl;
        [NonSerialized] public string ShotDirection = "none";
        [NonSerialized] private float _previousAngle;
        [SerializeField] public float airControl;
        [Range(0, .1f)] [SerializeField] private float coyoteTimeDuration;
        private float _coyoteTime;
        [Range(0, .3f)] [SerializeField] private float jumpBufferDuration;
        private float _jumpBuffer;
        private bool _jumpInput;

        [NonSerialized] public Rigidbody2D Rigidbody2D;
        [NonSerialized] private Vector3 _velocity;

        private void Awake()
        {
            _groundCheckSize = new Vector2(0.1f, GetComponent<BoxCollider2D>().size.x);
            _playerTransform = transform.Find("PlayerObject");
            _inventoryTransform = transform.Find("Inventory");
            _groundCheck = ObjectSearch.FindChild(transform, "GroundCheck");
            _ceilingCheck = ObjectSearch.FindChild(transform, "CeilingCheck");
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!dead)
            {
                horizontalInput = (int)Input.GetAxisRaw("Horizontal");
                horizontalMove = (horizontalInput * runSpeed);
                if (horizontalInput != 0 && horizontalInput != previousHorizontalInput)
                {
                    ShotDirection = "none";
                }

                previousHorizontalInput = horizontalInput != 0 ? horizontalInput : previousHorizontalInput;
                if (Input.GetButton("Crouch"))
                {
                    _crouchInput = true;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    _jumpInput = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!dead)
            {
                MoveControl(horizontalMove * Time.fixedDeltaTime, Armed);
                CrouchControl(_crouchInput);
                _crouchInput = false;
                JumpControl(_jumpInput);
                _jumpInput = false;
                grounded = false;
                // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
                // This can be done using layers instead but Sample Assets will not overwrite your project settings.
                if (!_recentlyJumped)
                {
                    Collider2D[] colliders =
                        Physics2D.OverlapBoxAll(_groundCheck.position, _groundCheckSize, whatIsGround);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (colliders[i].gameObject != gameObject)
                        {
                            grounded = true;
                            ShotDirection = "none";
                            previousHorizontalInput = 0;
                        }
                    }
                }
            }
        }

        public void MoveControl(float moveInput, bool armed)
        {
            if (grounded || canAirControl)
            {
                if (crouching)
                {
                    moveInput *= crouchSpeed;

                    // Disable one of the colliders when crouching
                    if (crouchDisableCollider != null)
                    {
                        crouchDisableCollider.enabled = false;
                    }
                }
                else
                {
                    // Enable the collider when not crouching
                    if (crouchDisableCollider != null)
                    {
                        crouchDisableCollider.enabled = true;
                    }
                }

                // Move the character by finding the target velocity
                if ((grounded && ShotDirection != "down")
                    ||
                    ShotDirection == "none"
                    ||
                    (!(ShotDirection == "left" && previousHorizontalInput == 1)
                     && !(ShotDirection == "right" && previousHorizontalInput == -1)
                     && horizontalInput != 0)
                   ) //au sol ou pas dans la direction du knockback
                {
                    float smoothingModifier = grounded ? 1 : airControl; // CONTROL IN AIR
                    Vector3 targetVelocity = new Vector2(moveInput * 10f, Rigidbody2D.velocity.y);
                    if (moveInput == 0) //checks if player stopped pressing
                    {
                        smoothingModifier *= exitMovementSmoothing;
                    }
                    else
                    {
                        smoothingModifier *= enterMovementSmoothing;
                    }

                    Rigidbody2D.velocity = Vector3.SmoothDamp(
                        Rigidbody2D.velocity,
                        targetVelocity,
                        ref _velocity,
                        smoothingModifier
                    );
                }

                if ((moveInput > 0 && !facingRight && !armed) || (moveInput < 0 && facingRight && !armed))
                {
                    Flip();
                }
            }
        }


        public void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = _playerTransform.localScale;
            theScale.x *= -1;
            _playerTransform.localScale = theScale;
        }

        public void JumpControl(bool jumpInput)
        {
            if (jumpInput)
            {
                if (grounded || _coyoteTime > 0) //check for ground or remaining coyote time
                {
                    Jump();
                }
                else
                {
                    _jumpBuffer = jumpBufferDuration;
                }
            }
            else if (grounded)
            {
                _coyoteTime = -1f;
                if (_jumpBuffer > 0) //check for remaining jump buffer
                {
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
        }

        private void Jump()
        {
            grounded = false;
            _recentlyJumped = true;
            CanJumpBoost = true;
            Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, 0f);
            Rigidbody2D.AddForce(new Vector2(0f, jumpForce));
            _coyoteTime = 0;
            _jumpBuffer = 0;
            StartCoroutine(JumpCoroutine());
        }

        private IEnumerator JumpCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            _recentlyJumped = false;
            CanJumpBoost = false;
        }

        public void BulletJump()
        {
            grounded = false;
            _recentlyJumped = true;
            StartCoroutine(BulletJumpCoroutine());
        }

        private IEnumerator BulletJumpCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            _recentlyJumped = false;
        }

        public void CrouchControl(bool crouchInput)
        {
            bool blocked = false;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(_ceilingCheck.position, _groundCheckSize, whatIsGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    blocked = true;
                }
            }

            if (crouchInput && !crouching)
            {
                Crouch();
            }
            else if (!crouchInput && crouching && !blocked)
            {
                StandUp();
            }
        }

        private void Crouch()
        {
            crouching = true;
            float squishFactor = 0.6f;
            _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, squishFactor, 1);
            _playerTransform.localPosition = new Vector3(0, (-1 + squishFactor) / 2, 0);
            _inventoryTransform.localPosition = new Vector3(0, -squishFactor / 2, 0);
        }

        private void StandUp()
        {
            crouching = false;
            _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, 1, 1);
            _playerTransform.localPosition = new Vector3(0, 0, 0);
            _inventoryTransform.localPosition = new Vector3(0, 0, 0);
        }

        public IEnumerator BoostTrail(int particles)
        {
            Sprite playerSprite = _playerTransform.GetComponent<SpriteRenderer>().sprite;
            for (int i = 0; i < particles; i++)
            {
                GameObject particle = Instantiate(boostParticlePrefab, _playerTransform.position, Quaternion.identity);
                particle.transform.localScale = new Vector3(_playerTransform.localScale.x * 0.75f,
                    _playerTransform.localScale.y * 0.75f,
                    1);
                particle.GetComponent<SpriteMask>().sprite = playerSprite;
                yield return new WaitForSeconds(0.05f);
            }
        }

        public void PlayerKnockBack(Transform weaponTransform, float appliedForce,
            float playerRecoilBoostWhileEmbracingRecoil = 1f)
        {
            Vector3 knockbackVector = -weaponTransform.right;
            float shotAngle = weaponTransform.eulerAngles.z;
            if (crouching)
            {
                appliedForce *= 0.75f;
            }

            if (Angles.AngleIsInAGivenRange(shotAngle, 160, 270))
            {
                //si le joueur tire dans un rayon de 180° en dessous de lui, il n'aura pas de friction horizontale
                BulletJump();
            }


            if (grounded || !Angles.AngleIsInAGivenRange(shotAngle, 90, _previousAngle))
            {
                //si le joueur tire plus ou moins dans la même direction ds les airs, la vélo est maintenue
                Rigidbody2D.velocity = Vector2.zero;
            }
            else
            {
                Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, 0);
            }

            string direction = (Angles.AngleToDirection(shotAngle, 45));
            ShotDirection = direction;
            if (horizontalInput == 0)
            {
                previousHorizontalInput = 0;
            }
            else if ((direction == "left" && horizontalInput == 1)
                     || (direction == "right" && horizontalInput == -1))
                //octroie un boost horizontal dans le knockback si le joueur va dans la direction de sa vélocité knockback
            {
                knockbackVector = new Vector3(knockbackVector.x * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.y,
                    knockbackVector.z);
                StartCoroutine(BoostTrail(5));
            }

            if (direction == "down" && CanJumpBoost)
                //octroie un boost vertical dans le knockback si le joueur saute et tire vers le bas
            {
                knockbackVector = new Vector3(knockbackVector.x,
                    knockbackVector.y * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.z);
                StartCoroutine(BoostTrail(5));
                CanJumpBoost = false;
            }

            _previousAngle = shotAngle;
            Rigidbody2D.AddForce(knockbackVector * appliedForce, ForceMode2D.Impulse);
        }
    }
}