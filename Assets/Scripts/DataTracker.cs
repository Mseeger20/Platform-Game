using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class DataTracker : MonoBehaviour
{
    public static DataTracker instance = null;
    public bool[] levelcomplete;
    public bool[] allcollectibles;
    public int totaldeaths = 0;
    public TimeSpan[] recordsno100;
    public TimeSpan[] records100;
    public TimeSpan timetaken;

    void Awake()
    {
        if (instance == null)
        {
            levelcomplete = new bool[SceneManager.sceneCountInBuildSettings];
            allcollectibles = new bool[SceneManager.sceneCountInBuildSettings];
            recordsno100 = new TimeSpan[SceneManager.sceneCountInBuildSettings];
            records100 = new TimeSpan[SceneManager.sceneCountInBuildSettings];

            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
