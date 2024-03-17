using System;
using System.Collections;
using Player;
using Player.Inventory;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Weapons;
using Weapons.Pistol;
using Angles = Utils.Angles;
using UnityEngine_Transform = UnityEngine.Transform;

namespace weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] public bool active = false;

        [NonSerialized] public UnityEngine_Transform PlayerTransform;
        [NonSerialized] protected Player.Player Player;
        [NonSerialized] protected Transform CannonTransform;

        [Header("Recoil Settings")] [SerializeField] [Range(0, 2)]
        public float gunRecoil;

        [SerializeField] [Range(0, 100)] public float playerRecoilForce;
        [SerializeField] [Range(1, 10)] public float playerRecoilBoostWhileEmbracingRecoil;
        [SerializeField] [Range(0, 100)] public float playerRecoilDuration;
        protected Vector3 OriginalPosition;
        protected Vector3 RecoilPosition;

        [Header("Bullet Settings")] [SerializeField]
        public GameObject bulletPrefab;

        [SerializeField] [Range(0, 100)] public int magazineSize;
        [SerializeField] public int currentAmmo;
        [SerializeField] public int totalAmmo;
        [SerializeField] [Range(0, 10)] public float reloadTime;
        [SerializeField] [Range(0, 20)] public float bulletSpeed;
        [NonSerialized] public bool IsReloading;
        [NonSerialized] public AmmoDisplay AmmoDisplay;

        [Header("Fire Rate Settings")] [SerializeField] [Range(0, 1)]
        public float fireRate;

        [NonSerialized] protected float NextFireTime;
        [NonSerialized] private Inventory _inventory;

        protected Coroutine gunKnockBackCoroutine;
        public Coroutine reloadCoroutine;

        private void Awake()
        {
            PlayerTransform = transform.parent.parent.parent;
            Player = PlayerTransform.GetComponent<Player.Player>();
            CannonTransform = transform.GetChild(0);
            AmmoDisplay = GetComponent<AmmoDisplay>();
            _inventory = ObjectSearch.FindParentWithScript<Inventory>(transform);
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
        }

        void Update()
        {
            if (!Player.dead && active && !IsReloading)
            {
                if (Input.GetButtonDown("Fire") && NextFireTime <= 0 && currentAmmo > 0)
                {
                    Shoot();
                }

                if ((currentAmmo == 0 || Input.GetButtonDown("Reload"))
                    && currentAmmo < magazineSize
                    && totalAmmo > 0
                    && Player.PlayerJump.Grounded)
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
            if (NextFireTime > 0)
            {
                NextFireTime -= Time.deltaTime;
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
            Player.PlayerKnockback.PlayerKnockBack(CannonTransform, appliedForce,
                playerRecoilBoostWhileEmbracingRecoil);
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
            AmmoDisplay.DisplayAmmo();
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            AmmoDisplay.HideAmmo();
            gameObject.SetActive(false);
        }

        public void PickUpAmmo(Ammo.AmmoType ammoType)
        {
            if (active && currentAmmo < magazineSize)
            {
                currentAmmo++;
                AmmoDisplay.DisplayAmmo();
            }
            else
            {
                totalAmmo++;
            }
        }
    }
}