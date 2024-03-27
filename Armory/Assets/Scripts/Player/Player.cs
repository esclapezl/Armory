using System;
using System.Collections;
using System.Collections.Generic;
using Levels;
using Player.Controls;
using UnityEngine;
using Utils;
using weapons;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [NonSerialized] public PlayerMovements PlayerMovements;
        [NonSerialized] public PlayerJump PlayerJump;
        [NonSerialized] public PlayerCrouch PlayerCrouch;
        [NonSerialized] public PlayerKnockback PlayerKnockback;

        [NonSerialized] private Rigidbody2D _rigidbody2D;
        [NonSerialized] private LevelManager _levelManager;

        [NonSerialized] public Transform PlayerSprite;
        [NonSerialized] public SpriteRenderer PlayerSpriteRenderer;
        [NonSerialized] public SpriteRenderer PlayerFilterSpriteRenderer;
        [SerializeField] public int health = 3;
        [NonSerialized] public bool Dead = false;
        [NonSerialized] private Coroutine _damageCoroutine;

        private void Awake()
        {
            PlayerMovements = GetComponent<PlayerMovements>();
            PlayerJump = GetComponent<PlayerJump>();
            PlayerCrouch = GetComponent<PlayerCrouch>();
            PlayerKnockback = GetComponent<PlayerKnockback>();

            _rigidbody2D = GetComponent<Rigidbody2D>();
            _levelManager = ObjectSearch.FindRoot("GameManager").GetComponent<LevelManager>();

            PlayerSprite = ObjectSearch.FindChild(transform, "PlayerSprite");
            PlayerSpriteRenderer = PlayerSprite.GetComponent<SpriteRenderer>();
            PlayerFilterSpriteRenderer =
                ObjectSearch.FindChild(transform, "PlayerFilter").GetComponent<SpriteRenderer>();
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
            Dead = true;
            GetComponent<BoxCollider2D>().enabled = false;

            PlayerSpriteRenderer.transform.localPosition = new Vector3(0, 0, -5);
            StartCoroutine(DieRotation(-_rigidbody2D.velocity.x * 100));
            _rigidbody2D.AddForce(new Vector2(0, 500));
            StartCoroutine(RespawnCoroutine());
        }

        private IEnumerator DieRotation(float rotationSpeed)
        {
            float absSpeed = Mathf.Abs(rotationSpeed);
            while (absSpeed > 0 && Dead)
            {
                PlayerSpriteRenderer.transform.Rotate(0, 0, (rotationSpeed + 100) * Time.deltaTime);
                absSpeed -= Time.deltaTime;
                yield return null;
            }
        }

        private void Respawn()
        {
            Dead = false;
            StopCoroutine(DieRotation(0));
            PlayerSpriteRenderer.transform.rotation = Quaternion.identity;
            PlayerSpriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
            _rigidbody2D.velocity = Vector2.zero;
            _levelManager.CurrentLevel.StartLevel();
            GetComponent<BoxCollider2D>().enabled = true;
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
            PlayerFilterSpriteRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
            _damageCoroutine = null;
            while (PlayerFilterSpriteRenderer.color.a > 0)
            {
                PlayerFilterSpriteRenderer.color = Color.Lerp(PlayerFilterSpriteRenderer.color,
                    new Color(targetColor.r, targetColor.g, targetColor.b, 0f), Time.deltaTime * 10);
                yield return null;
            }
        }
    }
}