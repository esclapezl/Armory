using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Player
{
    public class PlayerCrouch : MonoBehaviour
    {
        [NonSerialized] private PlayerJump _playerJump;
        [NonSerialized] private PlayerMovements _playerMovements;
        [NonSerialized] private PlayerKnockback _playerKnockback;
        [NonSerialized] private Player _player;
        
        [NonSerialized] public bool Crouching;
        [SerializeField] public float crouchSpeed;
        [NonSerialized] private bool _crouchInput;
        [NonSerialized] private Vector2 _originalColliderSize;
        [NonSerialized] private Vector2 _originalColliderOffset;
        [NonSerialized] private Transform _playerTransform;
        [NonSerialized] private Transform _inventoryTransform;
        [NonSerialized] private Transform _ceilingCheck;
        [NonSerialized] private Vector2 _ceillingCheckSize;
        [SerializeField] private LayerMask whatIsCeilling;

        private void Awake()
        {
            _playerJump = GetComponent<PlayerJump>();
            _playerMovements = GetComponent<PlayerMovements>();
            _playerKnockback = GetComponent<PlayerKnockback>();
            _player = GetComponent<Player>();
            
            
            _originalColliderSize = GetComponent<BoxCollider2D>().size;
            _originalColliderOffset = GetComponent<BoxCollider2D>().offset;
            _playerTransform = transform.Find("PlayerObject");
            _inventoryTransform = transform.Find("Inventory");
            _ceilingCheck = ObjectSearch.FindChild(transform, "CeilingCheck");
            _ceillingCheckSize = new Vector2(0.1f, GetComponent<BoxCollider2D>().size.x);
        }

        private void Update()
        {
            if (!_player.dead && Input.GetButton("Crouch"))
            {
                _crouchInput = true;
            }
        }

        private void FixedUpdate()
        {
            if (!_player.dead)
            {
                CrouchControl(_crouchInput);
                _crouchInput = false;
            }
        }

        public void CrouchControl(bool crouchInput)
        {
            bool blocked = false;
            Collider2D[] colliders = Physics2D.OverlapBoxAll(_ceilingCheck.position, _ceillingCheckSize, whatIsCeilling);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    blocked = true;
                }
            }

            if (crouchInput && !Crouching)
            {
                Crouch();
            }
            else if (!crouchInput && Crouching && !blocked)
            {
                StandUp();
            }
        }

        private void Crouch()
        {
            Crouching = true;
            float squishFactor = 0.6f;
            _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, squishFactor, 1);
            _playerTransform.localPosition = new Vector3(0, (-1 + squishFactor) / 2, 0);
            _inventoryTransform.localPosition = new Vector3(0, -squishFactor / 2, 0);
            
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(_originalColliderSize.x, squishFactor);
            collider.offset = new Vector2(_originalColliderOffset.x, -(1-squishFactor)/2);
        }

        private void StandUp()
        {
            Crouching = false;
            _playerTransform.localScale = new Vector3(_playerTransform.localScale.x, 1, 1);
            _playerTransform.localPosition = new Vector3(0, 0, 0);
            _inventoryTransform.localPosition = new Vector3(0, 0, 0);
            
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            collider.size = new Vector2(_originalColliderSize.x, _originalColliderSize.y);
            collider.offset = new Vector2(_originalColliderOffset.x, _originalColliderOffset.y);
        }
    }
}
