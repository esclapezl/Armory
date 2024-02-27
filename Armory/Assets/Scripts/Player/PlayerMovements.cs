using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
	public class PlayerMovements : MonoBehaviour
	{
		[Header("Ground Settings")]
		[SerializeField] public LayerMask whatIsGround;
		[NonSerialized] private Transform _groundCheck;
		[NonSerialized] private Transform _ceilingCheck;
		const float GroundedRadius = .2f;
		[NonSerialized] public bool Grounded;
		const float CeilingRadius = .2f;

		[Header("Crouch Settings")]
		[SerializeField] private Collider2D crouchDisableCollider;
		[Range(0, 1)] [SerializeField] private float crouchSpeed;
		private bool _crouching;
		private bool _crouchInput;

		[Header("Player Settings")]
		[NonSerialized] private GameObject _playerObject;
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

		[Header("Jump Settings")]
		[FormerlySerializedAs("_JumpForce")]
		[SerializeField] private float jumpForce;
		[NonSerialized] public bool RecentlyJumped = false;
		[SerializeField] public bool canAirControl;
		[NonSerialized] public string KnockBackDirection = "none";
		[SerializeField] public float airControl;
		[Range(0, .1f)] [SerializeField] private float coyoteTimeDuration;
		private float _coyoteTime;
		[Range(0, .3f)] [SerializeField] private float jumpBufferDuration;
		private float _jumpBuffer;
		private bool _jumpInput;

		private Rigidbody2D _rigidbody2D;
		private Vector3 _velocity = Vector3.zero;
		private void Awake()
		{
			_playerObject = transform.Find("PlayerObject").gameObject;
			_groundCheck = transform.Find("GroundCheck");
			_ceilingCheck = transform.Find("CeilingCheck");
			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			HorizontalInput = (int) Input.GetAxisRaw("Horizontal");
			HorizontalMove = (HorizontalInput * runSpeed);
			if (HorizontalMove != 0 && HorizontalMove != PreviousHorizontalInput) 
			{
				KnockBackDirection = "none";
			}
			PreviousHorizontalInput = HorizontalMove != 0 ? HorizontalMove : PreviousHorizontalInput;
			if (Input.GetButtonDown("Crouch"))
			{
				_crouchInput = true;
			}
			
			if (_rigidbody2D.velocity.y < 1 && _rigidbody2D.velocity.y < 3 && RecentlyJumped)
			{
				//A CHANGER EN COROUTINE
				RecentlyJumped = false;
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
			Grounded = false;
			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.
			Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, GroundedRadius, whatIsGround);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					Grounded = true;
					KnockBackDirection = "none";
					PreviousHorizontalInput = 0;
				}
			}
		}

		public void MoveControl(float moveInput, bool armed)
		{
			//only control the player if grounded or airControl is turned on
			if (Grounded || canAirControl)
			{
				// If crouching
				if (_crouching)
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
				if (Grounded
				    || (!(KnockBackDirection == "left" && PreviousHorizontalInput == 1) 
				    && !(KnockBackDirection == "right" && PreviousHorizontalInput == -1))) //au sol ou pas dans la direction du knockback
				{ 
					float smoothingModifier = Grounded ? 1 : airControl; // CONTROL IN AIR
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
			Vector3 theScale = _playerObject.transform.localScale;
			theScale.x *= -1;
			_playerObject.transform.localScale = theScale;
		}

		public void JumpControl(bool jumpInput)
		{
			if (jumpInput)
			{
				if (Grounded || _coyoteTime > 0) //check for ground or remaining coyote time
				{
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
			Grounded = false;
			RecentlyJumped = true;
			_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,0f);
			_rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			_coyoteTime = 0;
			_jumpBuffer = 0;
		}

		public void CrouchControl(bool crouchInput)
		{
			_crouching = crouchInput;
			if (!crouchInput)
			{
				// If the character has a ceiling preventing them from standing up, keep them crouching
				if (Physics2D.OverlapCircle(_ceilingCheck.position, CeilingRadius, whatIsGround))
				{
					_crouching = true;
				}
			}
		}

		private void Crouch()
		{
		
		}

		private void StandUp()
		{
		
		}
	}
}
