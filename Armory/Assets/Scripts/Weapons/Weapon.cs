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

        [NonSerialized] public Transform PlayerTransform;
        [NonSerialized] protected PlayerMovements PlayerMovements;
        [NonSerialized] protected Transform CannonTransform;
        
        [Header("Recoil Settings")]
        [SerializeField] [Range(0, 2)] public float gunRecoil;
        [SerializeField] [Range(0, 100)] public float playerRecoilForce;
        [SerializeField] [Range(1, 10)] public float playerRecoilBoostWhileEmbracingRecoil;
        [SerializeField] [Range(0, 100)] public float playerRecoilDuration;
        protected Vector3 OriginalPosition;
        protected Vector3 RecoilPosition;
    
        [Header("Bullet Settings")]
        [SerializeField] public GameObject bulletPrefab;
        [SerializeField] [Range(0, 100)] public int magazineSize;
        [SerializeField] public int currentAmmo;
        [SerializeField] public int totalAmmo;
        [SerializeField] [Range(0, 10)] public float reloadTime;
        [SerializeField] [Range(0, 20)] public float bulletSpeed;
        [NonSerialized] public bool IsReloading;
        [NonSerialized] protected AmmoDisplay AmmoDisplay;

        [Header("Fire Rate Settings")]
        [SerializeField] [Range(0, 1)] public float fireRate;
        [NonSerialized] protected float NextFireTime;
        
        protected Coroutine GunKnockBackCoroutine;
        public Coroutine ReloadCoroutine;

        private void Awake()
        {
            PlayerTransform = transform.parent.parent.parent;
            PlayerMovements = PlayerTransform.GetComponent<PlayerMovements>();
            CannonTransform = transform.GetChild(0);
            AmmoDisplay = GetComponent<AmmoDisplay>();
        }

        private void Start()
        {
            float gunSize = GetComponent<SpriteRenderer>().bounds.size.x;
            float margin = 0.1f;
            transform.localPosition = new Vector3(
                PlayerTransform.localScale.x / 2 + gunSize / 2 + margin,
                transform.localPosition.y,
                0
            );
            OriginalPosition = transform.localPosition;
            NextFireTime = 0f;
            currentAmmo = Mathf.Min(magazineSize, totalAmmo);
        }
    
        void Update()
        {
            if (active && !IsReloading)
            {
                if (Input.GetButtonDown("Fire") && NextFireTime <= 0 && currentAmmo > 0) {
                    Shoot();
                }
                
                if ((currentAmmo == 0 || Input.GetButtonDown("Reload"))
                    && currentAmmo < magazineSize
                    && totalAmmo > 0 
                    && PlayerMovements.Grounded)
                {
                    if (ReloadCoroutine != null)
                    {
                        StopCoroutine(ReloadCoroutine);
                    }
                    ReloadCoroutine = StartCoroutine(Reload());
                }
            }
        }
    
        private void FixedUpdate()
        {
            if (NextFireTime > 0)
            {
                NextFireTime -= Time.deltaTime;
            }
        }

        public void KnockBack()
        {
            PlayerKnockBack();
            if (GunKnockBackCoroutine != null)
            {
                StopCoroutine(GunKnockBackCoroutine);
            }
            GunKnockBackCoroutine = StartCoroutine(GunKnockBack());
        }
    
        private void PlayerKnockBack()
        {
            Vector3 knockbackVector = -CannonTransform.right;
            Rigidbody2D playerRigidbody = PlayerTransform.GetComponent<Rigidbody2D>();
            float appliedForce = playerRecoilForce;
            if (PlayerMovements.Crouching)
            {
                appliedForce *= 0.75f;
            }

            playerRigidbody.velocity = Vector2.zero;
            string direction = (Utils.AngleToDirection(CannonTransform.eulerAngles.z, 45));
            PlayerMovements.ShotDirection = direction;
            if (PlayerMovements.HorizontalInput == 0)
            {
                PlayerMovements.PreviousHorizontalInput = 0;
            }
            else if((direction == "left" && PlayerMovements.HorizontalInput == 1) 
                    || (direction == "right" && PlayerMovements.HorizontalInput == -1))
                //octroie un boost horizontal dans le knockback si le joueur va dans la direction de sa vélocité knockback
            {
                knockbackVector = new Vector3(knockbackVector.x * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.y,
                    knockbackVector.z);
                StartCoroutine(PlayerMovements.BoostTrail(5));
            }
            
            if (direction == "down" && PlayerMovements.CanJumpBoost)
                //octroie un boost vertical dans le knockback si le joueur saute et tire vers le bas
            {
                knockbackVector = new Vector3(knockbackVector.x,
                    knockbackVector.y * playerRecoilBoostWhileEmbracingRecoil,
                    knockbackVector.z);
                StartCoroutine(PlayerMovements.BoostTrail(5));
            }

            PlayerMovements.CanJumpBoost = false;
            playerRigidbody.AddForce(knockbackVector * appliedForce, ForceMode2D.Impulse);
        }
    
        private IEnumerator GunKnockBack()
        {
            RecoilPosition = new Vector3(OriginalPosition.x - gunRecoil, OriginalPosition.y, OriginalPosition.z);
            transform.localPosition = RecoilPosition;
            while (Vector3.Distance(transform.localPosition, OriginalPosition) > 0.01f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5);
                yield return null;
            }
            GunKnockBackCoroutine = null;
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
