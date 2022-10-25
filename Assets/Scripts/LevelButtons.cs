using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelButtons : MonoBehaviour
{
    public bool debugging;
    DataTracker dt;

    string ConvertTimeToString(TimeSpan x)
    {
        string part = x.Seconds < 10 ? $"0{x.Seconds}" : $"{x.Seconds}";
        return $"{x.Minutes}:" + part + $".{x.Milliseconds}";
    }

    void Start()
    {
        dt = FindObjectOfType<DataTracker>().GetComponent<DataTracker>();

        TMP_Text x = transform.GetChild(0).GetComponent<TMP_Text>();
        x.text = "Total Time: " + ConvertTimeToString(dt.timetaken)+
            $"\nTotal Deaths: {dt.totaldeaths}";

        for (int i = 1; i < this.transform.childCount; i++)
        {
            Button y = this.transform.GetChild(i).GetComponent<Button>();

            if (debugging || i == 1 || dt.levelcomplete[i-1])
            {
                y.interactable = true;
                TMP_Text z = y.transform.GetChild(1).GetComponent<TMP_Text>();

                if (dt.levelcomplete[i])
                    z.text = "Fastest Time: " + ConvertTimeToString(dt.recordsno100[i]);

                if (dt.allcollectibles[i])
                {
                    y.gameObject.GetComponent<Image>().color = Color.yellow;
                    z.text += "\nFastest 100%: " + ConvertTimeToString(dt.records100[i]);
                }

            }
            else
                y.interactable = false;
        }
    }
}
