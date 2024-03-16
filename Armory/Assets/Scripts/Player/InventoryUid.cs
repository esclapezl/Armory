using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using weapons;

namespace Player
{
    public class InventoryUid : MonoBehaviour
    {
        [NonSerialized] private Inventory _inventory;
        [NonSerialized] private List<InventorySlotUid> _inventorySlots;
        [SerializeField] private GameObject weaponSlotPrefab;
        [NonSerialized] private UnityEngine.Camera _mainCamera;
        [SerializeField] private float uidMargin;

        private void Awake()
        {
            _mainCamera = UnityEngine.Camera.main;
            _inventory = ObjectSearch.FindChildWithScript<Inventory>(ObjectSearch.FindRoot("Player"));
            _inventorySlots = new List<InventorySlotUid>();
            
            Vector3 margin = new Vector3(uidMargin, uidMargin, 0); // Marge en unités de monde
            Vector3 marginInPixels = _mainCamera!.WorldToScreenPoint(_mainCamera.transform.position + margin) - _mainCamera.WorldToScreenPoint(_mainCamera.transform.position);

            Vector3 cameraPosition = _mainCamera.ScreenToWorldPoint(new Vector3(marginInPixels.x, marginInPixels.y, _mainCamera.nearClipPlane));
            transform.position = new Vector3(cameraPosition.x, cameraPosition.y, transform.position.z);
        }

        public void ClearInventoryUID()
        {
            foreach (InventorySlotUid slot in _inventorySlots)
            {
                Destroy(slot.gameObject);
            }
            _inventorySlots.Clear();
        }

        public void RefreshInventoryUid()
        {
            ClearInventoryUID();
            int index = 0;
            foreach (GameObject weaponObject in _inventory.activeWeapons)
            {
                GameObject weaponSlot = Instantiate(weaponSlotPrefab, transform);
                InventorySlotUid inventorySlot = weaponSlot.GetComponent<InventorySlotUid>();
                inventorySlot.transform.localPosition = new Vector3(index, 0, 0);
                inventorySlot.SetSlot(weaponObject.GetComponent<SpriteRenderer>().sprite, weaponObject.GetComponent<Weapon>().totalAmmo);
                _inventorySlots.Add(inventorySlot);
                index++;
            }

            if (_inventory.activeWeapons.Count > 0)
            {
                //creates empty slot
                GameObject weaponSlot = Instantiate(weaponSlotPrefab, transform);
                InventorySlotUid inventorySlot = weaponSlot.GetComponent<InventorySlotUid>();
                inventorySlot.transform.localPosition = new Vector3(index, 0, 0);
                _inventorySlots.Add(inventorySlot);
                HighlightSlot(0);
            }
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
