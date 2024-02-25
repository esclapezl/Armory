using System;
using UnityEngine;

namespace Player
{
	public class PlayerMovements : MonoBehaviour
	{
		[SerializeField] private LayerMask whatIsGround;							// A mask determining what is ground to the character
		[NonSerialized] private Transform _groundCheck;							// A position marking where to check if the player is grounded.
		[NonSerialized] private Transform _ceilingCheck;							// A position marking where to check for ceilings
		[SerializeField] private Collider2D m_CrouchDisableCollider;				// A collider that will be disabled when crouching
		[NonSerialized] private GameObject _playerObject;
		const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
		[NonSerialized] public bool grounded;        
		const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
		private Rigidbody2D _rigidbody2D;
		[NonSerialized]public bool FacingRight = true;  // For determining which way the player is currently facing.
		private Vector3 _Velocity = Vector3.zero;
		private bool _armed = true;
	
		[Header("Movement Settings")]
		[SerializeField] private float _runSpeed;
		[Range(0, .3f)] [SerializeField] private float _EnterMovementSmoothing;
		[Range(0, .3f)] [SerializeField] private float _ExitMovementSmoothing;
		private float _horizontalMove;

		[Header("Jump Settings")]
		[SerializeField] private float _JumpForce;
		[SerializeField] private bool _canAirControl;      
		[SerializeField] public float airFriction; //for gun knockback
		[SerializeField] public float airControl; //for moving in the air
		[Range(0, .1f)] [SerializeField] private float _coyoteTimeDuration;
		private float _coyoteTime;
		[Range(0, .3f)] [SerializeField] private float _jumpBufferDuration;
		private float _jumpBuffer;
		private bool _jumpInput;

		[Header("Crouch Settings")]
		[Range(0, 1)] [SerializeField] private float _CrouchSpeed;          
		private bool _crouching;
		private bool _crouchInput;
		private void Awake()
		{
			_playerObject = transform.Find("PlayerObject").gameObject;
			_groundCheck = transform.Find("GroundCheck");
			_ceilingCheck = transform.Find("CeilingCheck");
			_rigidbody2D = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			_horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
			if (Input.GetButtonDown("Crouch"))
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
			MoveControl(_horizontalMove * Time.fixedDeltaTime, _armed);
			CrouchControl(_crouchInput);
			_crouchInput = false;
			JumpControl(_jumpInput);
			_jumpInput = false;
			grounded = false;
			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.
			Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, k_GroundedRadius, whatIsGround);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					grounded = true;
				}
			}
		}

		public void MoveControl(float moveInput, bool armed)
		{
			//only control the player if grounded or airControl is turned on
			if (grounded || _canAirControl)
			{
				// If crouching
				if (_crouching)
				{
					// Reduce the speed by the crouchSpeed multiplier
					moveInput *= _CrouchSpeed;
			
					// Disable one of the colliders when crouching
					if (m_CrouchDisableCollider != null)
					{
						m_CrouchDisableCollider.enabled = false;
					}
				} else
				{
					// Enable the collider when not crouching
					if (m_CrouchDisableCollider != null)
					{
						m_CrouchDisableCollider.enabled = true;
					}
				}

				// Move the character by finding the target velocity
				float smoothingModifier = grounded ? 1 : airControl; // CONTROL IN AIR
				Vector3 targetVelocity = new Vector2(moveInput * 10f, _rigidbody2D.velocity.y);
				if (moveInput == 0) //checks if player stopped pressing
				{
					smoothingModifier *= _ExitMovementSmoothing;
				}
				else
				{
					smoothingModifier *= _EnterMovementSmoothing;
				}
				_rigidbody2D.velocity = Vector3.SmoothDamp(
					_rigidbody2D.velocity,
					targetVelocity,
					ref _Velocity,
					smoothingModifier
				);


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
				if (grounded || _coyoteTime > 0) //check for ground or remaining coyote time
				{
					Jump();
				}
				else
				{
					_jumpBuffer = _jumpBufferDuration;
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
					_coyoteTime = _coyoteTimeDuration;
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
			_rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,0f);
			_rigidbody2D.AddForce(new Vector2(0f, _JumpForce));
			_coyoteTime = 0;
			_jumpBuffer = 0;
		}

		public void CrouchControl(bool crouchInput)
		{
			_crouching = crouchInput;
			if (!crouchInput)
			{
				// If the character has a ceiling preventing them from standing up, keep them crouching
				if (Physics2D.OverlapCircle(_ceilingCheck.position, k_CeilingRadius, whatIsGround))
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
