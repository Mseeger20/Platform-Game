using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Diagnostics;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum state
    {
        move,
        jump,
        dash,
        slide,
        wallJump,
        grapple,
    }
    public state statevisualizer;

    public PlayerMoveModel moveModel = new PlayerMoveModel();
    public PlayerJumpModel jumpModel = new PlayerJumpModel();
    public PlayerDashModel dashModel = new PlayerDashModel();
    public PlayerSlideModel slideModel = new PlayerSlideModel();
    public PlayerGrappleModel grappleModel = new PlayerGrappleModel();

    public PlayerState generalState;
    public PlayerMoveState moveState = new PlayerMoveState();
    public PlayerJumpState jumpState = new PlayerJumpState();
    public PlayerDashState dashState = new PlayerDashState();
    public PlayerSlidingState slideState = new PlayerSlidingState();
    public PlayerWallJumpState wallJumpState = new PlayerWallJumpState();
    public PlayerGrappleState grappleState = new PlayerGrappleState();

    [Header("Components")]
    public Rigidbody2D playerRB;
    public Animator playerAnim;

    private int groundHash;
    private int jumpVelHash;
    private int extraJumpTriggerHash;

    [Header("LayerMasks")]
    private LayerMask groundMask;

    //Vars for development (should not be serializing after build)
    /*[SerializeField] */
    private float groundHitDis = 0.28f;
    /*[SerializeField] */
    private float slideHitDis = 0.1f;
    [SerializeField] private float slideAllowHitDis = 1f;
    /*[SerializeField] */

    public Conveyor conveyor = null;
    DataTracker dt;
    GameObject EndScreen;

    TMP_Text textonscreen;
    public int unlocks = 0;
    int totalunlocks;

    public GameObject spawn;
    bool teleport = true;
    float defaultspeed;

    Stopwatch stopwatch;
    int deaths = 0;

    // Start is called before the first frame update
    void Start()
    {
        statevisualizer = state.move;
        spawn = GameObject.Find("Spawn");
        defaultspeed = this.moveModel.hspeed;

        stopwatch = new Stopwatch();
        stopwatch.Start();

        EndScreen = GameObject.Find("EndScreen");
        EndScreen.gameObject.SetActive(false);

        totalunlocks = GameObject.Find("Unlockables").transform.childCount;
        textonscreen = GameObject.Find("Texties").GetComponent<TMP_Text>();

        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        grappleModel.playerLine = GetComponent<LineRenderer>();
        grappleModel.playerLine.enabled = false;
        grappleModel.playerLine.SetPosition(0, transform.position);

        grappleModel.GrappleSensor = transform.Find("GrappleSensor").GetComponent<GrappleArea>();

        groundHash = Animator.StringToHash("isGrounded");
        extraJumpTriggerHash = Animator.StringToHash("ExJumpTrigger");
        jumpVelHash = Animator.StringToHash("jumpVelocity");

        groundMask = LayerMask.GetMask("Ground");
        dt = FindObjectOfType<DataTracker>().GetComponent<DataTracker>();
        ChangeState(moveState);
    }


    public void ChangeState(PlayerState newState)
    {
        if (generalState != null)
        {
            generalState.ExitState(this);
        }
        generalState = newState;
        if (generalState != null)
        {
            generalState.EnterState(this);
        }
    }

    string ConvertTimeToString(TimeSpan x)
    {
        string part = x.Seconds < 10 ? $"0{x.Seconds}" : $"{x.Seconds}";
        return $"{x.Minutes}:" + part + $".{x.Milliseconds}";
    }

    // Update is called once per frame
    void Update()
    {
        textonscreen.text = "Time: " + ConvertTimeToString(stopwatch.Elapsed) +
            $"\nDeaths: {deaths} / {dt.totaldeaths}" +
            $"\nCollectibles: {unlocks} / {totalunlocks}";

        playerAnim.SetBool(groundHash, jumpModel.isGrounded);
        playerAnim.SetFloat(jumpVelHash, playerRB.velocity.y);
        generalState.Update(this);
        checkGround();
        checkSlide();
        flip();
        //reset Platform Collision========================
        if (jumpModel.platformTimer > 0f)
        {
            jumpModel.platformTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.layer = 6;
        }

        //Timers
        if (slideModel.slidingCancelTimer >= 0) slideModel.slidingCancelTimer -= Time.deltaTime;
    }

    public void GotCollectible()
    {
        unlocks++;
    }

    void OnMove(InputValue input)
    {
        /*if (generalState != dashState)*/
        moveModel.HorizontalMovement = input.Get<Vector2>().x;
        if (generalState != slideState)
        {
            if (moveModel.HorizontalMovement < 0f)
                moveModel.Direction = PlayerMoveModel.PlayerDirection.Left;
            else if (moveModel.HorizontalMovement > 0f)
                moveModel.Direction = PlayerMoveModel.PlayerDirection.Right;
        }
        moveModel.VerticalMovement = input.Get<Vector2>().y;
    }

    void OnJump()
    {
        if (generalState != jumpState && jumpModel.jumpCount > 0 && generalState != slideState && generalState != wallJumpState)
        {
            gameObject.layer = 6;
            if (moveModel.VerticalMovement >= 0f)
            {
                jumpModel.jumpCount -= 1;
                playerRB.velocity = Vector2.up * jumpModel.jumpSpeed;
                ChangeState(jumpState);

            }
            else
            {
                if (!jumpModel.isPlatform)
                {
                    jumpModel.jumpCount -= 1;
                    playerRB.velocity = Vector2.up * jumpModel.jumpSpeed;
                    ChangeState(jumpState);
                }
                else
                {
                    if (jumpModel.platformTimer <= 0f)
                    {
                        ChangeState(jumpState);
                        gameObject.layer = 7;
                        jumpModel.platformTimer = jumpModel.platformMaxTimer;
                    }
                }
            }
        }
        else if (generalState == slideState)
        {
            jumpModel.jumpCount -= 1;
            ChangeState(wallJumpState);
        }
        else
        {
            if (jumpModel.jumpCount > 0)
            {
                playerRB.velocity = Vector2.up * jumpModel.jumpSpeed;
                jumpModel.jumpCount -= 1;
                playerAnim.SetTrigger(extraJumpTriggerHash);

            }
        }
    }

    void OnDash()
    {
        if (generalState != dashState && dashModel.allowDash && generalState != slideState)
        {
            ChangeState(dashState);
        }
    }

    void OnQuit()
    {
        Application.Quit();
    }

    void OnGrapple()
    {
        Vector3 grappleDir = (grappleModel.point.transform.position - transform.position).normalized;
        float length = Vector3.Distance(transform.position, grappleModel.point.transform.position);
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, grappleDir, length, groundMask);
        if (hitGround.collider == null && generalState != dashState && grappleModel.point.activeSelf)
        {
            GameObject target = grappleModel.GrappleSensor.closestGrapplePoint;
            if (target != null) StartCoroutine(disableGrapplePoint(target));
            ChangeState(grappleState);
        }
    }

    void flip()
    {
        transform.localScale = new Vector3((float)moveModel.Direction, transform.localScale.y, transform.localScale.z);
    }

    void checkGround()
    {
        RaycastHit2D hitJump = Physics2D.Raycast(transform.position + moveModel.groundSensor.transform.localPosition, Vector2.down, groundHitDis, groundMask);
        if (hitJump.collider != null)
        {
            jumpModel.isGrounded = true;
            if (hitJump.collider.CompareTag("Platform"))
                jumpModel.isPlatform = true;
            dashModel.allowDash = true;
            if (playerRB.gravityScale != slideModel.normalGravity)
                playerRB.gravityScale = slideModel.normalGravity;
            if (playerRB.velocity.y < 0f && generalState != dashState && generalState != moveState && generalState != grappleState)
            {
                ChangeState(moveState);
                jumpModel.jumpCount = jumpModel.jumpCountMax;
            }
        }
        else
        {
            jumpModel.isGrounded = false;
        }

        RaycastHit2D hitSlide = Physics2D.Raycast(transform.position + moveModel.groundSensor.transform.localPosition, Vector2.down, slideAllowHitDis, groundMask);
        if (hitSlide.collider != null)
        {
            slideModel.canSlide = false;
        }
        else
        {
            slideModel.canSlide = true;
        }
    }

    void checkSlide()
    {
        Vector3 HitSlideStart = transform.position +
            new Vector3(slideModel.slideDetectPos.transform.localPosition.x * (int)moveModel.Direction,
            slideModel.slideDetectPos.transform.localPosition.y,
            0);
        RaycastHit2D hit = Physics2D.Raycast(HitSlideStart,
            new Vector2((int)moveModel.Direction, 0), slideHitDis, groundMask);
        if (hit.collider != null && hit.collider.CompareTag("Slidable") && slideModel.canSlide && slideModel.slidingCancelTimer <= 0f && generalState != slideState)
        {
            ChangeState(slideState);
        }
        else if (hit.collider == null && generalState != jumpState && generalState != dashState && generalState != wallJumpState && generalState != grappleState)
        {
            ChangeState(moveState);
        }
    }

    IEnumerator disableGrapplePoint(GameObject point)
    {
        point.SetActive(false);
        yield return new WaitForSecondsRealtime(grappleModel.grapplePointExcludeCD);
        point.SetActive(true);
    }

    void FixedUpdate()
    {
        generalState.FixedUpdate(this);

        if (teleport)
        {
            gameObject.transform.position = spawn.transform.position;
            moveModel.hspeed = 0;
            playerRB.velocity = new Vector2(0, 0);
            teleport = false;
            ChangeState(moveState);
            StartCoroutine(Delay());
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.7f);
        moveModel.hspeed = defaultspeed;
    }

    public void LevelEnded()
    {
        stopwatch.Stop();
        dt.timetaken += stopwatch.Elapsed;
        EndScreen.SetActive(true);

        this.GetComponent<SpawnGrapplePoint>().enabled = false;
        this.GetComponent<PlayerInput>().enabled = false;
        ChangeState(moveState);

        int x = SceneManager.GetActiveScene().buildIndex;

        if (dt.levelcomplete[x] == false)
        {
            dt.levelcomplete[x] = true;
            dt.recordsno100[x] = stopwatch.Elapsed;
        }
        else if (unlocks < totalunlocks && dt.recordsno100[x] > stopwatch.Elapsed)
        {
            dt.recordsno100[x] = stopwatch.Elapsed;
            EndScreen.transform.GetChild(1).GetComponent<TMP_Text>().text += "You beat your previous time!\n";
        }
        if (unlocks >= totalunlocks && dt.allcollectibles[x] == false)
        {
            dt.allcollectibles[x] = true;
            dt.records100[x] = stopwatch.Elapsed;
            EndScreen.transform.GetChild(1).GetComponent<TMP_Text>().text += "You got all the collectibles!\n";
        }
        else if (unlocks >= totalunlocks && dt.records100[x] > stopwatch.Elapsed)
        {
            EndScreen.transform.GetChild(1).GetComponent<TMP_Text>().text += "You got all the collectibles!\n";
            EndScreen.transform.GetChild(1).GetComponent<TMP_Text>().text += "You beat your previous time!\n";
            dt.records100[x] = stopwatch.Elapsed;
            if (dt.recordsno100[x] > stopwatch.Elapsed)
                dt.recordsno100[x] = stopwatch.Elapsed;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Conveyor"))
        {
            conveyor = collision.gameObject.GetComponent<Conveyor>();
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            teleport = true;
            deaths++;
            dt.totaldeaths++;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Conveyor"))
        {
            conveyor = null;
        }
    }
}
