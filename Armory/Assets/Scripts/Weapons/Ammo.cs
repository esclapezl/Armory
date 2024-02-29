using DefaultNamespace;
using UnityEngine;
using weapons;

namespace Weapons
{
    public class Ammo : MonoBehaviour
    {
        [SerializeField] private string ammoType;
        // Start is called before the first frame update
        void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Player"))
            {
                Weapon weapon = Utils.FindChild(other.transform, ammoType).GetComponent<Weapon>();
                PickUp(weapon);
            }
        }

        void PickUp(Weapon weapon)
        {
            if (weapon.currentAmmo < weapon.magazineSize)
            {
                weapon.currentAmmo++;
                weapon.ammoDisplay.DisplayAmmo();
            }
            else
            {
                weapon.totalAmmo++;
            }
            Destroy(gameObject);
        }
    }
}
