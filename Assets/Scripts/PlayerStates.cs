using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    public abstract void EnterState(PlayerController pl);
    public abstract void Update(PlayerController pl);
    public abstract void FixedUpdate(PlayerController pl);
    public abstract void ExitState(PlayerController pl);
}

public class PlayerMoveState : PlayerState
{
    private int moveBoolHash = Animator.StringToHash("isMoving");
    public override void EnterState(PlayerController pl)
    {
        pl.statevisualizer = PlayerController.state.move;
        pl.jumpModel.jumpCount = pl.jumpModel.jumpCountMax;
        pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed, pl.playerRB.velocity.y);
    }
    public override void Update(PlayerController pl)
    {
        pl.playerAnim.SetBool(moveBoolHash, pl.moveModel.HorizontalMovement != 0f);
    }
    public override void FixedUpdate(PlayerController pl)
    {
        if (pl.conveyor == null)
        {
            pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.left && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Left)
        {
            pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed - pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.left && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Right)
        {
            pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed - pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.right && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Right)
        {
            pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed + pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.right && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Left)
        {
            pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed + pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        Debug.Log(pl.playerRB.velocity);
    }
    public override void ExitState(PlayerController pl)
    {

    }
}

public class PlayerJumpState : PlayerState
{
    public override void EnterState(PlayerController pl)
    {
        /*if(!pl.slideModel.wallJumped)
            pl.jumpModel.jumpCount -= 1;*/
        pl.statevisualizer = PlayerController.state.jump;
    }
    public override void Update(PlayerController pl)
    {
        if (pl.jumpModel.isGrounded && Mathf.Abs(pl.playerRB.velocity.y) < 0.1f) pl.ChangeState(pl.moveState);
    }
    public override void FixedUpdate(PlayerController pl)
    {
        pl.playerRB.velocity = new Vector2(pl.moveModel.HorizontalMovement * pl.moveModel.hspeed, pl.playerRB.velocity.y);
    }
    public override void ExitState(PlayerController pl)
    {
        //pl.jumpModel.jumpCount = pl.jumpModel.jumpCountMax;
    }
}

