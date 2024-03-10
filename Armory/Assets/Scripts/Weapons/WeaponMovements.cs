using System;
using Player;
using UnityEngine;

namespace Weapons
{
    public class WeaponMovements : MonoBehaviour
    {
        public bool armed = true;
        [NonSerialized] public PlayerMovements PlayerMovements;
        [NonSerialized] public Transform WeaponCenterTransform;
        [NonSerialized] public Transform WeaponTransform;
        
        private void Awake()
        {
            PlayerMovements = GetComponent<PlayerMovements>();
            WeaponCenterTransform = transform.Find("Inventory");
            WeaponTransform = WeaponCenterTransform.Find("Weapons");
        }
        private void FixedUpdate()
        {
            //weapon rotation
            Vector3 mouseWorld = UnityEngine.Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            Vector3 rotation = mouseWorld - transform.position;
            float newRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
            WeaponCenterTransform.rotation = Quaternion.Euler(0, 0, newRotation);
        
            float zRotation = WeaponCenterTransform.rotation.eulerAngles.z;
            if (
                (zRotation >= 90 && zRotation < 270 && PlayerMovements.facingRight)
                || ((zRotation < 90 || zRotation >= 270) && !PlayerMovements.facingRight)
            ) {
                if (armed) {
                    PlayerMovements.Flip();
                }
                FlipWeapon();
            }
        }

        private void FlipWeapon() {
            Vector3 theScale = WeaponTransform.localScale;
            theScale.y *= -1;
            WeaponTransform.localScale = theScale;

            // Invert the angle of the weapon
            float currentRotation = WeaponTransform.localEulerAngles.z;
            float newRotation = -currentRotation;
            WeaponTransform.localEulerAngles = new Vector3(0, 0, newRotation);
        }

    
    }
}
