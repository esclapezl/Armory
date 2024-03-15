using System;
using UnityEngine;

namespace Levels.Restartables
{
    public class AmmoPickUpRestart : Restartable
    {
        private SpriteRenderer _spriteRender;
        private BoxCollider2D _boxCollider;

        private void Awake()
        {
            _spriteRender = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        public override void Restart()
        {
            _spriteRender.enabled = true;
            _boxCollider.enabled = true;
        }

        public override void Exit()
        {
            _spriteRender.enabled = false;
            _boxCollider.enabled = false;
        }
    }
}