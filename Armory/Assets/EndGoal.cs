using System;
using System.Collections;
using System.Collections.Generic;
using Levels;
using UnityEngine;

public class EndGoal : MonoBehaviour
{
    private Level _level;

    private void Awake()
    {
        _level = transform.parent.GetComponent<Level>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EndLevel();
        }
    }

    private void EndLevel()
    {
        _level.EndLevel();
    }
}
