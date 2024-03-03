using System;
using UnityEngine;
using Utils;

namespace Levels
{
    public class Level : MonoBehaviour
    {
        [NonSerialized] private bool _active = false;
        
        [NonSerialized] private GameManager _gameManager;
        
        [NonSerialized] private Transform _startPosition;
        [NonSerialized] private Transform _playerTransform;
        [NonSerialized] public Transform BottomLeftDelimiter;
        [NonSerialized] public Transform TopRightDelimiter;
        
        [NonSerialized] public bool LevelCompleted = false;
        [NonSerialized] public int LevelNumber;

        private void Awake()
        {
            _gameManager = ObjectSearch.FindRoot("GameManager").GetComponent<GameManager>();
            _startPosition = ObjectSearch.FindChild(transform.transform,"StartPosition");
            _playerTransform = ObjectSearch.FindRoot("Player");
            BottomLeftDelimiter = ObjectSearch.FindChild(transform,"BottomLeftDelimiter");
            TopRightDelimiter = ObjectSearch.FindChild(transform,"TopRightDelimiter");
        }

        private void Update()
        {
            if (_active)
            {
                
            }
        }

        public void StartLevel()
        {
            _active = true;
            _playerTransform.position = _startPosition.position;
        }

        public void EndLevel()
        {
            _active = false;
            LevelCompleted = true;
            if (_gameManager.LevelFolder.childCount < LevelNumber)
            {
                _gameManager.StartLevel(_gameManager.currentLevelNumber+1);
            }
            else
            {
                ExitLevel();
            }
        }

        public void ExitLevel()
        {
            _active = false;
            throw new Exception("No menu yet");
        }
    }
}
