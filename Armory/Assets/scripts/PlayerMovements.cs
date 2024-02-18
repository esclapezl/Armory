using TreeEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovements : MonoBehaviour
{
    public PlayerController controller;
    public float runSpeed = 40f;
    private float _horizontalMove = 0f;
    public bool armed = true;
    public Transform weaponCenterTransform;
    public Transform weaponTransform;
    
    private void Update()
    {
        _horizontalMove = Input.GetAxis("Horizontal") * runSpeed;
    }
    
    private void FixedUpdate()
    {
        //horizontal movement
        controller.Move(_horizontalMove * Time.fixedDeltaTime, false, false, armed);
        
        //weapon rotation
        Vector3 mouseWorld = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mouseWorld - transform.position;
        float newRotation = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        weaponCenterTransform.rotation = Quaternion.Euler(0, 0, newRotation);
        
        float zRotation = weaponCenterTransform.rotation.eulerAngles.z;
        if (
            (zRotation >= 90 && zRotation < 270 && controller.m_FacingRight)
            || ((zRotation < 90 || zRotation >= 270) && !controller.m_FacingRight)
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
    }
}
