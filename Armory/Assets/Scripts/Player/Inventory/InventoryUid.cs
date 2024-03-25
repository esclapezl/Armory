using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using weapons;

namespace Player.Inventory
{
    public class InventoryUid : MonoBehaviour
    {
        [NonSerialized] private global::Player.Inventory.Inventory _inventory;
        [NonSerialized] private List<InventorySlotUid> _inventorySlots;
        [SerializeField] private GameObject weaponSlotPrefab;
        [NonSerialized] private UnityEngine.Camera _mainCamera;
        [SerializeField] private float uidMargin;

        private void Awake()
        {
            _mainCamera = UnityEngine.Camera.main;
            _inventory = ObjectSearch.FindChildWithScript<global::Player.Inventory.Inventory>(ObjectSearch.FindRoot("Player"));
            _inventorySlots = new List<InventorySlotUid>();

            Vector3 margin = new Vector3(uidMargin, uidMargin, 0); // Marge en unités de monde
            Vector3 marginInPixels = _mainCamera!.WorldToScreenPoint(_mainCamera.transform.position + margin) -
                                     _mainCamera.WorldToScreenPoint(_mainCamera.transform.position);

            Vector3 cameraPosition =
                _mainCamera.ScreenToWorldPoint(new Vector3(marginInPixels.x, marginInPixels.y,
                    _mainCamera.nearClipPlane));
            transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
        }

        public void ClearInventoryUid()
        {
            foreach (InventorySlotUid slot in _inventorySlots)
            {
                Destroy(slot.gameObject);
            }

            _inventorySlots.Clear();
        }

        public void RefreshInventoryUid()
        {
            ClearInventoryUid();
            int index = 0;
            foreach (GameObject weaponObject in _inventory.ActiveWeapons)
            {
                CreateSlot(index).SetSlot(weaponObject.GetComponent<SpriteRenderer>().sprite,
                    _inventory.GetAmmoForWeapon(weaponObject.name).ammo ,
                    weaponObject.GetComponent<Weapon>().AmmoSprite);
                index++;
            }

            if (_inventory.ActiveWeapons.Count > 0)
            {
                //creates empty slot
                CreateSlot(index);
                HighlightSlot(0);
            }
        }

        private InventorySlotUid CreateSlot(int index)
        {
            GameObject weaponSlot = Instantiate(weaponSlotPrefab, transform);
            weaponSlot.name = index + "_WeaponSlot";
            InventorySlotUid inventorySlot = weaponSlot.GetComponent<InventorySlotUid>();
            inventorySlot.transform.localPosition = new Vector3(index, 0, 0);
            _inventorySlots.Add(inventorySlot);
            return inventorySlot;
        }

        public void HighlightSlot(int index)
        {
            foreach (InventorySlotUid slot in _inventorySlots)
            {
                slot.Unhighlight();
            }

            _inventorySlots[index].Highlight();
        }
    }
}