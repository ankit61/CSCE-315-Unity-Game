using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound
{

    enum PlayerState { IdleState, PunchState };

    public class PlayerController : MonoBehaviour
    {
        private Player m_player;

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

            if (Input.GetKeyDown(Constants.DOWN_KEY))
            {
                m_player.Move(Player.Direction.DOWN);
            }

            if (Input.GetKeyDown(Constants.JUMP_KEY))
            {
                m_player.Jump();
            }
        }

        // Update is called once per frame
        void Update()
        {
            HandleMovementAndAction();
        }
    }
}