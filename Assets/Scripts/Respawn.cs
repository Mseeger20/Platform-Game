using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public GameObject spawn;
    public PlayerController pc;
    public bool teleport;
    void Start()
    {
        pc = gameObject.GetComponent<PlayerController>();
        teleport = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            pc.enabled = false;
            teleport = true;
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