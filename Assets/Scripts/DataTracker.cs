using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataTracker : MonoBehaviour
{
    public static DataTracker instance = null;
    public bool[] levelcomplete;
    public bool[] allcollectibles;
    public int totaldeaths = 0;

    void Awake()
    {
        if (instance == null)
        {
            levelcomplete = new bool[SceneManager.sceneCountInBuildSettings];
            allcollectibles = new bool[SceneManager.sceneCountInBuildSettings];
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadLevel(int n)
    {
        SceneManager.LoadScene(n);
    }
}
