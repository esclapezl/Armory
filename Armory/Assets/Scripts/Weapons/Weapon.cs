using System;
using System.Collections;
using Camera;
using GameElements.PickUps;
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
        [SerializeField] [Range(0, 10)] public float reloadTime;
        [SerializeField] [Range(0, 20)] public float bulletSpeed;
        [NonSerialized] public bool IsReloading;
        [NonSerialized] public AmmoDisplay AmmoDisplay;

        [Header("Fire Rate Settings")] [SerializeField] [Range(0, 1)]
        public float fireRate;

        [NonSerialized] protected float NextFireTime;
        [NonSerialized] protected Inventory _inventory;

        [NonSerialized] protected Coroutine GunKnockBackCoroutine;
        [NonSerialized] public Coroutine ReloadCoroutine;
        
        [NonSerialized] protected CameraShake CameraShake;

        [SerializeField] public Sprite AmmoSprite;

        private void Awake()
        {
            PlayerTransform = transform.parent.parent.parent;
            Player = PlayerTransform.GetComponent<Player.Player>();
            CannonTransform = transform.GetChild(0);
            AmmoDisplay = GetComponent<AmmoDisplay>();
            _inventory = ObjectSearch.FindParentWithScript<Inventory>(transform);
            CameraShake = ObjectSearch.FindRoot("Main Camera").GetComponent<CameraShake>();
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
            if (!Player.Dead && active && !IsReloading)
            {
                if (Input.GetButtonDown("Fire") && NextFireTime <= 0 && currentAmmo > 0)
                {
                    Shoot();
                }

                if ((currentAmmo == 0 || Input.GetButtonDown("Reload"))
                    && currentAmmo < magazineSize
                    && _inventory.GetAmmoForWeapon(transform.name).ammo > 0
                    && Player.PlayerJump.Grounded)
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

            GunKnockBackCoroutine = null;
        }

        protected void Shoot()
        {
            StartCoroutine(CameraShake.Shake(0.1f, playerRecoilForce/100));
            Collider2D hitCollider =
                Physics2D.OverlapCircle(CannonTransform.position, 0.1f, Player.PlayerJump.whatIsGround);
            if (hitCollider == null)
            {
                ShootingMethod();
            }

            NextFireTime = fireRate;
            AmmoDisplay.ToggleUidBullet(magazineSize - currentAmmo);
            currentAmmo--;
            _inventory.InventoryUid.RefreshInventoryUid();

            KnockBack();
        }

        protected virtual void ShootingMethod()
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
    }
}