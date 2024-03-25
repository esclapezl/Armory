using System;
using System.Collections.Generic;
using GameElements.PickUps;
using Player.Controls;
using UnityEngine;
using UnityEngine.Serialization;
using weapons;
using Weapons;
using ObjectSearch = Utils.ObjectSearch;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] public GameObject[] availableWeapons;
        [NonSerialized] public List<GameObject> ActiveWeapons = new List<GameObject>();
        [Serializable] public class WeaponAmmo
        {
            public string weaponName;
            public int ammo;
        }

        [NonSerialized] public WeaponAmmo[] AmmoStock;

        [NonSerialized] public int CurrentWeapon;
        [NonSerialized] private float _switchWeapon;
        [NonSerialized] private Transform _weaponsTransform;
        [NonSerialized] private PlayerMovements _playerMovements;
        [NonSerialized] public InventoryUid InventoryUid;

        private void Awake()
        {
            AmmoStock = new WeaponAmmo[availableWeapons.Length];
            int i = 0;
            foreach (GameObject weapon in availableWeapons)
            {
                WeaponAmmo weaponAmmo = new WeaponAmmo();
                weaponAmmo.weaponName = weapon.name;
                weaponAmmo.ammo = 0;
                AmmoStock[i] = weaponAmmo;
                i++;
            }
            
            InventoryUid = ObjectSearch.FindChild(UnityEngine.Camera.main!.transform, "InventoryUID")
                .GetComponent<InventoryUid>();
            _playerMovements = ObjectSearch.FindParentWithScript<PlayerMovements>(transform);
            _weaponsTransform = ObjectSearch.FindChild(transform, "Weapons");
        }

        void Update()
        {
            _switchWeapon = Input.GetButtonDown("SwitchWeaponUp") ? 1 : _switchWeapon;
            _switchWeapon = Input.GetButtonDown("SwitchWeaponDown") ? -1 : _switchWeapon;
        }

        private void FixedUpdate()
        {
            if (ActiveWeapons.Count > 0 && _switchWeapon != 0)
            {
                int targetWeapon = CurrentWeapon + (int)_switchWeapon;
                targetWeapon %= ActiveWeapons.Count;
                targetWeapon = targetWeapon < 0 ? ActiveWeapons.Count - 1 : targetWeapon;
                ChangeWeapon(targetWeapon);
            }

            _switchWeapon = 0;
        }

        private void ChangeWeapon(int index)
        {
            InventoryUid.HighlightSlot(index);
            GameObject currentWeapon = ActiveWeapons[CurrentWeapon];
            GameObject targetWeapon = ActiveWeapons[index];
            ToggleWeapon(currentWeapon);
            ActivateWeapon(targetWeapon);
            CurrentWeapon = index;
        }

        private void ToggleWeapon(GameObject weapon)
        {
            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.active = false;
            weapon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            weapon.GetComponent<AmmoDisplay>().HideAmmo();
            if (weaponInfo.IsReloading)
            {
                weaponInfo.StopCoroutine(weaponInfo.ReloadCoroutine);
                weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                weaponInfo.IsReloading = false;
                weaponInfo.ReloadCoroutine = null;
            }
        }

        private void ActivateWeapon(GameObject weapon)
        {
            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.active = true;
            weapon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            weapon.GetComponent<AmmoDisplay>().DisplayAmmo();
        }

        public void PickUpWeapon(WeaponPickUp.WeaponType weaponPickup)
        {
            string weaponScriptName = weaponPickup.ToString();
            foreach (GameObject availableWeapon in availableWeapons)
            {
                if (availableWeapon.name == weaponScriptName)
                {
                    AddWeapon(availableWeapon, 0);
                    RefreshInventory();
                    return;
                }
            }
            
            throw new Exception("Weapon not found in armory");
        }

        public void Clear()
        {
            foreach (GameObject weapon in ActiveWeapons)
            {
                weapon.GetComponent<AmmoDisplay>().HideAmmo();
                ToggleWeapon(weapon);
                weapon.SetActive(false);
            }

            ActiveWeapons.Clear();
        }

        public void AddWeapon(GameObject weaponPrefab, int ammo)
        {
            GameObject weapon = Instantiate(weaponPrefab, _weaponsTransform.position, Quaternion.identity, _weaponsTransform);
            weapon.name = weaponPrefab.name;
            weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
            weapon.transform.parent = _weaponsTransform;
            ActiveWeapons.Add(weapon);
            weapon.SetActive(true);
            ToggleWeapon(weapon);
            
            GetAmmoForWeapon(weaponPrefab.name).ammo += ammo;

            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.GetComponent<AmmoDisplay>().SetDisplay();
            weaponInfo.currentAmmo = Mathf.Min(weaponInfo.magazineSize, ammo);
        }

        public void RefreshInventory()
        {
            foreach (GameObject weapon in ActiveWeapons)
            {
                weapon.SetActive(true);
                ToggleWeapon(weapon);
            }
            
            if (ActiveWeapons.Count > 0)
            {
                CurrentWeapon = 0;
                ActiveWeapons[0].GetComponent<AmmoDisplay>().DisplayAmmo();
                ActivateWeapon(ActiveWeapons[0]);
                _playerMovements.Armed = true;
                InventoryUid.StartInventoryUid();
            }
            else
            {
                _playerMovements.Armed = false;
            }
        }

        public WeaponAmmo GetAmmoForWeapon(string weaponName)
        {
            foreach (WeaponAmmo weaponAmmo in AmmoStock)
            {
                if (weaponAmmo.weaponName == weaponName)
                {
                    return weaponAmmo;
                }
            }
            throw new Exception("Weapon not found in armory");
        }
        
        public void PickUpAmmo(Ammo.AmmoType ammoType)
        {
            GameObject activeWeapon = ActiveWeapons[CurrentWeapon];
            string ammoName = ammoType.ToString();
            
            if (activeWeapon.name == ammoName)
            {
                Weapon weapon = ActiveWeapons[CurrentWeapon].GetComponent<Weapon>();
                if (weapon.active && weapon.currentAmmo < weapon.magazineSize)
                {
                    weapon.currentAmmo++;
                    weapon.AmmoDisplay.DisplayAmmo();
                }
                else
                {
                    StoreAmmo(ammoName);
                }
            }
            else
            {
                StoreAmmo(ammoName);
            }
        }

        private void StoreAmmo(string ammoName)
        {
            WeaponAmmo weaponAmmo = GetAmmoForWeapon(ammoName);
            weaponAmmo.ammo++;
            InventoryUid.RefreshInventoryUid();
        }
    }
}