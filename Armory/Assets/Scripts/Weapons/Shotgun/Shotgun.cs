using System.Collections;
using TreeEditor;
using UnityEngine;
using weapons;
using Weapons.Pistol;

namespace Weapons.Shotgun
{
    public class Shotgun : Weapon
    {
        protected override void Shoot()
        {
            Collider2D hitCollider = Physics2D.OverlapCircle(cannonTransform.position, 0.1f);
            if (hitCollider == null)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 bulletRotation = transform.rotation.eulerAngles;
                    bulletRotation.z += Random.Range(-10, 10);
                    GameObject bulletObject = Instantiate(bulletPrefab, cannonTransform.position,
                        Quaternion.Euler(bulletRotation));
                    Bullet bullet = bulletObject.GetComponent<Bullet>();
                    bulletObject.transform.localScale = new Vector3(2.5f, 2.5f, 1);
                    bullet.SetSpeed(Random.Range(bulletSpeed - 5, bulletSpeed));
                }
            }

            nextFireTime = fireRate;
            ammoDisplay.ToggleUidBullet(magazineSize - currentAmmo);
            currentAmmo--;

            KnockBack();
        }

        protected override IEnumerator Reload()
        {
            isReloading = true;
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
            for (int i = currentAmmo; i < Mathf.Min(totalAmmo + currentAmmo, magazineSize); i++)
            {
                ammoDisplay.EnableBullet(magazineSize - i - 1);
                currentAmmo++;
                totalAmmo--;
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
            isReloading = false;
            reloadCoroutine = null;
        }
    }
}