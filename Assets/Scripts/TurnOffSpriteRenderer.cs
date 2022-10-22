using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffSpriteRenderer : MonoBehaviour
{
    SpriteRenderer x;

    private void Awake()
    {
        x = this.GetComponent<SpriteRenderer>();
        x.enabled = false;
    }
}
