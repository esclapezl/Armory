using System;
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
        [NonSerialized] private int _selectedLevel = 0;
        [SerializeField] private GameObject levelSelectorPrefab;

        [Range(0, 5)] [SerializeField] private float verticalGap = 2;
        [Range(0, 5)] [SerializeField] private float horizontalGap = 2;

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

            int levelsPerRow = (int)(cameraWidth / (horizontalGap + levelSelectorSize));
            int levelsPerColumn = (int)(cameraHeight / (verticalGap + levelSelectorSize));
            
            float remaingingWidth = cameraWidth - (levelsPerRow * (levelSelectorSize + horizontalGap));

            int index = 0;
            foreach (LevelInfo levelInfo in levelInfos)
            {
                Transform levelSelectorTransform = _levelTransforms[index];
                levelInfo.Number = index;
                levelSelectorTransform.name = levelInfo.Number + "_" + levelInfo.Name;

                float x = (index%levelsPerRow * (levelSelectorSize + horizontalGap)) + remaingingWidth/2 + horizontalGap/2 + levelSelectorSize/2;
                float y = (index/levelsPerRow * (levelSelectorSize + verticalGap)) + levelSelectorSize;
                levelSelectorTransform.localPosition = new Vector3(x - cameraWidth / 2 , -y + cameraHeight / 2 , 0);
                index++;
            }
        }

        private void Update()
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
            StaticLevelInfos = levelInfos;
            GameManager.CurrentLevelNumber = levelIndex;
            SceneManager.LoadScene("MainScene");
        }
    }
}