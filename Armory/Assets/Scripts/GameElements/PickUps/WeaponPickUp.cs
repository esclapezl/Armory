using System.Collections.Generic;
using Player.Inventory;
using UnityEngine;
using Utils;

namespace GameElements.PickUps
{
    [ExecuteInEditMode]
    public class WeaponPickUp : MonoBehaviour
    {
        public enum WeaponType
        {
            Pistol,
            Shotgun,
        }

        [System.Serializable]
        public struct WeaponSprite
        {
            public WeaponType weaponType;
            public Sprite sprite;
        }

        [SerializeField] private WeaponType weaponType;
        [SerializeField] private WeaponSprite[] ammoSprites;

        private Dictionary<WeaponType, Sprite> _weaponTypeToSprite;

        private void OnRenderObject()
        {
            _weaponTypeToSprite = new Dictionary<WeaponType, Sprite>();
            foreach (var ammoSprite in ammoSprites)
            {
                _weaponTypeToSprite[ammoSprite.weaponType] = ammoSprite.sprite;
            }

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _weaponTypeToSprite[weaponType];
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = spriteRenderer.sprite.bounds.size;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Inventory inventory = ObjectSearch.FindChildWithScript<Inventory>(other.transform);
                PickUp(inventory);
            }
        }

        void PickUp(Inventory inventory)
        {
            inventory.PickUpWeapon(weaponType);
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}