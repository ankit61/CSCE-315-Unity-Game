using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rebound
{

    public class Player : MonoBehaviour
    {

        public enum Direction { LEFT, RIGHT, DOWN, UP }

        public enum State { IDLE, MOVING, JUMPING, PUNCHING, KICKING, FALLING, RAGDOLLING }

        private State m_currentState;

        private Animator m_animator;

        private bool m_isFacingLeft = false;

        private float m_stateStartTime;

        public void Move(Direction _direction)
        {
            ChangeState(State.MOVING);
            switch (_direction)
            {
                case Direction.LEFT:
                    AddVelocity(new Vector2(-Constants.MOVEMENT_SPEED, 0));
                    break;
                case Direction.RIGHT:
                    AddVelocity(new Vector2(Constants.MOVEMENT_SPEED, 0));
                    break;
                case Direction.UP:
                    Jump();
                    return;
                case Direction.DOWN:
                    AddVelocity(new Vector2(0, -Constants.MOVEMENT_SPEED));
                    break;
                default:
                    throw new ArgumentException("Invalid direction", "_direction");
            }
        }

        public Dictionary<Player.State, float> STATE_TIMES = new Dictionary<Player.State, float>() {
                                                            {Player.State.PUNCHING, 1.0f},
                                                            {Player.State.KICKING, 1.0f},
                                                        };

        public void Jump()
        {
            if (m_currentState == State.JUMPING)
                return;
            ChangeState(State.JUMPING);
            AddVelocity(new Vector2(0, Constants.JUMP_SPEED));
        }

        public void Punch()
        {
            ChangeState(State.PUNCHING);
            float sign = Math.Sign(gameObject.GetComponent<Rigidbody2D>().velocity.x);
            if (sign == 0) {
                if (m_isFacingLeft)
                    sign--;
                else
                    sign++;
            }
            AddVelocity(new Vector2(sign * Constants.PUNCH_SPEED, 0));
        }

        public void Kick()
        {

            ChangeState(State.KICKING);
        }

        private void Draw()
        {
            //if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
            //    m_currentState = State.IDLE;

            //Debug.Log(m_currentState);

            m_animator.SetInteger("Animation State", Constants.EMPTY_STATE_CODE);

            switch (m_currentState)
            {
                case State.IDLE:
                    m_animator.enabled = true;
                    m_animator.SetInteger("Animation State", Constants.IDLE_STATE_CODE);
                    break;
                case State.MOVING:
                    m_animator.enabled = true;
                    m_animator.SetInteger("Animation State", Constants.EMPTY_STATE_CODE);
                    break;
                case State.JUMPING:
                    break;
                case State.PUNCHING:
                    m_animator.enabled = false;
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Constants.PUNCH_SPRITE_PATH);
                    //Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite);
                    break;
                case State.KICKING:
                    break;
                case State.FALLING:
                    break;
                case State.RAGDOLLING:
                    break;
                default:
                    break;
            }

            if(gameObject.GetComponent<Rigidbody2D>().velocity.x != 0.0f)
                m_isFacingLeft = gameObject.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<Rigidbody2D>().velocity.x < 0.0f;

        }

        private void ManageState()
        {
            if (STATE_TIMES.ContainsKey(m_currentState) && (Time.time - m_stateStartTime) >= STATE_TIMES[m_currentState])
                ChangeState(State.MOVING);
        }

        private void ChangeState(State _state)
        {
            m_currentState = _state;
            m_stateStartTime = Time.time;
        }

        private void AddVelocity(Vector2 _velocity)
        {
            float vx = gameObject.GetComponent<Rigidbody2D>().velocity.x;
            float vy = gameObject.GetComponent<Rigidbody2D>().velocity.y;

            float limit = 0;

            switch(m_currentState)
            {
                case State.JUMPING:
                    limit = Constants.JUMP_SPEED;
                    break;
                case State.MOVING:
                    limit = Constants.MOVEMENT_SPEED;
                    break;
                case State.IDLE:
                case State.PUNCHING:
                case State.KICKING:
                default:
                    limit = Constants.SPEED_LIMIT;
                    break;
            }

            vx += _velocity.x;
            vy += _velocity.y;
            vx = Math.Sign(vx) * Math.Min(limit, Math.Abs(vx));
            vy = Math.Sign(vy) * Math.Min(limit, Math.Abs(vy));
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(vx, vy);
        }

        void Start()
        {
            ChangeState(State.IDLE);
            m_animator = gameObject.GetComponent<Animator>();
        }

        void Update() {
            ManageState();
            Draw();
        }

    }

}