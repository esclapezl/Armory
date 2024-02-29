using System;
using UnityEngine;
using Utils;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        // [SerializeField] [Range(0, 20)] [NonSerialized] [NonSerialized]
        [NonSerialized] private Transform _startPosition;
        [NonSerialized] private Transform _playerTransform;
        [NonSerialized] public bool LevelCompleted = false;
        [NonSerialized] public int LevelNumber;

        private void Awake()
        {
            _startPosition = ObjectSearch.FindChild(transform.transform,"StartPosition");
            _playerTransform = ObjectSearch.FindRoot("Player");
        }

        public void StartLevel()
        {
            _playerTransform.position = _startPosition.position;
        }
    }
}
