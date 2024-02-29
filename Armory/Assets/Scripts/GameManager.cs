using System;
using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Serialization;
using Utils;


public class GameManager : MonoBehaviour
{
    [NonSerialized] private Transform _levelFolder;
    [SerializeField] public int currentLevelNumber;
    [NonSerialized] public Level CurrentLevel;

    void Awake()
    {
        _levelFolder = ObjectSearch.FindChild(transform,"Levels");
        SetUpLevels();
    }
    private void Update()
    {
        //PLAYTEST USAGE---
        if (CurrentLevel == null || currentLevelNumber != CurrentLevel.LevelNumber)
        {
            if(currentLevelNumber > _levelFolder.childCount)
            {
                currentLevelNumber = _levelFolder.childCount;
            }
            CurrentLevel = ObjectSearch.FindChild(_levelFolder, currentLevelNumber+"_.*").GetComponent<Level>();
            StartLevel(CurrentLevel);
        }
        //-----------------
    }

    private void SetUpLevels()
    {
        for(int i = 0; i < _levelFolder.childCount; i++)
        {
            Transform child = _levelFolder.GetChild(i);
            child.name = (i+1) + "_" + child.name;
            Level childLevel = child.GetComponent<Level>();
            childLevel.LevelNumber = i+1;
        }
    }

    private void StartLevel(Level level)
    {
        level.StartLevel();
    }

    private void ExitLevel()
    {
        
    }
}
