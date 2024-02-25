using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [NonSerialized] public SpriteRenderer PlayerSprite;
        [NonSerialized] public SpriteRenderer PlayerFilterSprite;
        [SerializeField] public int health = 3;
        private Coroutine _damageCoroutine;

        private void Awake()
        {
            PlayerSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
            PlayerFilterSprite = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        }

        public void TakeDamage()
        {
            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
            }
            _damageCoroutine = StartCoroutine(TakeDamageCoroutine());
        }
    
        private IEnumerator TakeDamageCoroutine()
        {
            Color targetColor = new Color(1, 0, 0);
            PlayerFilterSprite.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
            _damageCoroutine = null;
            while(PlayerFilterSprite.color.a > 0)
            {
                PlayerFilterSprite.color = Color.Lerp(PlayerFilterSprite.color, new Color(targetColor.r, targetColor.g, targetColor.b, 0f), Time.deltaTime * 10);
                yield return null;
            }
        }
    }
}


