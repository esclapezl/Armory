using System;
using System.Collections.Generic;
using GameElements.PickUps;
using Player.Controls;
using UnityEngine;
using weapons;
using Weapons;
using ObjectSearch = Utils.ObjectSearch;

namespace Player.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [NonSerialized] public List<GameObject> ActiveWeapons = new List<GameObject>();
        [NonSerialized] public List<GameObject> AvailableWeapons = new List<GameObject>();

        [NonSerialized] private int _currentWeapon;
        [NonSerialized] private float _switchWeapon;
        [NonSerialized] private Transform _weaponsTransform;
        [NonSerialized] private PlayerMovements _playerMovements;
        [NonSerialized] private InventoryUid _inventoryUid;

        private void Awake()
        {
            _inventoryUid = ObjectSearch.FindChild(UnityEngine.Camera.main!.transform, "InventoryUID")
                .GetComponent<InventoryUid>();
            _playerMovements = ObjectSearch.FindParentWithScript<PlayerMovements>(transform);
            _weaponsTransform = ObjectSearch.FindChild(transform, "Weapons");
            for (int i = 0; i < _weaponsTransform.childCount; i++)
            {
                GameObject weaponChild = _weaponsTransform.GetChild(i).gameObject;
                AvailableWeapons.Add(weaponChild);
                weaponChild.GetComponent<AmmoDisplay>().SetDisplay();
            }
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
                int targetWeapon = _currentWeapon + (int)_switchWeapon;
                targetWeapon %= ActiveWeapons.Count;
                targetWeapon = targetWeapon < 0 ? ActiveWeapons.Count - 1 : targetWeapon;
                ChangeWeapon(targetWeapon);
            }

            _switchWeapon = 0;
        }

        private void ChangeWeapon(int index)
        {
            _inventoryUid.HighlightSlot(index);
            GameObject currentWeapon = ActiveWeapons[_currentWeapon];
            GameObject targetWeapon = ActiveWeapons[index];
            ToggleWeapon(currentWeapon);
            ActivateWeapon(targetWeapon);
            _currentWeapon = index;
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
            GameObject weapon = ObjectSearch.FindChild(_weaponsTransform, weaponPickup.ToString()).gameObject;
            ActiveWeapons.Add(weapon);
            RefreshInventory();
        }

        public void Clear()
        {
            foreach (GameObject weapon in AvailableWeapons)
            {
                weapon.GetComponent<AmmoDisplay>().HideAmmo();
                ToggleWeapon(weapon);
                weapon.SetActive(false);
            }

            ActiveWeapons.Clear();
        }

        public void AddWeapon(GameObject weapon, int ammo)
        {
            ActiveWeapons.Add(weapon);
            weapon.SetActive(true);
            ToggleWeapon(weapon);

            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.currentAmmo = Mathf.Min(weaponInfo.magazineSize, ammo);
            weaponInfo.totalAmmo = ammo - weaponInfo.currentAmmo;
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
                _currentWeapon = 0;
                ActiveWeapons[0].GetComponent<AmmoDisplay>().DisplayAmmo();
                ActivateWeapon(ActiveWeapons[0]);
                _playerMovements.Armed = true;
                _inventoryUid.RefreshInventoryUid();
            }
            else
            {
                _playerMovements.Armed = false;
            }
        }
    }
}