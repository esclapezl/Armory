using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using weapons;
using Weapons.Pistol;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        public List<GameObject> weapons = new List<GameObject>();
        private PlayerMovements _playerMovements;
        private int _currentWeapon;
        private float _switchWeapon;
        private Transform _weaponsTransform;

        private void Awake()
        {
            _weaponsTransform = transform.GetChild(0);
            _playerMovements = GetComponent<PlayerMovements>();
        }

        void Start()
        {
            for (int i = 0; i < _weaponsTransform.childCount; i++)
            {
                GameObject weaponChild = _weaponsTransform.GetChild(i).gameObject;
                weapons.Add(weaponChild);
                weaponChild.GetComponent<AmmoDisplay>().SetDisplay();
                ToggleWeapon(weaponChild);
            }
            if (weapons.Count > 0)
            {
                _currentWeapon = 0;
                weapons[0].GetComponent<AmmoDisplay>().DisplayAmmo();
                ActivateWeapon(weapons[0]);
            }
        }
        void Update()
        {
            _switchWeapon = Input.GetButtonDown("SwitchWeaponUp") ? 1 : _switchWeapon;
            _switchWeapon = Input.GetButtonDown("SwitchWeaponDown") ? -1 : _switchWeapon;
        }

        private void FixedUpdate()
        {
            if (_switchWeapon != 0)
            {
                int targetWeapon = _currentWeapon + (int) _switchWeapon;
                targetWeapon %= weapons.Count; 
                targetWeapon = targetWeapon < 0 ? weapons.Count - 1 : targetWeapon;
                ChangeWeapon(targetWeapon);
            }
            _switchWeapon = 0;
        }

        private void ChangeWeapon(int index)
        {
            GameObject currentWeapon = weapons[_currentWeapon];
            GameObject targetWeapon = weapons[index];
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
    
        private void PickUpWeapon(GameObject weapon)
        {
            weapons.Add(weapon);
            weapon.SetActive(false);
        }
    }
}