public class PlayerDashState : PlayerState
{
    private int dashTriggerHash = Animator.StringToHash("dashTrigger");
    public override void EnterState(PlayerController pl)
    {
        //Play Animation
        pl.playerAnim.SetTrigger(dashTriggerHash);
        pl.statevisualizer = PlayerController.state.dash;
        pl.playerRB.velocity = Vector2.zero;
        pl.playerRB.velocity = new Vector2(pl.dashModel.dashSpeed, 0) * (int)pl.moveModel.Direction;
        if (!pl.jumpModel.isGrounded)
        {
            pl.playerRB.gravityScale = 0f;
            pl.dashModel.allowDash = false;
        }
    }
    public override void Update(PlayerController pl)
    {

    }
    public override void FixedUpdate(PlayerController pl)
    {
        if (pl.conveyor == null)
            pl.playerRB.velocity = new Vector2(pl.dashModel.dashSpeed, 0) * (int)pl.moveModel.Direction;
        else if (pl.conveyor.going == Conveyor.Direction.left && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Left)
        {
            pl.playerRB.velocity = new Vector2(-1*pl.dashModel.dashSpeed - pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.left && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Right)
        {
            pl.playerRB.velocity = new Vector2(pl.dashModel.dashSpeed - pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.right && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Right)
        {
            pl.playerRB.velocity = new Vector2(pl.dashModel.dashSpeed + pl.conveyor.speed, pl.playerRB.velocity.y);
        }
        else if (pl.conveyor.going == Conveyor.Direction.right && pl.moveModel.Direction == PlayerMoveModel.PlayerDirection.Left)
        {
            pl.playerRB.velocity = new Vector2(-1*pl.dashModel.dashSpeed + pl.conveyor.speed, pl.playerRB.velocity.y);
        }
    }
    public override void ExitState(PlayerController pl)
    {
        pl.playerRB.velocity = Vector2.zero;
        pl.playerRB.gravityScale = 3f;
    }
}

public class PlayerSlidingState : PlayerState
{
    public override void EnterState(PlayerController pl)
    {
        pl.jumpModel.jumpCount = pl.jumpModel.jumpCountMax;
        pl.statevisualizer = PlayerController.state.slide;
        pl.playerRB.gravityScale = pl.slideModel.slideGravity;
        pl.playerRB.velocity = Vector2.zero;
        //pl.slideModel.isSliding = true;
        pl.playerAnim.SetBool("isSlide", true);
        //pl.slideModel.slidingCancelTimer = pl.slideModel.slidingCancelTimerMax;
        pl.dashModel.allowDash = true;
    }
    public override void Update(PlayerController pl)
    {
        /*if(pl.slideModel.slidingCancelTimer > 0f)
            pl.slideModel.slidingCancelTimer -= Time.deltaTime;*/
        //pl.playerRB.gravityScale = Mathf.Lerp(0f, pl.slideModel.normalGravityScale, pl.slideModel.slidingCancelTimer);
    }
    public override void FixedUpdate(PlayerController pl)
    {
        if (pl.conveyor == null || pl.conveyor.going == Conveyor.Direction.right || pl.conveyor.going == Conveyor.Direction.left)
            pl.playerRB.gravityScale = pl.slideModel.slideGravity;
        else if (pl.conveyor.going == Conveyor.Direction.up)
            pl.playerRB.gravityScale = pl.slideModel.slideGravity - pl.conveyor.speed;
        else if (pl.conveyor.going == Conveyor.Direction.down)
            pl.playerRB.gravityScale = pl.slideModel.slideGravity + pl.conveyor.speed;
    }
    public override void ExitState(PlayerController pl)
    {
        //pl.moveModel.HorizontalMovement = 0f;
        pl.playerRB.gravityScale = pl.slideModel.normalGravity;
        pl.playerAnim.SetBool("isSlide", false);

    }
}

public class PlayerWallJumpState : PlayerState
{
    public override void EnterState(PlayerController pl)
    {
        pl.slideModel.slidingCancelTimer = pl.slideModel.slidingCancelTimerMax;
        /*pl.jumpModel.jumpCount--;*/
        pl.statevisualizer = PlayerController.state.wallJump;
        pl.playerRB.velocity = new Vector2((int)pl.moveModel.Direction * -1f * pl.slideModel.slideJumpHorizontalSpeed, pl.jumpModel.jumpSpeed);
        pl.slideModel.wallJumped = true;
    }
    public override void Update(PlayerController pl)
    {
        if (pl.slideModel.slidingCancelTimer < 0f) pl.ChangeState(pl.jumpState);
    }

    public override void FixedUpdate(PlayerController pl)
    {
        
    }

    public override void ExitState(PlayerController pl)
    {
        if (pl.moveModel.HorizontalMovement < 0f)
            pl.moveModel.Direction = PlayerMoveModel.PlayerDirection.Left;
        else if (pl.moveModel.HorizontalMovement > 0f)
            pl.moveModel.Direction = PlayerMoveModel.PlayerDirection.Right;
    }
}

public class PlayerGrappleState : PlayerState
{
    public override void EnterState(PlayerController pl)
    {
        pl.statevisualizer = PlayerController.state.grapple;
        pl.dashModel.allowDash = true;
        pl.jumpModel.jumpCount = pl.jumpModel.jumpCountMax;
        pl.grappleModel.playerLine.enabled = true;
        pl.grappleModel.playerLine.SetPosition(0, pl.transform.position);
        pl.grappleModel.playerLine.SetPosition(1, pl.grappleModel.point.transform.position);
    }
    public override void Update(PlayerController pl)
    {
        pl.grappleModel.playerLine.SetPosition(0, pl.transform.position);
        if (Vector3.Distance(pl.transform.position, pl.grappleModel.point.transform.position) > pl.grappleModel.reachTolerance)
        {
            pl.playerRB.gravityScale = 0f;
        }
        else
        {
            if (pl.jumpModel.isGrounded)
            {
                pl.ChangeState(pl.moveState);
            }
            else
            {
                pl.ChangeState(pl.jumpState);
            }
        }
    }
    public override void FixedUpdate(PlayerController pl)
    {
        Vector3 GrappleDirection = Vector3.Normalize((pl.grappleModel.point.transform.position - pl.transform.position));
        pl.playerRB.velocity = new Vector2(GrappleDirection.x, GrappleDirection.y) * pl.grappleModel.grappleJumpSpeed;
    }
    public override void ExitState(PlayerController pl)
    {
        pl.grappleModel.playerLine.enabled = false;
        pl.playerRB.gravityScale = 3f;
    }
}

