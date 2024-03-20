using System;
using Levels;
using UnityEngine;
using Utils;

namespace Camera
{
    public class CameraMovements : MonoBehaviour
    {
        [NonSerialized] private Transform _playerTransform;
        [NonSerialized] private Transform _bottomLeftDelimiter;
        [NonSerialized] private Transform _topRightDelimiter;
        [SerializeField] public float smoothing = 5f;

        private void Awake()
        {
            _playerTransform = ObjectSearch.FindRoot("Player");
        }

        private void FixedUpdate()
        {
            CalculateCameraPosition();
        }

        public void SetDelimiters(Level level)
        {
            _bottomLeftDelimiter = level.BottomLeftDelimiter;
            _topRightDelimiter = level.TopRightDelimiter;
        }

        private void CalculateCameraPosition()
        {
            UnityEngine.Camera cam = ObjectSearch.FindRoot("Main Camera").GetComponent<UnityEngine.Camera>();
            float camWidth = cam.orthographicSize * 2.0f * cam.aspect;
            float camHeight = cam.orthographicSize * 2.0f;
            
            float posX = Mathf.Clamp(_playerTransform.position.x,
                _bottomLeftDelimiter.position.x + camWidth / 2,
                _topRightDelimiter.position.x - camWidth / 2);
            float posY = Mathf.Clamp(_playerTransform.position.y,
                _bottomLeftDelimiter.position.y + camHeight / 2,
                _topRightDelimiter.position.y - camHeight / 2);

            Vector3 targetPos = new Vector3(posX, posY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing * Time.deltaTime);
        }
    }
}