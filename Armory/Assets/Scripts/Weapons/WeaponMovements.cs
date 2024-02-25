using Player;
using UnityEngine;

namespace Weapons
{
    public class WeaponMovements : MonoBehaviour
    {
        public PlayerMovements controller;
    
        public bool armed = true;
        public Transform weaponCenterTransform;
        public Transform weaponTransform;
        private void FixedUpdate()
        {
            //weapon rotation
            Vector3 mouseWorld = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mouseWorld - transform.position;
            float newRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            weaponCenterTransform.rotation = Quaternion.Euler(0, 0, newRotation);
        
            float zRotation = weaponCenterTransform.rotation.eulerAngles.z;
            if (
                (zRotation >= 90 && zRotation < 270 && controller.FacingRight)
                || ((zRotation < 90 || zRotation >= 270) && !controller.FacingRight)
            ) {
                if (armed) {
                    controller.Flip();
                }
                FlipWeapon();
            }
        }

        private void FlipWeapon() {
            Vector3 theScale = weaponTransform.localScale;
            theScale.y *= -1;
            weaponTransform.localScale = theScale;

            // Invert the angle of the weapon
            float currentRotation = weaponTransform.localEulerAngles.z;
            float newRotation = -currentRotation;
            weaponTransform.localEulerAngles = new Vector3(0, 0, newRotation);
        }

    
    }
}
