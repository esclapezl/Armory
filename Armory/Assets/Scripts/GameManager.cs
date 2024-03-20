using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Camera;
using Levels;
using Levels.LevelSelection;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Utils;


public class GameManager : MonoBehaviour
{
    [NonSerialized] private LevelManager _levelManager;
    

    private void Awake()
    {
        _levelManager = transform.GetComponent<LevelManager>();
    }

    private void Start()
    {
        _levelManager.UpdateLevel();
    }

    private void Update()
    {
        //PLAYTEST USAGE---
        _levelManager.UpdateLevel();
        //-----------------
    }
}