using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState {IdleState, PunchState};

public class BlobController : MonoBehaviour {

    public Animator playerAnimator;
    public float ragdollTimer = 2.0f;
    public float speedLimit = 70.0f;

    private float distanceToGround;
    private Vector2 curVelocity;
    private float ragdollStartTime;

	// Use this for initialization
	void Start () {
        ragdollStartTime = 0.0f;
        playerAnimator = gameObject.GetComponent<Animator>();
        distanceToGround = GetComponent<PolygonCollider2D>().bounds.extents.y;
    }

    void HandleXMovement(){

        if (Input.GetKey("d"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(10.0f, curVelocity.y);
        }
        else if (Input.GetKey("a"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-10.0f, curVelocity.y);
        }
    }

    void HandleMovementAndAction()
    {
        if(gameObject.GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.None){
            HandleXMovement();
            if((Time.time - ragdollStartTime) >= ragdollTimer){
                transform.rotation = Quaternion.identity;
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            return;
        }

        if (Input.GetKeyDown("j")) // Punch
        {
            float punchVelocity = 0.0f;
            if (Input.GetKey("a"))
            {
                punchVelocity = -40.0f;
            }
            else if (Input.GetKey("d"))
            {
                punchVelocity = 40.0f;
            }
            else
            {
                punchVelocity = (curVelocity.x >= 0.0f) ? 40.0f : -40.0f;
            }
            playerAnimator.SetInteger("Animation State", 1);
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(punchVelocity, curVelocity.y);
        }
        else if (Input.GetKeyDown("k")) // Kick
        {
            gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(100000.0f, 0.0f));
        }
        else
        {
            HandleXMovement();
        }

        if (Input.GetKey("s"))
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 3.0f;
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
        }

        if (Input.GetKeyDown("space"))
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(curVelocity.x, 10.0f);
        }

        if (Input.GetKeyDown("r"))
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            ragdollStartTime = Time.time;
        }
    }

    void HandleMovementDirection()
    {
        Vector2 curVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        if (curVelocity.x >= 0.0f){
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    void GetPlayerState(){
        int curState = playerAnimator.GetInteger("Animation State");
        //Debug.Log("Player is in state: " + ((PlayerState)curState).ToString());
    }

    bool IsGrounded(){
        bool val = Physics.Raycast(transform.position, Vector2.down, distanceToGround + 0.1f);
        Debug.Log("Standing : " + val.ToString());
        return val;
    }

    void CheckSpeedLimit(){
        float xSpeed = (curVelocity.x > speedLimit) ? speedLimit : curVelocity.x;
        float ySpeed = (curVelocity.y > speedLimit) ? speedLimit : curVelocity.y;
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(xSpeed, ySpeed);
    }

// Update is called once per frame
    void Update()
    {
        curVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        CheckSpeedLimit();
        GetPlayerState();
        HandleMovementAndAction();
        HandleMovementDirection();
    }
}
