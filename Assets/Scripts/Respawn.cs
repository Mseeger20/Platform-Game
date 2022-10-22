using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Respawn : MonoBehaviour
{
    GameObject spawn;
    PlayerController pc;
    public bool teleport;
    Stopwatch stopwatch;
    int deaths = 0;
    string timetotext;

    void Start()
    {
        spawn = GameObject.Find("Spawn");
        pc = gameObject.GetComponent<PlayerController>();
        teleport = true;

        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            pc.enabled = false;
            teleport = true;
            deaths++;
        }

        if (collision.gameObject.CompareTag("End"))
        {
            UnityEngine.Debug.Log("reached the end");
            stopwatch.Stop();
            TimeSpan x = stopwatch.Elapsed;
            string part = x.Seconds < 10 ? $"0{x.Seconds}" : $"{x.Seconds}";
            timetotext = $"{x.Minutes}:" + part + $".{x.Milliseconds}";
        }
    }

    void FixedUpdate()
    {
        if (teleport)
        {
            gameObject.transform.position = spawn.transform.position;
            teleport = false;
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.7f);
        pc.enabled = true;
    }
}