using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerMoveModel
{
    public float HorizontalMovement;
    public float VerticalMovement;
    public float hspeed;

    public enum PlayerDirection
    {
        Left = -1, 
        Right = 1,
    }
    public PlayerDirection Direction;
    public GameObject groundSensor;
}

[System.Serializable]
public class PlayerJumpModel
{
    public float jumpSpeed;
    public float smallJumpSpeed;
    public int jumpCount;
    public int jumpCountMax;
    public bool isGrounded;
    public bool isPlatform;
    public float platformTimer;
    public float platformMaxTimer = 1f;
}

[System.Serializable]
public class PlayerDashModel
{
    public float dashSpeed;
    public bool allowDash = true;
    
}

[System.Serializable]
public class PlayerSlideModel
{
    public GameObject slideDetectPos;
    public float slideJumpHorizontalSpeed;
    public float slidingCancelTimer;
    public float slidingCancelTimerMax;
    public bool canSlide;
    public float normalGravity = 3f;
    public float slideGravity = 0.1f;
    public bool wallJumped = false;
}

[System.Serializable]
public class PlayerGrappleModel
{
    public GameObject point;
    public float reachTolerance;
    public float grappleJumpSpeed;

    public float grapplePointExcludeCD;
    public GrappleArea GrappleSensor;
    public LineRenderer playerLine;
}
