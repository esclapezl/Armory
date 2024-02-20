using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weapons.HandGun
{
    public class HandGun : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] public PlayerController playerController;
        [SerializeField] public Transform playerTransform;
        [SerializeField] public Transform cannonTransform;

        [Header("Recoil Settings")]
        [SerializeField] [Range(0, 1)] public float recoil;
        [SerializeField] [Range(0, 100)] public float recoilMultiplier;
        private Coroutine _playerKnockBackCoroutine;
        private Coroutine _gunKnockBackCoroutine;

        [Header("Bullet Settings")]
        [SerializeField] public GameObject bulletPrefab;
        [SerializeField] [Range(0, 100)] public int magazineSize;
        public int currentAmmo;
        public int totalAmmo;
        [SerializeField] [Range(0, 10)] public float reloadTime;
        private List<GameObject> _ammoDisplay;
        private bool _isReloading;
        private Coroutine _reloadCoroutine;

        [Header("UID Bullet Settings")]
        [SerializeField] public GameObject uidBulletPrefab;
        [SerializeField] [Range(0, 1)] public float maxBulletSize;
        [SerializeField] [Range(0, 1)] public float minDisplayGap;

        [Header("Fire Rate Settings")]
        [SerializeField] [Range(0, 1)] public float fireRate;
        private float _nextFireTime;

        private Vector3 _originalPosition;
        private Vector3 _recoilPosition;
        
        private void Start()
        {
            var vector3 = transform.localPosition;
            vector3.x = playerTransform.localScale.x;
            transform.localPosition = vector3;
            _originalPosition = transform.localPosition;
            _nextFireTime = 0f;
            currentAmmo = magazineSize;
            
            _ammoDisplay = new List<GameObject>();
            DisplayAmmo();
        }

        void Update()
        {
            if (Input.GetButtonDown("Fire") && _nextFireTime <= 0 && currentAmmo > 0 && !_isReloading) {
                Shoot();
            }

            if (((playerController.m_Grounded && currentAmmo == 0) || 
                 (Input.GetButtonDown("Reload") && currentAmmo < magazineSize))
                && !_isReloading &&  totalAmmo > 0)
            {
                if (_reloadCoroutine != null)
                {
                    StopCoroutine(_reloadCoroutine);
                }
                _reloadCoroutine = StartCoroutine(Reload());
            }
        }

        private void FixedUpdate()
        {
            if (_nextFireTime > 0)
            {
                _nextFireTime -= Time.deltaTime;
            }
        }

        private void Shoot()
        {
            _nextFireTime = fireRate;
            Instantiate(bulletPrefab, cannonTransform.position, transform.rotation);
            ToggleUidBullet(_ammoDisplay.Count-currentAmmo);
            currentAmmo--;
            
            //reset la vélocité actuelle du rigidbody du joueur
            if (_playerKnockBackCoroutine != null)
            {
                StopCoroutine(_playerKnockBackCoroutine);
            }
            _playerKnockBackCoroutine = StartCoroutine(PlayerKnockBack());
            
            if(_gunKnockBackCoroutine != null)
            {
                StopCoroutine(_gunKnockBackCoroutine);
            }
            _gunKnockBackCoroutine = StartCoroutine(GunKnockBack());
        }

        private IEnumerator PlayerKnockBack()
        {
            Vector2 knockbackDirection = -cannonTransform.right;
            Rigidbody2D playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();

            float currentRecoil = recoil * recoilMultiplier;
            playerRigidbody.velocity = Vector2.zero;
            float airFriction = playerController.airFriction;
            while (currentRecoil > 0)
            {
                playerRigidbody.AddForce(knockbackDirection * currentRecoil, ForceMode2D.Impulse);
                currentRecoil -= Time.fixedDeltaTime * airFriction; // Adjust the 10 to control how quickly the force decreases
                yield return null;
            }
            _playerKnockBackCoroutine = null;
        }
        
        private IEnumerator GunKnockBack()
        {
            _recoilPosition = new Vector3(_originalPosition.x - recoil, _originalPosition.y, _originalPosition.z);
            transform.localPosition = _recoilPosition;
            while (Vector3.Distance(transform.localPosition, _originalPosition) > 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _originalPosition, Time.deltaTime * 5);
                yield return null;
            }
            _gunKnockBackCoroutine = null;
        }

        private IEnumerator Reload()
        {
            _isReloading = true;
            Quaternion originalRotation = transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, 90 * transform.localScale.y);

            // Rotate quickly to -90 degrees
            while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.01f)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Time.deltaTime * 500); // Adjust the 500 to control how quickly the gun rotates
                yield return null;
            }
            
            //refill mag
            for (int i = Mathf.Min(totalAmmo, magazineSize - currentAmmo); i > 0; i--)
            {
                EnableBullet(i-1);
                currentAmmo++;
                totalAmmo--;
                yield return new WaitForSeconds(reloadTime / magazineSize);
            }
            
            // Rotate back slowly to original position
            while (Quaternion.Angle(transform.localRotation, originalRotation) > 0.01f)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * 30);
                yield return null;
            }
            transform.localRotation = originalRotation;
            _isReloading = false;
            _reloadCoroutine = null;
        }
        
        private void DisplayAmmo()
        {
            float displayLenght = playerTransform.localScale.x;
            float bulletSize = Mathf.Max(
                (displayLenght + minDisplayGap) / magazineSize - displayLenght,
                maxBulletSize
            );
            
            float uidSize = bulletSize * 0.2f * magazineSize + minDisplayGap * (magazineSize - 1);
            
            GameObject magazineUid = new GameObject("magazineUID");
            magazineUid.transform.parent = playerTransform;
            for (int i = 0; i < magazineSize; i++)
            {
                var position = playerTransform.position;
                Vector3 bulletposition = new Vector3(
                    position.x - uidSize / magazineSize * i,
                    position.y + 1,
                    position.z
                );

                GameObject uidBullet = Instantiate(uidBulletPrefab, 
                    bulletposition,
                    Quaternion.identity
                    );
                
                uidBullet.transform.parent = magazineUid.transform;
                uidBullet.transform.localScale = new Vector3(bulletSize, bulletSize, 1);
                uidBullet.transform.Rotate(0, 0, 90);
                _ammoDisplay.Add(uidBullet);
            }
            magazineUid.transform.localPosition = new Vector3(uidSize/2 - bulletSize * 0.2f/2, 0, -2);
        }

        private void ToggleUidBullet(int i)
        {
            SpriteRenderer sp = _ammoDisplay[i].GetComponent<SpriteRenderer>();
            Color color = Color.black;
            color.a = 0.5f;
            sp.color = color;
        }
        
        private void EnableBullet(int i)
        {
            SpriteRenderer sp = _ammoDisplay[i].GetComponent<SpriteRenderer>();
            Color color = Color.white;
            color.a = 1f;
            sp.color = color;
        }
    }
}
