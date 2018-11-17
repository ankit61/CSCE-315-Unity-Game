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
        void Awake()
        {
            m_player = gameObject.GetComponent<Player>();
        }

        void HandleXMovement()
        {
            if (Input.GetKey(Constants.RIGHT_KEY1) || Input.GetKey(Constants.RIGHT_KEY2))
                m_player.Move(Player.Direction.RIGHT);
            else if (Input.GetKey(Constants.LEFT_KEY1) || Input.GetKey(Constants.LEFT_KEY2))
                m_player.Move(Player.Direction.LEFT);

        }

        void HandleMovementAndAction()
        {
        
            if (Input.GetKeyDown(Constants.PUNCH_KEY)) // Punch
                m_player.Punch();
            else if (Input.GetKeyDown(Constants.KICK_KEY)) // Kick
                m_player.Kick();
            else
                HandleXMovement();

            if (Input.GetKey(Constants.DOWN_KEY1) || Input.GetKey(Constants.DOWN_KEY2))
                m_player.Move(Player.Direction.DOWN);
            else if (Input.GetKeyDown(Constants.MISSILE_KEY1) || Input.GetKeyDown(Constants.MISSILE_KEY2))
                m_player.Missile();
            else if (Input.GetKeyDown(Constants.JUMP_KEY))
                m_player.Jump();

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