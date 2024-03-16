using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;
using Weapons.Pistol;
using Angles = Utils.Angles;
using UnityEngine_Transform = UnityEngine.Transform;

namespace weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] public bool active = false;

        [NonSerialized] public UnityEngine_Transform playerTransform;
        [NonSerialized] protected Player.Player player;
        [NonSerialized] protected Transform cannonTransform;

        [Header("Recoil Settings")] [SerializeField] [Range(0, 2)]
        public float gunRecoil;

        [SerializeField] [Range(0, 100)] public float playerRecoilForce;
        [SerializeField] [Range(1, 10)] public float playerRecoilBoostWhileEmbracingRecoil;
        [SerializeField] [Range(0, 100)] public float playerRecoilDuration;
        protected Vector3 originalPosition;
        protected Vector3 recoilPosition;

        [Header("Bullet Settings")] [SerializeField]
        public GameObject bulletPrefab;

        [SerializeField] [Range(0, 100)] public int magazineSize;
        [SerializeField] public int currentAmmo;
        [SerializeField] public int totalAmmo;
        [SerializeField] [Range(0, 10)] public float reloadTime;
        [SerializeField] [Range(0, 20)] public float bulletSpeed;
        [NonSerialized] public bool isReloading;
        [NonSerialized] public AmmoDisplay ammoDisplay;

        [Header("Fire Rate Settings")] [SerializeField] [Range(0, 1)]
        public float fireRate;

        [NonSerialized] protected float nextFireTime;

        protected Coroutine gunKnockBackCoroutine;
        public Coroutine reloadCoroutine;

        private void Awake()
        {
            playerTransform = transform.parent.parent.parent;
            player = playerTransform.GetComponent<Player.Player>();
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
        }

        void Update()
        {
            if (!player.dead && active && !isReloading)
            {
                if (Input.GetButtonDown("Fire") && nextFireTime <= 0 && currentAmmo > 0)
                {
                    Shoot();
                }

                if ((currentAmmo == 0 || Input.GetButtonDown("Reload"))
                    && currentAmmo < magazineSize
                    && totalAmmo > 0
                    && player.PlayerJump.Grounded)
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
            float appliedForce = playerRecoilForce;
            player.PlayerKnockback.PlayerKnockBack(cannonTransform, appliedForce,
                playerRecoilBoostWhileEmbracingRecoil);
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

        public void Enable()
        {
            ammoDisplay.DisplayAmmo();
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            ammoDisplay.HideAmmo();
            gameObject.SetActive(false);
        }

        public void PickUpAmmo(Ammo.AmmoType ammoType)
        {
            if (currentAmmo < magazineSize)
            {
                currentAmmo++;
                ammoDisplay.DisplayAmmo();
            }
            else
            {
                totalAmmo++;
            }
        }
    }
}