using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public SpriteRenderer playerSprite;
        public SpriteRenderer playerFilterSprite;
        public int health = 3;
        private Coroutine _damageCoroutine;
    
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
            playerFilterSprite.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
            _damageCoroutine = null;
            while(playerFilterSprite.color.a > 0)
            {
                playerFilterSprite.color = Color.Lerp(playerFilterSprite.color, new Color(targetColor.r, targetColor.g, targetColor.b, 0f), Time.deltaTime * 10);
                yield return null;
            }
        }
    }
}


