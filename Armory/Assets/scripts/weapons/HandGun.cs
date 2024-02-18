using System.Collections;
using UnityEngine;

namespace weapons
{
    public class HandGun : MonoBehaviour
    {
        public Transform playerTransform;
        public Transform cannonTransform;
        
        public float recoil;
        public float recoilMultiplier;
        private Coroutine _knockBackCoroutine;
        
        public int magazineSize;
        public int currentAmmo;
        public int totalAmmo;
        public float reloadTime;
        public float bulletSpeed;
        private bool _isReloading;
        
        public float fireRate;
        private float _nextFireTime;
        
        public GameObject bulletPrefab;

        private Vector3 _originalPosition;
        private Vector3 _recoilPosition;
        
        private void Start()
        {
            var vector3 = transform.localPosition;
            vector3.x = playerTransform.localScale.x;
            transform.localPosition = vector3;
            _originalPosition = transform.localPosition;
            _nextFireTime = 0f;
        }

        void Update()
        {
            if (Input.GetButtonDown("Fire") && _nextFireTime <= 0) {
                Shoot();
            }

            // Move the gun back to its original position smoothly
            transform.localPosition = Vector3.Lerp(transform.localPosition, _originalPosition, Time.deltaTime * 5);
            if (_nextFireTime > 0)
            {
                _nextFireTime -= Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            _nextFireTime -= Time.deltaTime;
        }

        private void Shoot()
        {
            _recoilPosition = new Vector3(_originalPosition.x - recoil, _originalPosition.y, _originalPosition.z);
            transform.localPosition = _recoilPosition;
            _nextFireTime = fireRate;

            Instantiate(bulletPrefab, cannonTransform.position, transform.rotation);

            // If a knockback coroutine is already running, stop it
            if (_knockBackCoroutine != null)
            {
                StopCoroutine(_knockBackCoroutine);
            }

            // Start a new knockback coroutine
            _knockBackCoroutine = StartCoroutine(KnockBack());
        }

        private IEnumerator KnockBack()
        {
            Vector2 knockbackDirection = -cannonTransform.right;
            Rigidbody2D playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();

            float currentRecoil = recoil * recoilMultiplier;
            playerRigidbody.velocity = Vector2.zero;
            while (currentRecoil > 0)
            {
                playerRigidbody.AddForce(knockbackDirection * currentRecoil, ForceMode2D.Impulse);
                currentRecoil -= Time.fixedDeltaTime * 10; // Adjust the 10 to control how quickly the force decreases

                yield return null; // Wait for the next frame
            }

            _knockBackCoroutine = null; // Reset the coroutine reference when it's done
        }
    }
}
