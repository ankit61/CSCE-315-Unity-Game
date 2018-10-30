using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound
{

    enum PlayerState { IdleState, PunchState };

    public class BlobController : MonoBehaviour
    {

        public Animator playerAnimator;
        public float ragdollTimer = 2.0f;
        public float speedLimit = 70.0f;

        private float distanceToGround;
        private Vector2 curVelocity;
        private float ragdollStartTime;
        private Player m_player;

        // Use this for initialfization
        void Start()
        {
            m_player = gameObject.GetComponent<Player>();
            ragdollStartTime = 0.0f;
            playerAnimator = gameObject.GetComponent<Animator>();
            distanceToGround = GetComponent<PolygonCollider2D>().bounds.extents.y;
            
        }

        void HandleXMovement()
        {

            if (Input.GetKey("d"))
                m_player.Move(Player.Direction.RIGHT);
            else if (Input.GetKey("a"))
                m_player.Move(Player.Direction.LEFT);

        }

        void HandleMovementAndAction()
        {
            if (gameObject.GetComponent<Rigidbody2D>().constraints == RigidbodyConstraints2D.None)
            {
                HandleXMovement();
                if ((Time.time - ragdollStartTime) >= ragdollTimer)
                {
                    transform.rotation = Quaternion.identity;
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                return;
            }

            if (Input.GetKeyDown("j")) // Punch
            {
                m_player.Punch();
                //playerAnimator.SetInteger("Animation State", 1);
            }
            else if (Input.GetKeyDown("k")) // Kick
            {
                m_player.Kick();
            }
            else
            {
                HandleXMovement();
            }

            if (Input.GetKeyDown("s"))
            {
                m_player.Move(Player.Direction.DOWN);
            }

            if (Input.GetKeyDown("space"))
            {
                m_player.Jump();
            }

            if (Input.GetKeyDown("r"))
            {
                gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                ragdollStartTime = Time.time;
            }
        }

        void GetPlayerState()
        {
            int curState = playerAnimator.GetInteger("Animation State");
            //Debug.Log("Player is in state: " + ((PlayerState)curState).ToString());
        }

        bool IsGrounded()
        {
            bool val = Physics.Raycast(transform.position, Vector2.down, distanceToGround + 0.1f);
            //Debug.Log("Standing : " + val.ToString());
            return val;
        }

        // Update is called once per frame
        void Update()
        {
            curVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;
            GetPlayerState();
            HandleMovementAndAction();
        }
    }
}