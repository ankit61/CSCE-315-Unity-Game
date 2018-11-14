using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound
{

    enum PlayerState { IdleState, PunchState };

    public class PlayerController : MonoBehaviour
    {
        private Player m_player;
        private bool m_playerAlive = true;

        // Use this for initialfization
        void Start()
        {
            m_player = gameObject.GetComponent<Player>();
        }

        void HandleXMovement()
        {
            if (Input.GetKey(Constants.RIGHT_KEY))
                m_player.Move(Player.Direction.RIGHT);
            else if (Input.GetKey(Constants.LEFT_KEY))
                m_player.Move(Player.Direction.LEFT);

        }

        void HandleMovementAndAction()
        {
        
            if (Input.GetKeyDown(Constants.PUNCH_KEY)) // Punch
            {

                m_player.Punch();
            }
            else if (Input.GetKeyDown(Constants.KICK_KEY)) // Kick
            {
                m_player.Kick();
            }
            else
            {
                HandleXMovement();
            }

            if (Input.GetKeyDown(Constants.ROCK_KEY))
            {
                //m_player.Move(Player.Direction.DOWN);
                m_player.Rock();
            }

            if (Input.GetKeyDown(Constants.JUMP_KEY))
            {
                m_player.Jump();
            }


            /*if (Input.GetKeyDown(Constants.MISSILE_KEY)) //Missile
            {
                m_player.Missile();
            }*/
        }

        void CheckPlayerAlive(){
            if (!m_playerAlive)
                return;
            float thresholdHeight = Camera.main.orthographicSize * 1f + 1;
            float thresholdWidth = Camera.main.aspect * thresholdHeight + 1;
            float curX = m_player.GetComponent<Rigidbody2D>().position.x;
            float curY = m_player.GetComponent<Rigidbody2D>().position.y;
            if( (curX > thresholdWidth) || (curX < -thresholdWidth) || (curY < -thresholdHeight)){
                m_player.Die();
                Debug.Log("Killing Player");
                m_playerAlive = false;
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovementAndAction();
            CheckPlayerAlive();
        }
    }
}