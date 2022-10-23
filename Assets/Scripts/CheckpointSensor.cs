using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSensor : MonoBehaviour
{
    PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        pc = this.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            UnityEngine.Debug.Log("reached a checkpoint");
            pc.spawn.transform.position = collision.gameObject.transform.position;
        }

    }
}
