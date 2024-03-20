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
            public string name;
            public int number;
            public bool completed;
        }
        
        [Serializable]
        public class LevelData
        {
            public LevelInfo[] levels;
        }
        

        [SerializeField] private LevelData levelInfos;

        [NonSerialized] private List<Transform> _levelTransforms;
        [NonSerialized] private int _selectedLevel = -1;
        [NonSerialized] private int _levelsPerRow;
        [SerializeField] private GameObject levelSelectorPrefab;

        [Range(0, 5)] [SerializeField] private float verticalGap;
        [Range(0, 5)] [SerializeField] private float verticalMargin;
        [Range(0, 5)] [SerializeField] private float horizontalGap;
        [Range(0, 5)] [SerializeField] private float horizontalMargin;
        
        [NonSerialized] private Coroutine _holdCoroutine;
        
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

            if (levelInfos.levels.Length > _levelTransforms.Count)
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
            LevelInfo levelInfo = levelInfos.levels[index];
            levelInfo.number = index;
            // levelInfo.Completed = à chopper dans le saveFile?;

            GameObject levelSelector = Instantiate(levelSelectorPrefab, transform, true);
            levelSelector.name = levelInfo.number + "_" + levelInfo.name;
            _levelTransforms.Add(levelSelector.transform);
        }

        private void RefreshLevels()
        {
            RefreshProgression();
            float levelSelectorSize = 1;
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
            float cameraHeight = 2f * mainCamera.orthographicSize;
            float cameraWidth = cameraHeight * mainCamera.aspect;

            _levelsPerRow = (int)((cameraWidth - horizontalMargin) / (horizontalGap + levelSelectorSize));

            float remaingingWidth =
                cameraWidth - (_levelsPerRow * (levelSelectorSize + horizontalGap)) - horizontalMargin;

            int index = 0;
            foreach (LevelInfo levelInfo in levelInfos.levels)
            {
                Transform levelSelectorTransform = _levelTransforms[index];
                levelInfo.number = index + 1;
                levelSelectorTransform.name = levelInfo.number + "_" + levelInfo.name;

                float x = (index % _levelsPerRow * (levelSelectorSize + horizontalGap)) + remaingingWidth / 2 +
                          levelSelectorSize / 2 + horizontalMargin / 2 + horizontalGap / 2;
                float y = (index / _levelsPerRow * (levelSelectorSize + verticalGap)) + levelSelectorSize / 2 +
                          verticalMargin / 2;
                levelSelectorTransform.localPosition = new Vector3(x - cameraWidth / 2, -y + cameraHeight / 2, 0);

                LevelSelector levelSelector = levelSelectorTransform.GetComponent<LevelSelector>();
                levelSelector.SetInfo(levelInfo);
                index++;
            }
        }

        private void RefreshProgression()
        {
            levelInfos = Data.LoadJsonFromFile<LevelData>(Application.dataPath + "/Data/Levels.json");
            //faire des anims pour les levels complétés ??
        }

        private void Update()
        {
            Dictionary<KeyCode, Func<int>> keyToFunctionMap = new Dictionary<KeyCode, Func<int>>
            {
                { KeyCode.LeftArrow, 
                    () => _selectedLevel % _levelsPerRow == 0 && _selectedLevel/_levelsPerRow < _levelTransforms.Count/_levelsPerRow ? _levelsPerRow - 1 : 
                    _selectedLevel % _levelsPerRow == 0 && _selectedLevel/_levelsPerRow == _levelTransforms.Count/_levelsPerRow ?  _levelTransforms.Count % _levelsPerRow  - 1 : 
                    -1 },
                { KeyCode.RightArrow, 
                    () => (_selectedLevel + 1) % _levelsPerRow == 0 && _selectedLevel/_levelsPerRow < _levelTransforms.Count/_levelsPerRow ? -_levelsPerRow + 1 : 
                    _selectedLevel  == _levelTransforms.Count - 1 ? -(_levelTransforms.Count % _levelsPerRow) + 1 : 
                    1 },
                { KeyCode.UpArrow, 
                    () => _selectedLevel/_levelsPerRow == 0 && _selectedLevel%_levelsPerRow < _levelTransforms.Count%_levelsPerRow ? (_levelTransforms.Count/_levelsPerRow)*_levelsPerRow :
                    _selectedLevel/_levelsPerRow == 0 && _selectedLevel%_levelsPerRow >= _levelTransforms.Count%_levelsPerRow ? ((_levelTransforms.Count/_levelsPerRow)-1)*_levelsPerRow :
                    -_levelsPerRow },
                { KeyCode.DownArrow, 
                    () => _selectedLevel/_levelsPerRow == _levelTransforms.Count/_levelsPerRow ? -(_levelTransforms.Count/_levelsPerRow)*_levelsPerRow :
                    _selectedLevel/_levelsPerRow == _levelTransforms.Count/_levelsPerRow - 1 && _selectedLevel%_levelsPerRow >= _levelTransforms.Count%_levelsPerRow ? -((_levelTransforms.Count/_levelsPerRow)-1)*_levelsPerRow :
                    _levelsPerRow }
            };

            foreach (var keyFunctionPair in keyToFunctionMap)
            {
                if (Input.GetKeyDown(keyFunctionPair.Key))
                {
                    if (_holdCoroutine != null)
                    {
                        StopCoroutine(_holdCoroutine);
                    }
                    _holdCoroutine = StartCoroutine(HoldCoroutine(keyFunctionPair.Key, keyFunctionPair.Value));
                    break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartLevel(_selectedLevel+1);
            }
        }

        private void UpdateSelectedLevel(int increment)
        {
            
                if (_selectedLevel == -1)
                {
                    _selectedLevel = 0;
                    HoverLevel(_selectedLevel);
                }
                else
                {
                    ClearHover(_selectedLevel);
                    _selectedLevel += increment;
                    HoverLevel(_selectedLevel);
                }
        }

        private IEnumerator HoldCoroutine(KeyCode keycode, Func<int> incrementFunc)
        {
            float delay = 0.5f;
            while (Input.GetKey(keycode))
            {
                UpdateSelectedLevel(incrementFunc.Invoke());
                yield return new WaitForSeconds(delay);
                delay = Mathf.Max(delay * 0.5f, 0.05f);
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
            Data.UpdateJsonFile(levelInfos, Application.dataPath + "/Data/Levels.json");
            LevelManager.CurrentLevelNumber = levelIndex;
            SceneManager.LoadScene("MainScene");
        }
    }
}