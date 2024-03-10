using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using weapons;
using Weapons.Pistol;
using ObjectSearch = Utils.ObjectSearch;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        public List<GameObject> activeWeapons = new List<GameObject>();
        public List<GameObject> availableWeapons = new List<GameObject>();
        
        private int _currentWeapon;
        private float _switchWeapon;
        private Transform _weaponsTransform;
        private PlayerMovements _playerMovements;

        private void Awake()
        {
            _playerMovements = ObjectSearch.FindParentWithScript<PlayerMovements>(transform);
            _weaponsTransform = ObjectSearch.FindChild(transform, "Weapons");
            for (int i = 0; i < _weaponsTransform.childCount; i++)
            {
                GameObject weaponChild = _weaponsTransform.GetChild(i).gameObject;
                availableWeapons.Add(weaponChild);
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
            if (activeWeapons.Count > 0 && _switchWeapon != 0)
            {
                int targetWeapon = _currentWeapon + (int) _switchWeapon;
                targetWeapon %= activeWeapons.Count; 
                targetWeapon = targetWeapon < 0 ? activeWeapons.Count - 1 : targetWeapon;
                ChangeWeapon(targetWeapon);
            }
            _switchWeapon = 0;
        }

        private void ChangeWeapon(int index)
        {
            GameObject currentWeapon = activeWeapons[_currentWeapon];
            GameObject targetWeapon = activeWeapons[index];
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
            if (weaponInfo.isReloading)
            {
                weaponInfo.StopCoroutine(weaponInfo.reloadCoroutine);
                weapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
                weaponInfo.isReloading = false;
                weaponInfo.reloadCoroutine = null;
            }
        }
    
        private void ActivateWeapon(GameObject weapon)
        {
            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.active = true;
            weapon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            weapon.GetComponent<AmmoDisplay>().DisplayAmmo();
        }
    
        private void PickUpWeapon(GameObject weapon)
        {
            activeWeapons.Add(weapon);
            weapon.SetActive(false);
        }

        public void Clear()
        {
            foreach (GameObject weapon in availableWeapons)
            {
                weapon.GetComponent<AmmoDisplay>().HideAmmo();
                ToggleWeapon(weapon);
                weapon.SetActive(false);
            }
            activeWeapons.Clear();
        }
        
        public void AddWeapon(GameObject weapon, int ammo)
        {
            activeWeapons.Add(weapon);
            weapon.SetActive(true);
            ToggleWeapon(weapon);
            
            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.currentAmmo = Mathf.Min(weaponInfo.magazineSize, ammo);
            weaponInfo.totalAmmo = ammo - weaponInfo.currentAmmo;
        }
        
        public void SetUpInventory()
        {
            foreach (GameObject weapon in activeWeapons)
            {
                weapon.SetActive(true);
                ToggleWeapon(weapon);
            }
            if (activeWeapons.Count > 0)
            {
                _currentWeapon = 0;
                activeWeapons[0].GetComponent<AmmoDisplay>().DisplayAmmo();
                ActivateWeapon(activeWeapons[0]);
                _playerMovements.Armed = true;
            }
            else
            {
                _playerMovements.Armed = false;
            }
        }
    }
}
