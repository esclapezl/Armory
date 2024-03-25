using System;
using TMPro;
using UnityEngine;
using Utils;

namespace Player.Inventory
{
    public class InventorySlotUid : MonoBehaviour
    {
        [NonSerialized] public int SlotNumber;
        [NonSerialized] public Sprite WeaponSprite;
        [NonSerialized] public TextMeshPro TotalAmmoText;
        [NonSerialized] public Sprite AmmoSprite;
        [NonSerialized] public bool Active;

        [SerializeField] public Sprite highlightSprite;
        [NonSerialized] private SpriteRenderer _highlightSpriteRenderer;
        [NonSerialized] private SpriteRenderer _weaponSpriteRenderer;
        [NonSerialized] private SpriteRenderer _ammoSpriteRenderer;

        private void Awake()
        {
            _highlightSpriteRenderer = ObjectSearch.FindChild(transform, "Highlight").GetComponent<SpriteRenderer>();
            _weaponSpriteRenderer = ObjectSearch.FindChild(transform, "Weapon").GetComponent<SpriteRenderer>();
            _ammoSpriteRenderer = ObjectSearch.FindChild(transform, "AmmoIcon").GetComponent<SpriteRenderer>();
            TotalAmmoText = ObjectSearch.FindChild(transform, "AmmoNumber").GetComponent<TextMeshPro>();
        }

        public void SetSlot(Sprite weaponSprite, int totalAmmo, Sprite ammoSprite)
        {
            _weaponSpriteRenderer.sprite = weaponSprite;
            TotalAmmoText.text = totalAmmo.ToString();
            _ammoSpriteRenderer.sprite = ammoSprite;
        }

        public void SetActive(bool active)
        {
            Active = active;
        }

        public void SetAmmo(int totalAmmo)
        {
            TotalAmmoText.text = totalAmmo.ToString();
        }

        public void Highlight()
        {
            _highlightSpriteRenderer.color = new Color(1, 1, 1, 1);
            float scaleUp = 1.2f;
            transform.localScale = new Vector3(scaleUp, scaleUp, 1);
            transform.localPosition = new Vector3(transform.localPosition.x, (1 - scaleUp) / 2, 0);
        }

        public void Unhighlight()
        {
            _highlightSpriteRenderer.color = new Color(1, 1, 1, 0);
            transform.localScale = new Vector3(1, 1, 1);
            transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
        }
    }
}