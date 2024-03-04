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
		[Header("Ground Settings")]
		[SerializeField] public LayerMask whatIsGround;
		[NonSerialized] private Transform _groundCheck;
		[NonSerialized] private Transform _ceilingCheck;
		const float GroundedRadius = .05f;
		[SerializeField] public bool grounded;
		const float CeilingRadius = .2f;

		[Header("Crouch Settings")]
		[SerializeField] private Collider2D crouchDisableCollider;
		[Range(0, 1)] [SerializeField] private float crouchSpeed;
		[NonSerialized] public bool Crouching;
		private bool _crouchInput;

		[Header("Player Settings")]
		[NonSerialized] private Transform _playerTransform;
		[NonSerialized] private Transform _inventoryTransform;
		[NonSerialized] public bool FacingRight = true;
		private bool _armed = true;

		[Header("Movement Settings")]
		[FormerlySerializedAs("_runSpeed")]
		[SerializeField] private int runSpeed;
		[Range(0, .3f)] [SerializeField] private float enterMovementSmoothing;
		[Range(0, .3f)] [SerializeField] private float exitMovementSmoothing;
		[NonSerialized] public int HorizontalInput;
		[NonSerialized] public int HorizontalMove;
		[NonSerialized] public int PreviousHorizontalInput = 0;
		[SerializeField] private GameObject boostParticlePrefab;

		[Header("Jump Settings")]
		[FormerlySerializedAs("_JumpForce")]
		[SerializeField] private float jumpForce;
		[NonSerialized] private bool _recentlyJumped = false;
		[NonSerialized] public bool CanJumpBoost = false;
		[SerializeField] public bool canAirControl;
		[NonSerialized] public string ShotDirection = "none";
		[SerializeField] public float airControl;
		[Range(0, .1f)] [SerializeField] private float coyoteTimeDuration;
		private float _coyoteTime;
		[Range(0, .3f)] [SerializeField] private float jumpBufferDuration;
		private float _jumpBuffer;
		private bool _jumpInput;

		private Rigidbody2D _rigidbody2D;
		private Vector3 _velocity;
		private void Awake()
		{
			_playerTransform = transform.Find("PlayerObject");
			_inventoryTransform = transform.Find("Inventory");
			_groundCheck = transform.Find("GroundCheck");
			_ceilingCheck = transform.Find("CeilingCheck");
			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			HorizontalInput = (int) Input.GetAxisRaw("Horizontal");
			HorizontalMove = (HorizontalInput * runSpeed);
			if (HorizontalInput != 0 && HorizontalInput != PreviousHorizontalInput) 
			{
				ShotDirection = "none";
			}
			PreviousHorizontalInput = HorizontalInput != 0 ? HorizontalInput : PreviousHorizontalInput;
			if (Input.GetButton("Crouch"))
			{
				_crouchInput = true;
			}
			
			if (Input.GetButtonDown("Jump"))
			{
				_jumpInput = true;
			}
		}
		
		private void FixedUpdate()
		{
			MoveControl(HorizontalMove * Time.fixedDeltaTime, _armed);
			CrouchControl(_crouchInput);
			_crouchInput = false;
			JumpControl(_jumpInput);
			_jumpInput = false;
			grounded = false;
			_groundCheck.GetComponent<SpriteRenderer>().color = Color.red;
			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.
			if (!_recentlyJumped)
			{
				Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, GroundedRadius, whatIsGround);
				for (int i = 0; i < colliders.Length; i++)
				{
					if (colliders[i].gameObject != gameObject)
					{
						_groundCheck.GetComponent<SpriteRenderer>().color = Color.green;
						grounded = true;
						ShotDirection = "none";
						PreviousHorizontalInput = 0;
					}
				}
			}
		}

		public void MoveControl(float moveInput, bool armed)
		{
			//only control the player if grounded or airControl is turned on
			if (grounded || canAirControl)
			{
				// If crouching
				if (Crouching)
				{
					// Reduce the speed by the crouchSpeed multiplier
					moveInput *= crouchSpeed;
			
					// Disable one of the colliders when crouching
					if (crouchDisableCollider != null)
					{
						crouchDisableCollider.enabled = false;
					}
				} else
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
				    (!(ShotDirection == "left" && PreviousHorizontalInput == 1) 
				    && !(ShotDirection == "right" && PreviousHorizontalInput == -1)
				    && HorizontalInput != 0)
				    ) //au sol ou pas dans la direction du knockback
				{ 
					_playerTransform.GetComponent<SpriteRenderer>().color = Color.black;
					float smoothingModifier = grounded ? 1 : airControl; // CONTROL IN AIR
					Vector3 targetVelocity = new Vector2(moveInput * 10f, _rigidbody2D.velocity.y);
					if (moveInput == 0) //checks if player stopped pressing
					{
						smoothingModifier *= exitMovementSmoothing;
					}
					else
					{
						smoothingModifier *= enterMovementSmoothing;
					}
					_rigidbody2D.velocity = Vector3.SmoothDamp(
						_rigidbody2D.velocity,
						targetVelocity,
						ref _velocity,
						smoothingModifier
					);
				}
				else
				{
					_playerTransform.GetComponent<SpriteRenderer>().color = Color.white;
				}
				
				if ((moveInput > 0 && !FacingRight && !armed) || (moveInput < 0 && FacingRight && !armed))
				{
					Flip();
				}
			}
		}


		public void Flip()
		{
			// Switch the way the player is labelled as facing.
			FacingRight = !FacingRight;

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
				else if(_coyoteTime < 0) //leaves ground
				{
					_coyoteTime = coyoteTimeDuration;
				}
			
				if(_jumpBuffer > 0)
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
			_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,0f);
			_rigidbody2D.AddForce(new Vector2(0f, jumpForce));
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
			bool blocked = Physics2D.OverlapCircle(_ceilingCheck.position, CeilingRadius, whatIsGround);
			if (crouchInput && !Crouching)
			{
				Crouch();
			}
			else if(!crouchInput && Crouching && !blocked)
			{
				StandUp();
			}
		}

		private void Crouch()
		{
			Crouching = true;
			float squishFactor = 0.6f;
			_playerTransform.localScale = new Vector3(_playerTransform.localScale.x, squishFactor, 1);
			_playerTransform.localPosition = new Vector3(0, (-1+squishFactor)/2, 0);
			_inventoryTransform.localPosition = new Vector3(0, -squishFactor/2, 0);
		}

		private void StandUp()
		{
			Crouching = false;
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

		public void PlayerKnockBack(Transform weaponTransform, float appliedForce, float playerRecoilBoostWhileEmbracingRecoil = 1f)
		{
			Vector3 knockbackVector = -weaponTransform.right;
			float shotAngle = weaponTransform.eulerAngles.z;
			if (Crouching)
			{
				appliedForce *= 0.75f;
			}

			if (Angles.AngleIsInAGivenRange(shotAngle, 160, 270))
			{ //si le joueur tire dans un rayon de 180° en dessous de lui, il n'aura pas de friction horizontale
				BulletJump();
			}

			//AFAIR ICI
			_rigidbody2D.velocity = Vector2.zero;
			string direction = (Angles.AngleToDirection(shotAngle, 45));
			ShotDirection = direction;
			if (HorizontalInput == 0)
			{
				PreviousHorizontalInput = 0;
			}
			else if((direction == "left" && HorizontalInput == 1) 
			        || (direction == "right" && HorizontalInput == -1))
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
			_rigidbody2D.AddForce(knockbackVector * appliedForce, ForceMode2D.Impulse);
		}
	}
}
