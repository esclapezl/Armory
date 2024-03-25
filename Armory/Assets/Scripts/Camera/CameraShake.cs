using System.Collections;
using UnityEngine;

namespace Camera
{
    public class CameraShake : MonoBehaviour
    {
        public IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPosition = transform.localPosition;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                originalPosition = new Vector3(transform.localPosition.x - x,
                    transform.localPosition.y - y,
                    originalPosition.z);

                transform.localPosition = new Vector3(originalPosition.x + x,
                    originalPosition.y + y,
                    originalPosition.z);
 
                elapsed += Time.deltaTime;

                yield return null;
            }

            transform.localPosition = originalPosition;
        }
    }
}