using System;
using System.Collections;
using UnityEngine;

namespace Effects
{
    public class Squishable : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [NonSerialized] private Vector3 _originalScale;
        [NonSerialized] private Vector3 _originalPositon;

        private void Awake()
        {
            _originalScale = targetTransform.localScale;
            _originalPositon = targetTransform.localPosition;
        }

        public IEnumerator Squish(float duration, float squishScale, bool horizontalSquish)
        {
            float elapsedTime = 0;
            Vector3 squishScaleVector = GetSquishScaleVector(squishScale, horizontalSquish);
            targetTransform.localScale = squishScaleVector;

            while (elapsedTime < duration)
            {
                targetTransform.localScale = Vector3.Lerp(squishScaleVector, _originalScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            targetTransform.localScale = _originalScale;
        }
        
        public IEnumerator GroundedSquish(float duration, float squishScale, bool horizontalSquish)
        {
            float elapsedTime = 0;
            Vector3 squishScaleVector = GetSquishScaleVector(squishScale, horizontalSquish);
            targetTransform.localScale = squishScaleVector;
            targetTransform.localPosition = new Vector3(_originalPositon.x,  (_originalScale.y - squishScaleVector.y)/2, _originalPositon.z);

            while (elapsedTime < duration)
            {
                targetTransform.localScale = Vector3.Lerp(squishScaleVector, _originalScale, (elapsedTime / duration));
                targetTransform.localPosition = Vector3.Lerp(new Vector3(_originalPositon.x,  (_originalScale.y - squishScaleVector.y)/2, _originalPositon.z), _originalPositon, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            targetTransform.localScale = _originalScale;
            targetTransform.localPosition = _originalPositon;
        }
        
        private Vector3 GetSquishScaleVector(float squishScale, bool horizontalSquish)
        {
            Vector3 squishScaleVector = new Vector3(_originalScale.x, _originalScale.y, _originalScale.z);
            if (horizontalSquish)
            {
                squishScaleVector.x *= squishScale;
                squishScaleVector.y /= squishScale;
            }
            else
            {
                squishScaleVector.y *= squishScale;
                squishScaleVector.x /= squishScale;
            }

            return squishScaleVector;
        }
    }
}
