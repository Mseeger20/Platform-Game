using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffSpriteRenderer : MonoBehaviour
{
    public SpriteRenderer x;

    private void Awake()
    {
        x.enabled = false;
    }
}
