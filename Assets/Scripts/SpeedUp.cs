using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : MonoBehaviour
{
    public PlayerController pc;
    void Start()
    {
        pc = gameObject.GetComponent<PlayerController>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Conveyor"))
        {
            pc.moveModel.hspeed = 20;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Conveyor"))
        {
            pc.moveModel.hspeed = 10;
        }
    }
}