using System.Collections;
using Player.Inventory;
using UnityEngine;
using weapons;

namespace Weapons.Pistol
{
    public class Pistol : Weapon
    {
        protected override void ShootingMethod()
        {
            GameObject bulletObject = Instantiate(bulletPrefab, CannonTransform.position, transform.rotation);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.SetSpeed(bulletSpeed);
        }

        protected override IEnumerator Reload()
        {
            IsReloading = true;
            Quaternion originalRotation = transform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(0, 0, 90 * transform.localScale.y);

            // Rotate quickly to -90 degrees
            while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.01f)
            {
                transform.localRotation =
                    Quaternion.RotateTowards(transform.localRotation, targetRotation,
                        Time.deltaTime * 500); // Adjust the 500 to control how quickly the gun rotates
                yield return null;
            }

            //refill mag
            Inventory.WeaponAmmo weaponAmmo = _inventory.GetAmmoForWeapon(transform.name);
            int totalAmmo = weaponAmmo.ammo;
            for (int i = currentAmmo; i < Mathf.Min(totalAmmo + currentAmmo, magazineSize); i++)
            {
                AmmoDisplay.EnableBullet(magazineSize - i - 1);
                currentAmmo++;
                weaponAmmo.ammo--;
                _inventory.InventoryUid.RefreshInventoryUid();
                yield return new WaitForSeconds(reloadTime / magazineSize);
            }

            // Rotate back slowly to original position
            while (Quaternion.Angle(transform.localRotation, originalRotation) > 0.01f)
            {
                transform.localRotation =
                    Quaternion.Lerp(transform.localRotation, originalRotation, Time.deltaTime * 30);
                yield return null;
            }

            transform.localRotation = originalRotation;
            IsReloading = false;
            ReloadCoroutine = null;
        }
    }
}