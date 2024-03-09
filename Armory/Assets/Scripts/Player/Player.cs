using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [NonSerialized] private PlayerMovements _playerMovements;
        [NonSerialized] private Rigidbody2D _rigidbody2D;
        [NonSerialized] private GameManager _gameManager;
        [NonSerialized] public SpriteRenderer PlayerSprite;
        [NonSerialized] public SpriteRenderer PlayerFilterSprite;
        [SerializeField] public int health = 3;
        private Coroutine _damageCoroutine;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _playerMovements = GetComponent<PlayerMovements>();
            
            PlayerSprite = ObjectSearch.FindChild(transform, "PlayerObject").GetComponent<SpriteRenderer>();
            PlayerFilterSprite = ObjectSearch.FindChild(transform, "PlayerFilter").GetComponent<SpriteRenderer>();
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Danger"))
            {
                Die();
            }
        }

        private void Die()
        {
            _playerMovements.dead = true;
            GetComponent<BoxCollider2D>().enabled = false;
            
            PlayerSprite.transform.position = new Vector3(0,0, -10);
            _rigidbody2D.AddForce(new Vector2(0, 500)); 
            _rigidbody2D.AddTorque(500);
            
            StartCoroutine(RespawnCoroutine());
        }

        private void Respawn()
        {
            PlayerSprite.transform.position = new Vector3(0,0, 0);
            _gameManager.CurrentLevel.StartLevel();
            GetComponent<BoxCollider2D>().enabled = true;
            _playerMovements.dead = false;
        }
    
        private IEnumerator RespawnCoroutine()
        {
            yield return new WaitForSeconds(1);
            Respawn();
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


