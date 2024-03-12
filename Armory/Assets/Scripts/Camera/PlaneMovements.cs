using System;
using UnityEngine;
using Utils;

namespace Camera
{
    public class PlaneMovements : MonoBehaviour
    {
        [NonSerialized] private Transform _cameraTransform;
        [NonSerialized] private Vector3 _originalPosition;

        private void Awake()
        {
            _cameraTransform = ObjectSearch.FindRoot("Main Camera");
            _originalPosition = transform.position;
        }

        private void FixedUpdate()
        {
            CalculatePlanePosition();
        }

        private void CalculatePlanePosition()
        {
            float depth = 0;
            if (transform.position.z < 0)
            {
                depth = Mathf.Abs(transform.position.z) / _cameraTransform.localPosition.z;
            }
            else if (transform.position.z > 0)
            {
                depth = 1 + transform.position.z / _cameraTransform.localPosition.z;
            }

            float posX = _originalPosition.x + _cameraTransform.localPosition.x * depth;
            float posY = _originalPosition.y + _cameraTransform.localPosition.y * depth;

            transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}