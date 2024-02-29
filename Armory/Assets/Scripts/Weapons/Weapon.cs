using System;
using System.Collections;
using DefaultNamespace;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons.Pistol;

namespace weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] public bool active = false;

        [NonSerialized] public Transform playerTransform;
        [NonSerialized] protected PlayerMovements playerMovements;
        [NonSerialized] protected Transform cannonTransform;
        
        [Header("Recoil Settings")]
        [SerializeField] [Range(0, 2)] public float gunRecoil;
        [SerializeField] [Range(0, 100)] public float playerRecoilForce;
        [SerializeField] [Range(1, 10)] public float playerRecoilBoostWhileEmbracingRecoil;
        [SerializeField] [Range(0, 100)] public float playerRecoilDuration;
        protected Vector3 originalPosition;
        protected Vector3 recoilPosition;
    
        [Header("Bullet Settings")]
        [SerializeField] public GameObject bulletPrefab;
        [SerializeField] [Range(0, 100)] public int magazineSize;
        [SerializeField] public int currentAmmo;
        [SerializeField] public int totalAmmo;
        [SerializeField] [Range(0, 10)] public float reloadTime;
        [SerializeField] [Range(0, 20)] public float bulletSpeed;
        [NonSerialized] public bool isReloading;
        [NonSerialized] public AmmoDisplay ammoDisplay;

        [Header("Fire Rate Settings")]
        [SerializeField] [Range(0, 1)] public float fireRate;
        [NonSerialized] protected float nextFireTime;
        
        protected Coroutine gunKnockBackCoroutine;
        public Coroutine reloadCoroutine;

        private void Awake()
        {
            playerTransform = transform.parent.parent.parent;
            playerMovements = playerTransform.GetComponent<PlayerMovements>();
            cannonTransform = transform.GetChild(0);
            ammoDisplay = GetComponent<AmmoDisplay>();
        }

        private void Start()
        {
            float gunSize = GetComponent<SpriteRenderer>().bounds.size.x;
            float margin = 0.1f;
            transform.localPosition = new Vector3(
                playerTransform.localScale.x / 2 + gunSize / 2 + margin,
                transform.localPosition.y,
                0
            );
            originalPosition = transform.localPosition;
            nextFireTime = 0f;
            currentAmmo = Mathf.Min(magazineSize, totalAmmo);
        }
    
        void Update()
        {
            if (active && !isReloading)
            {
                if (Input.GetButtonDown("Fire") && nextFireTime <= 0 && currentAmmo > 0) {
                    Shoot();
                }
                
                if ((currentAmmo == 0 || Input.GetButtonDown("Reload"))
                    && currentAmmo < magazineSize
                    && totalAmmo > 0 
                    && playerMovements.Grounded)
                {
                    if (reloadCoroutine != null)
                    {
                        StopCoroutine(reloadCoroutine);
                    }
                    reloadCoroutine = StartCoroutine(Reload());
                }
            }
        }
    
        private void FixedUpdate()
        {
            if (nextFireTime > 0)
            {
                nextFireTime -= Time.deltaTime;
            }
        }

        public void KnockBack()
        {
            PlayerKnockBack();
            if (gunKnockBackCoroutine != null)
            {
                StopCoroutine(gunKnockBackCoroutine);
            }
            gunKnockBackCoroutine = StartCoroutine(GunKnockBack());
        }
    
        private void PlayerKnockBack()
        {
            Vector3 knockbackVector = -cannonTransform.right;
            Rigidbody2D playerRigidbody = playerTransform.GetComponent<Rigidbody2D>();
            float appliedForce = playerRecoilForce;
            if (playerMovements.Crouching)
            {
                appliedForce *= 0.75f;
            }

            playerRigidbody.velocity = Vector2.zero;
            string direction = (Utils.AngleToDirection(cannonTransform.eulerAngles.z, 45));
            playerMovements.ShotDirection = direction;
            if (playerMovements.HorizontalInput == 0)
            {
                playerMovements.PreviousHorizontalInput = 0;
            }
            else if((direction == "left" && playerMovements.HorizontalInput == 1) 
                    || (direction == "right" && playerMovements.HorizontalInput == -1))
                //octroie un boost horizontal dans le knockback si le joueur va dans la direction de sa vélocité knockback
            {
                knockbackVector = new Vector3(knockbackVector.x * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.y,
                    knockbackVector.z);
                StartCoroutine(playerMovements.BoostTrail(5));
            }
            
            if (direction == "down" && playerMovements.CanJumpBoost)
                //octroie un boost vertical dans le knockback si le joueur saute et tire vers le bas
            {
                knockbackVector = new Vector3(knockbackVector.x,
                    knockbackVector.y * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.z);
                StartCoroutine(playerMovements.BoostTrail(5));
            }

            playerMovements.CanJumpBoost = false;
            playerRigidbody.AddForce(knockbackVector * appliedForce, ForceMode2D.Impulse);
        }
    
        private IEnumerator GunKnockBack()
        {
            recoilPosition = new Vector3(originalPosition.x - gunRecoil, originalPosition.y, originalPosition.z);
            transform.localPosition = recoilPosition;
            while (Vector3.Distance(transform.localPosition, originalPosition) > 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5);
                yield return null;
            }
            gunKnockBackCoroutine = null;
        }

        protected virtual void Shoot()
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerator Reload()
        {
            throw new NotImplementedException();
        }
    }
}
