using System;
using System.Collections;
using Player;
using UnityEngine;
using weapons.HandGun;

namespace weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] public bool active = false;

        [NonSerialized] public Transform PlayerTransform;
        [NonSerialized] protected PlayerMovements PlayerMovements;
        [NonSerialized] protected Transform CannonTransform;

        [Header("Recoil Settings")]
        [SerializeField] [Range(0, 1)] public float recoil;
        [SerializeField] [Range(0, 100)] public float recoilMultiplier;
        protected Vector3 OriginalPosition;
        protected Vector3 RecoilPosition;
    
        [Header("Bullet Settings")]
        [SerializeField] public GameObject bulletPrefab;
        [SerializeField] [Range(0, 100)] public int magazineSize;
        [NonSerialized] public int CurrentAmmo;
        [SerializeField] public int TotalAmmo;
        [SerializeField] [Range(0, 10)] public float reloadTime;
        [NonSerialized] public bool IsReloading;
        [NonSerialized] protected AmmoDisplay AmmoDisplay;

        [Header("Fire Rate Settings")]
        [SerializeField] [Range(0, 1)] public float fireRate;
        [NonSerialized] protected float NextFireTime;
    
        protected Coroutine PlayerKnockBackCoroutine;
        protected Coroutine GunKnockBackCoroutine;
        protected Coroutine ReloadCoroutine;

        private void Awake()
        {
            PlayerTransform = transform.parent.parent;
            PlayerMovements = PlayerTransform.GetComponent<PlayerMovements>();
            CannonTransform = transform.GetChild(0);
            AmmoDisplay = GetComponent<AmmoDisplay>();
        }

        private void Start()
        {
            OriginalPosition = transform.localPosition;
            transform.localPosition = new Vector3(
                PlayerTransform.localScale.x,
                transform.localPosition.y,
                0
            );
            NextFireTime = 0f;
            CurrentAmmo = magazineSize;
        }
    
        void Update()
        {
            if (Input.GetButtonDown("Fire") && NextFireTime <= 0 && CurrentAmmo > 0 && !IsReloading) {
                Shoot();
            }

            if (((PlayerMovements.grounded && CurrentAmmo == 0) || 
                 (Input.GetButtonDown("Reload") && CurrentAmmo < magazineSize))
                && !IsReloading &&  TotalAmmo > 0)
            {
                if (ReloadCoroutine != null)
                {
                    StopCoroutine(ReloadCoroutine);
                }
                ReloadCoroutine = StartCoroutine(Reload());
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
            if (PlayerKnockBackCoroutine != null)
            {
                StopCoroutine(PlayerKnockBackCoroutine);
            }
            PlayerKnockBackCoroutine = StartCoroutine(PlayerKnockBack());
        
            if (GunKnockBackCoroutine != null)
            {
                StopCoroutine(GunKnockBackCoroutine);
            }
            GunKnockBackCoroutine = StartCoroutine(GunKnockBack());
        }
    
        private IEnumerator PlayerKnockBack()
        {
            Vector2 knockbackDirection = -CannonTransform.right;
            Rigidbody2D playerRigidbody = PlayerTransform.GetComponent<Rigidbody2D>();

            float currentRecoil = recoil * recoilMultiplier;
            playerRigidbody.velocity = Vector2.zero;
            float airFriction = PlayerMovements.airFriction;
            while (currentRecoil > 0)
            {
                playerRigidbody.AddForce(knockbackDirection * currentRecoil, ForceMode2D.Impulse);
                currentRecoil -= Time.fixedDeltaTime * airFriction; // Adjust the 10 to control how quickly the force decreases
                yield return null;
            }
            PlayerKnockBackCoroutine = null;
        }
    
        private IEnumerator GunKnockBack()
        {
            RecoilPosition = new Vector3(OriginalPosition.x - recoil, OriginalPosition.y, OriginalPosition.z);
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
