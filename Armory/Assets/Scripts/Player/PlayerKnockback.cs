using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace Player
{
    public class PlayerKnockback : MonoBehaviour
    {
        [NonSerialized] private PlayerJump _playerJump;
        [NonSerialized] private PlayerMovements _playerMovements;
        [NonSerialized] private PlayerCrouch _playerCrouch;
        [NonSerialized] private Player _player;

        [NonSerialized] private Transform _playerTransform;
        [SerializeField] private GameObject boostParticlePrefab;
        [NonSerialized] private float _previousAngle;
        [NonSerialized] public string ShotDirection = "none";
        [NonSerialized] public Rigidbody2D Rigidbody2D;

        private void Awake()
        {
            _playerJump = GetComponent<PlayerJump>();
            _playerMovements = GetComponent<PlayerMovements>();
            _playerCrouch = GetComponent<PlayerCrouch>();
            _player = GetComponent<Player>();

            _playerTransform = transform.Find("PlayerObject");
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!_player.dead && _playerMovements.HorizontalInput != 0 &&
                _playerMovements.HorizontalInput != _playerMovements.PreviousHorizontalInput)
            {
                ShotDirection = "none";
            }
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

        public void PlayerKnockBack(Transform weaponTransform, float appliedForce,
            float playerRecoilBoostWhileEmbracingRecoil = 1f)
        {
            Vector3 knockbackVector = -weaponTransform.right;
            float shotAngle = weaponTransform.eulerAngles.z;
            if (_playerCrouch.Crouching)
            {
                appliedForce *= 0.75f;
            }

            if (Angles.AngleIsInAGivenRange(shotAngle, 160, 270))
            {
                //si le joueur tire dans un rayon de 180° en dessous de lui, il n'aura pas de friction horizontale
                BulletJump();
            }


            if (_playerJump.Grounded || !Angles.AngleIsInAGivenRange(shotAngle, 90, _previousAngle))
            {
                //si le joueur tire plus ou moins dans la même direction ds les airs, la vélo est maintenue
                Rigidbody2D.velocity = Vector2.zero;
            }
            else
            {
                Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x, 0);
            }

            string direction = (Angles.AngleToDirection(shotAngle, 45));
            ShotDirection = direction;
            if (_playerMovements.HorizontalInput == 0)
            {
                _playerMovements.PreviousHorizontalInput = 0;
            }
            else if ((direction == "left" && _playerMovements.HorizontalInput == 1)
                     || (direction == "right" && _playerMovements.HorizontalInput == -1))
                //octroie un boost horizontal dans le knockback si le joueur va dans la direction de sa vélocité knockback
            {
                knockbackVector = new Vector3(knockbackVector.x * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.y,
                    knockbackVector.z);
                StartCoroutine(BoostTrail(5));
            }

            if (direction == "down" && _playerJump.CanJumpBoost)
                //octroie un boost vertical dans le knockback si le joueur saute et tire vers le bas
            {
                knockbackVector = new Vector3(knockbackVector.x,
                    knockbackVector.y * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.z);
                StartCoroutine(BoostTrail(5));
                _playerJump.CanJumpBoost = false;
            }

            _previousAngle = shotAngle;
            Rigidbody2D.AddForce(knockbackVector * appliedForce, ForceMode2D.Impulse);
        }

        public void BulletJump()
        {
            _playerJump.Grounded = false;
            _playerJump.RecentlyJumped = true;
            StartCoroutine(BulletJumpCoroutine());
        }

        private IEnumerator BulletJumpCoroutine()
        {
            yield return new WaitForSeconds(0.1f);
            _playerJump.RecentlyJumped = false;
        }
    }
}