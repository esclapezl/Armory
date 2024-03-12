using System;
using UnityEngine;

namespace Levels.Restartables
{
    public class AmmoRestart : Restartable
    {
        private SpriteRenderer _spriteRender;
        private BoxCollider2D _boxCollider;
        private Rigidbody2D _rigidbody2D;
        private Vector2 _boxColliderSize;

        private void Awake()
        {
            _spriteRender = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _boxColliderSize = _boxCollider.size;
        }

        public override void Restart()
        {
            GetComponent<SpriteRenderer>().enabled = true;
            if (_boxCollider == null)
            {
                _boxCollider = gameObject.AddComponent<BoxCollider2D>();
                _boxCollider.size = _boxColliderSize;
                _boxCollider.isTrigger = true;
            }

            if (_rigidbody2D == null)
            {
                _rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
                _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        public override void Exit()
        {
            if (_boxCollider != null)
            {
                Destroy(_boxCollider);
                Destroy(_rigidbody2D);
            }
        }
    }
}