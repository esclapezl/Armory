using System;
using Player;
using UnityEngine;

namespace Weapons
{
    public class WeaponMovements : MonoBehaviour
    {
        [NonSerialized] public Player.Player Player;
        [NonSerialized] public Transform WeaponCenterTransform;
        [NonSerialized] public Transform WeaponTransform;

        private void Awake()
        {
            Player = GetComponent<Player.Player>();
            WeaponCenterTransform = transform.Find("Inventory");
            WeaponTransform = WeaponCenterTransform.Find("Weapons");
        }

        private void FixedUpdate()
        {
            if (!Player.dead && Player.PlayerMovements.Armed)
            {
                //weapon rotation
                Vector3 mouseWorld = UnityEngine.Camera.main!.ScreenToWorldPoint(Input.mousePosition);
                Vector3 rotation = mouseWorld - transform.position;
                float newRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
                WeaponCenterTransform.rotation = Quaternion.Euler(0, 0, newRotation);

                float zRotation = WeaponCenterTransform.rotation.eulerAngles.z;
                if (
                    (zRotation >= 90 && zRotation < 270 && Player.PlayerMovements.FacingRight)
                    || ((zRotation < 90 || zRotation >= 270) && !Player.PlayerMovements.FacingRight)
                )
                {
                    
                    Player.PlayerMovements.FlipPlayer();
                    FlipWeapon();
                }
            }
        }

        public void FlipWeapon()
        {
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