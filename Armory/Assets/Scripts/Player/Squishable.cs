using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Player
{
    public class Squishable : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [NonSerialized] private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = targetTransform.localScale;
        }

        public IEnumerator Squish(float duration, float squishScale, bool horizontalSquish)
        {
            float localScaleX = targetTransform.localScale.x < 0 ? -1 : 1;
            Vector3 originalScale = new Vector3(localScaleX, _originalScale.y, _originalScale.z);
            float elapsedTime = 0;
            targetTransform.localScale = originalScale;
            
            Vector3 squishScaleVector = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            if (horizontalSquish)
            {
                squishScaleVector.x = squishScaleVector.x * squishScale;
                squishScaleVector.y = squishScaleVector.y / squishScale;
            }
            else
            {
                squishScaleVector.y = squishScaleVector.y * squishScale;
                squishScaleVector.x = squishScaleVector.x / squishScale;
            }
            
            targetTransform.localScale = squishScaleVector;

            while (elapsedTime < duration)
            {
                targetTransform.localScale = Vector3.Lerp(squishScaleVector, originalScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            targetTransform.localScale = originalScale;
        }
    }
}
