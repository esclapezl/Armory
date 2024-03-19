using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;

namespace Levels.LevelSelection
{
    [ExecuteInEditMode]
    public class LevelSelection : MonoBehaviour
    {
        [Serializable]
        public class LevelInfo
        {
            public string Name;
            public int Number;
            public bool Completed;
        }

        [SerializeField] private LevelInfo[] levelInfos;
        [NonSerialized] public static LevelInfo[] StaticLevelInfos;

        [NonSerialized] private List<Transform> _levelTransforms;
        [NonSerialized] private int _selectedLevel = -1;
        [NonSerialized] private int _levelsPerRow;
        [SerializeField] private GameObject levelSelectorPrefab;

        [Range(0, 5)] [SerializeField] private float verticalGap;
        [Range(0, 5)] [SerializeField] private float verticalMargin;
        [Range(0, 5)] [SerializeField] private float horizontalGap;
        [Range(0, 5)] [SerializeField] private float horizontalMargin;

        private void OnRenderObject()
        {
            if (_levelTransforms == null || transform.childCount != _levelTransforms.Count)
            {
                _levelTransforms = new List<Transform>();
                foreach (Transform child in transform)
                {
                    _levelTransforms.Add(child);
                }
            }

            if (levelInfos.Length > _levelTransforms.Count)
            {
                CreateLevelSelector();
            }
            else
            {
                RefreshLevels();
            }
        }


        private void CreateLevelSelector()
        {
            int index = _levelTransforms.Count;
            LevelInfo levelInfo = levelInfos[index];
            levelInfo.Number = index;
            // levelInfo.Completed = à chopper dans le saveFile?;

            GameObject levelSelector = Instantiate(levelSelectorPrefab, transform, true);
            levelSelector.name = levelInfo.Number + "_" + levelInfo.Name;
            _levelTransforms.Add(levelSelector.transform);
        }

        private void RefreshLevels()
        {
            float levelSelectorSize = 1;
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            _levelsPerRow = (int)((cameraWidth - horizontalMargin) / (horizontalGap + levelSelectorSize));
            int levelsPerColumn = (int)(cameraHeight / (verticalGap + levelSelectorSize));

            float remaingingWidth =
                cameraWidth - (_levelsPerRow * (levelSelectorSize + horizontalGap)) - horizontalMargin;

            int index = 0;
            foreach (LevelInfo levelInfo in levelInfos)
            {
                Transform levelSelectorTransform = _levelTransforms[index];
                levelInfo.Number = index;
                levelSelectorTransform.name = levelInfo.Number + "_" + levelInfo.Name;

                float x = (index % _levelsPerRow * (levelSelectorSize + horizontalGap)) + remaingingWidth / 2 +
                          levelSelectorSize / 2 + horizontalMargin / 2 + horizontalGap / 2;
                float y = (index / _levelsPerRow * (levelSelectorSize + verticalGap)) + levelSelectorSize / 2 +
                          verticalMargin / 2;
                levelSelectorTransform.localPosition = new Vector3(x - cameraWidth / 2, -y + cameraHeight / 2, 0);

                LevelSelector levelSelector = levelSelectorTransform.GetComponent<LevelSelector>();
                levelSelector.LevelNumber = levelInfo.Number;
                levelSelector.LevelTitle = levelInfo.Name;
                levelSelector.Refresh();
                index++;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StartCoroutine(HoldCoroutine("LeftArrow", () => _selectedLevel > 0, -1));
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StartCoroutine(HoldCoroutine("RightArrow", () => _selectedLevel < _levelTransforms.Count - 1, 1));
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                StartCoroutine(HoldCoroutine("UpArrow", () => _selectedLevel >= _levelsPerRow, -_levelsPerRow));
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                StartCoroutine(HoldCoroutine("DownArrow", () => _selectedLevel / _levelsPerRow < _levelTransforms.Count / _levelsPerRow, _levelsPerRow));
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartLevel(_selectedLevel);
            }
        }

        private void UpdateSelectedLevel(bool condition, int increment)
        {
            if (_selectedLevel == -1)
            {
                _selectedLevel = 0;
            }
            else if (condition)
            {
                ClearHover(_selectedLevel);
                _selectedLevel += increment;
            }

            HoverLevel(_selectedLevel);
        }

        private IEnumerator HoldCoroutine(string keycode, Func<bool> conditionFunc, int increment)
        {
            float delay = 0.5f;
            while (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), keycode)))
            {
                if (conditionFunc.Invoke())
                {
                    UpdateSelectedLevel(true, increment);
                }
                yield return new WaitForSeconds(delay);
                delay = Mathf.Max(delay*0.5f, 0.05f);
            }
        }

        private void HoverLevel(int levelIndex)
        {
            _levelTransforms[levelIndex].GetComponent<LevelSelector>().Highlight();
        }

        private void ClearHover(int levelIndex)
        {
            _levelTransforms[levelIndex].GetComponent<LevelSelector>().Unhighlight();
        }

        private void StartLevel(int levelIndex)
        {
            StaticLevelInfos = levelInfos;
            GameManager.CurrentLevelNumber = levelIndex;
            SceneManager.LoadScene("MainScene");
        }
    }
}