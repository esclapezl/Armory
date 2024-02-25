using System.Collections.Generic;
using UnityEngine;
using weapons;

namespace Player
{
    public class Inventory : MonoBehaviour
    {
        public List<GameObject> weapons = new List<GameObject>();
        private PlayerMovements _playerMovements;
        private int _currentWeapon;
        private float _mouseWheelValue;

        private void Awake()
        {
            _playerMovements = GetComponent<PlayerMovements>();
        }

        void Start()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject weaponChild = transform.GetChild(i).gameObject;
                weapons.Add(weaponChild);
                ToggleWeapon(weaponChild);
            }
            _currentWeapon = 0;
        }
        void Update()
        {
            _mouseWheelValue = Input.mouseScrollDelta.y;
        }

        private void FixedUpdate()
        {
            if (_mouseWheelValue != 0)
            {
                _currentWeapon += (int) _mouseWheelValue;
                _currentWeapon %= weapons.Count;
                ChangeWeapon(_currentWeapon);
            }
        }

        private void ChangeWeapon(int index)
        {
            GameObject currentWeapon = weapons[_currentWeapon];
            GameObject targetWeapon = weapons[index];
            ToggleWeapon(currentWeapon);
            ActivateWeapon(targetWeapon);
        }

        private void ToggleWeapon(GameObject weapon)
        {
            Weapon weaponInfo = weapon.GetComponent<Weapon>();
            weaponInfo.active = false;
            weaponInfo.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            if (weaponInfo.IsReloading)
            {
                // StopCoroutine(weaponInfo.ReloadCoroutine);
                weaponInfo.IsReloading = false;
                // weaponInfo.ReloadCoroutine = null;
            }
        }
    
        private void ActivateWeapon(GameObject weapon)
        {
            Weapon weaponDetails = weapon.GetComponent<Weapon>();
            weaponDetails.active = true;
            weapon.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
    
        private void PickUpWeapon(GameObject weapon)
        {
            weapons.Add(weapon);
            weapon.SetActive(false);
        }
    }
}
