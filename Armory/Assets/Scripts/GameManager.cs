using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Camera;
using Levels;
using Levels.LevelSelection;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Serialization;
using Utils;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    [Header("Level Selection")] [NonSerialized]
    public Transform LevelFolder;

    [SerializeField] public static int CurrentLevelNumber;
    [NonSerialized] public Level CurrentLevel;
    [NonSerialized] public LevelSelection.LevelData LevelData;
    [NonSerialized] private CameraMovements _cameraMovements;

    private void Awake()
    {
        LevelFolder = ObjectSearch.FindChild(transform, "Levels");
        LevelData = Data.LoadJsonFromFile<LevelSelection.LevelData>(Application.dataPath + "/Data/Levels.json");
        _cameraMovements = ObjectSearch.FindRoot("Main Camera").GetComponent<CameraMovements>();
        RefreshLevels();
    }
    private void Start()
    {
        UpdateLevel();
    }

    private void Update()
    {
        //PLAYTEST USAGE---
        UpdateLevel();
        //-----------------
    }

    private void RefreshLevels()
    {
        for (int i = 0; i < LevelFolder.childCount; i++)
        {
            Transform child = LevelFolder.GetChild(i);
            child.name =  LevelData.levels.Length > i ? i + "_" + LevelData.levels[i].name : i + "_noData";
            Level childLevel = child.GetComponent<Level>();
            childLevel.LevelNumber = i;
        }
    }

    private void UpdateLevel()
    {
        if (CurrentLevel == null || CurrentLevelNumber != CurrentLevel.LevelNumber)
        {
            if (CurrentLevelNumber > LevelFolder.childCount)
            {
                CurrentLevelNumber = LevelFolder.childCount;
            }
            else if (CurrentLevelNumber == 0)
            {
                CurrentLevelNumber = 1;
            }
            StartLevel(CurrentLevelNumber);
        }
    }

    public void StartLevel(int level)
    {
        CurrentLevelNumber = level;
        CurrentLevel = ObjectSearch.FindChild(LevelFolder, CurrentLevelNumber + "_.*").GetComponent<Level>();
        _cameraMovements.SetDelimiters(CurrentLevel);
        CurrentLevel.StartLevel();
    }

    private void ExitLevel(Level level)
    {
        level.ExitLevel();
    }
}