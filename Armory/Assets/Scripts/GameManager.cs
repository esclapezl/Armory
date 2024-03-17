using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using Levels;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Serialization;
using Utils;


public class GameManager : MonoBehaviour
{
    [Header("Level Selection")] [NonSerialized]
    public Transform LevelFolder;

    [SerializeField] public static int CurrentLevelNumber;
    [NonSerialized] public Level CurrentLevel;
    [NonSerialized] private CameraMovements _cameraMovements;

    void Awake()
    {
        _cameraMovements = ObjectSearch.FindRoot("Main Camera").GetComponent<CameraMovements>();
        LevelFolder = ObjectSearch.FindChild(transform, "Levels");
        SetUpLevels();
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

    private void SetUpLevels()
    {
        for (int i = 0; i < LevelFolder.childCount; i++)
        {
            Transform child = LevelFolder.GetChild(i);
            child.name = (i + 1) + "_" + child.name;
            Level childLevel = child.GetComponent<Level>();
            childLevel.LevelNumber = i + 1;
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
        CurrentLevel.StartLevel();
        _cameraMovements.SetDelimiters(CurrentLevel);
    }

    private void ExitLevel(Level level)
    {
        level.ExitLevel();
    }
}