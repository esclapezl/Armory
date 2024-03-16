using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Levels.LevelSelection
{
    public class LevelSelection : MonoBehaviour
    {
        [NonSerialized] private List<Transform> _levelTransforms;
        [NonSerialized] private int _selectedLevel = 0;
        [NonSerialized] private GameManager _gameManager;
        [SerializeField] private GameObject levelSelectorPrefab;

        private void Awake()
        {
            _gameManager = ObjectSearch.FindRoot("GameManager").GetComponent<GameManager>();
            _levelTransforms = new List<Transform>();
        }

        private void Start()
        {
            for (int i = 1; i < _gameManager.LevelFolder.childCount; i++)
            {
                GameObject level = Instantiate(levelSelectorPrefab, transform, true);
                level.transform.localPosition = new Vector3(2 * i, 0, 0);
                level.name = (i) + "_" + _gameManager.LevelFolder.GetChild(i).name;
                _levelTransforms.Add(level.transform);
                ClearHover(i - 1);
            }
        }

        private void Update()
        {
            if (_gameManager.currentLevelNumber == 1)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (_selectedLevel > 0)
                    {
                        ClearHover(_selectedLevel);
                        _selectedLevel--;
                        HoverLevel(_selectedLevel);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (_selectedLevel < _levelTransforms.Count - 1)
                    {
                        ClearHover(_selectedLevel);
                        _selectedLevel++;
                        HoverLevel(_selectedLevel);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StartLevel(_selectedLevel);
                }
            }
        }

        private void HoverLevel(int levelIndex)
        {
            SpriteRenderer spriteRenderer = _levelTransforms[levelIndex].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 0, 0, 1f);
        }

        private void ClearHover(int levelIndex)
        {
            SpriteRenderer spriteRenderer = _levelTransforms[levelIndex].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0, 0, 0, 0.5f);
        }

        private void StartLevel(int levelIndex)
        {
            Debug.Log(_selectedLevel);
            _gameManager.StartLevel(_selectedLevel);
        }
    }
}