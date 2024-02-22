using System.Collections;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovements : MonoBehaviour
{
    public PlayerController controller;
    
    
    public bool armed = true;
    public Transform weaponCenterTransform;
    public Transform weaponTransform;

    public SpriteRenderer playerSprite;
    public SpriteRenderer playerFilterSprite;
    public int health = 3;
    private Coroutine _damageCoroutine;
    private void Update()
    {
        
    }
    
    private void FixedUpdate()
    {
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

        // Invert the angle of the weapon
        float currentRotation = weaponTransform.localEulerAngles.z;
        float newRotation = -currentRotation;
        weaponTransform.localEulerAngles = new Vector3(0, 0, newRotation);
    }

    public void TakeDamage()
    {
        if (_damageCoroutine != null)
        {
            StopCoroutine(_damageCoroutine);
        }
        _damageCoroutine = StartCoroutine(TakeDamageCoroutine());
    }
    
    private IEnumerator TakeDamageCoroutine()
    {
        Color targetColor = new Color(1, 0, 0);
        playerFilterSprite.color = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
        _damageCoroutine = null;
        while(playerFilterSprite.color.a > 0)
        {
            playerFilterSprite.color = Color.Lerp(playerFilterSprite.color, new Color(targetColor.r, targetColor.g, targetColor.b, 0f), Time.deltaTime * 10);
            yield return null;
        }
    }
}
