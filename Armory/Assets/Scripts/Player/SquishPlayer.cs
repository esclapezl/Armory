using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishPlayer : MonoBehaviour
{
    public IEnumerator Squish(float duration, float squishForce, bool horizontalSquish)
    {
        float elapsedTime = 0;
        Vector3 originalScale = transform.localScale;
        Vector3 squishScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        if (horizontalSquish)
        {
            squishScale.x = squishScale.x * squishForce;
        }
        else
        {
            squishScale.y = squishScale.y * squishForce;
        }
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, squishScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = squishScale;
    }
}
