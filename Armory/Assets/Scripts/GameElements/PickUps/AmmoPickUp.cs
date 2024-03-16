using System.Collections.Generic;
using UnityEngine;
using Utils;
using weapons;

namespace Weapons
{
    [ExecuteInEditMode]
    public class Ammo : MonoBehaviour
    {
        public enum AmmoType
        {
            Pistol,
            Shotgun,
        }

        [System.Serializable]
        public struct AmmoSprite
        {
            public AmmoType ammoType;
            public Sprite sprite;
        }

        [SerializeField] private AmmoType ammoType;
        [SerializeField] private AmmoSprite[] ammoSprites;

        private Dictionary<AmmoType, Sprite> _ammoTypeToSprite;

        private void OnRenderObject()
        {
            _ammoTypeToSprite = new Dictionary<AmmoType, Sprite>();
            foreach (var ammoSprite in ammoSprites)
            {
                _ammoTypeToSprite[ammoSprite.ammoType] = ammoSprite.sprite;
            }

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = _ammoTypeToSprite[ammoType];
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Weapon weapon = ObjectSearch.FindChild(other.transform, ammoType.ToString()).GetComponent<Weapon>();
                PickUp(weapon);
            }
        }

        void PickUp(Weapon weapon)
        {
            weapon.PickUpAmmo(ammoType);
            GetComponent<BoxCollider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}