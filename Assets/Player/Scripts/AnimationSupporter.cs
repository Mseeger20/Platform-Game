using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSupporter : MonoBehaviour
{
    [SerializeField] PlayerController pl;

    private void Awake()
    {
        pl = GetComponent<PlayerController>();
    }

    public void moveForward(float velocity)
    {
        float vel = (int)pl.moveModel.Direction * velocity;
        pl.playerRB.velocity = new Vector2 (vel, pl.playerRB.velocity.y);
    }

    public void EndDash()
    {
        if (pl.generalState == pl.dashState)
        {
            if (pl.jumpModel.isGrounded) pl.ChangeState(pl.moveState);
            else pl.ChangeState(pl.jumpState);
        }
    }

    public void setGravity(float scale)
    {
        pl.playerRB.gravityScale = scale;
    }
}
