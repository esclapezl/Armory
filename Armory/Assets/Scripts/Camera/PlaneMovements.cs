using System;
using UnityEngine;
using Utils;

namespace Camera
{
    public class PlaneMovements : MonoBehaviour
    {
        [NonSerialized] private Transform _cameraTransform;

        private void Awake()
        {
            _cameraTransform = ObjectSearch.FindRoot("Main Camera");
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
                depth = Mathf.Abs(transform.position.z) / _cameraTransform.position.z;
            }
            else if (transform.position.z > 0)
            {
                depth = 1 + transform.position.z / _cameraTransform.position.z;
            }

            float posX = _cameraTransform.position.x * depth;
            float posY = _cameraTransform.position.y * depth;

            transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}