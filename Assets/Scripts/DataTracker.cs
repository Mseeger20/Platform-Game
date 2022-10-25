using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DataTracker : MonoBehaviour
{
    public static DataTracker instance = null;
    public bool[] levelcomplete = new bool[SceneManager.sceneCountInBuildSettings];
    public bool[] allcollectibles = new bool[SceneManager.sceneCountInBuildSettings];
    public int totaldeaths = 0;

    void Awake()
    {
        if (instance == null)
        {
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
