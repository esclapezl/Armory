using UnityEngine;
using Utils;
using weapons;

namespace Weapons
{
    public class Ammo : MonoBehaviour
    {
        [SerializeField] private string ammoType;

        // Start is called before the first frame update
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Weapon weapon = ObjectSearch.FindChild(other.transform, ammoType).GetComponent<Weapon>();
                PickUp(weapon);
            }
        }

        void PickUp(Weapon weapon)
        {
            if (weapon.currentAmmo < weapon.magazineSize)
            {
                weapon.currentAmmo++;
                if (weapon.active)
                {
                    weapon.ammoDisplay.DisplayAmmo();
                }
            }
            else
            {
                weapon.totalAmmo++;
            }

            Destroy(GetComponent<BoxCollider2D>());
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}