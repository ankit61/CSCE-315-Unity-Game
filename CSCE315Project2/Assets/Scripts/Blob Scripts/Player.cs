using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rebound
{

    public class Player : MonoBehaviour
    {

        public enum Direction { LEFT, RIGHT, DOWN, UP }

        public enum State { IDLE, MOVING, JUMPING, PUNCHING, KICKING, RAGDOLLING }

        private State m_currentState;

        private Animator m_animator;

        private bool m_isFacingLeft = false;

        private bool m_inAir = false;

        private float m_stateStartTime;

        private Dictionary<Player.State, float> m_STATE_TIMES = new Dictionary<Player.State, float>() {
                                                            {Player.State.PUNCHING, 1.0f},
                                                            {Player.State.KICKING, 1.0f},
                                                        };

        private Dictionary<Player.State, HashSet<State> > m_STATE_TRANSITIONS = new Dictionary<State, HashSet<State>>();

        void Awake()
        {
            m_animator = gameObject.GetComponent<Animator>();

            m_STATE_TRANSITIONS[State.IDLE] = new HashSet<State>()
            {
                State.JUMPING, State.KICKING, State.MOVING, State.PUNCHING, State.RAGDOLLING, State.IDLE
            };

            m_STATE_TRANSITIONS[State.MOVING] = new HashSet<State>()
            {
                State.MOVING, State.JUMPING, State.KICKING, State.PUNCHING, State.IDLE, State.RAGDOLLING
            };

            m_STATE_TRANSITIONS[State.PUNCHING] = new HashSet<State>()
            {
                State.IDLE
            };

            m_STATE_TRANSITIONS[State.KICKING] = new HashSet<State>()
            {
                State.IDLE
            };

            m_STATE_TRANSITIONS[State.JUMPING] = new HashSet<State>()
            {
                State.KICKING, State.PUNCHING, State.IDLE, State.RAGDOLLING, State.MOVING
            };

            m_STATE_TRANSITIONS[State.RAGDOLLING] = new HashSet<State>()
            {
                State.IDLE
            };
        }

        public void Move(Direction _direction)
        {
            if (!ChangeState(State.MOVING))
                return;

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

        public void Jump()
        {
            if (!ChangeState(State.JUMPING) || m_inAir)
                return;
            
            AddVelocity(new Vector2(0, Constants.JUMP_SPEED));
        }

        void OnCollisionExit(Collision other)
        {
            m_inAir = true;
            Debug.Log("Exiting collisions");
        }

        void OnCollisionStay(Collision other)
        {
            m_inAir = false;
            Debug.Log("In collisions");
        }

        void OnCollisionEnter(Collision other)
        {
            m_inAir = false;
            Debug.Log("Entering collisions");
        }

        public void Punch()
        {
            if (!ChangeState(State.PUNCHING))
                return;
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

            if (!ChangeState(State.KICKING))
                return;
        }

        private void Draw()
        {

            m_animator.SetInteger("Animation State", Constants.EMPTY_STATE_CODE);

            switch (m_currentState)
            {
                case State.IDLE:
                    m_animator.enabled = true;
                    Destroy(gameObject.GetComponent<PolygonCollider2D>());
                    gameObject.AddComponent<PolygonCollider2D>();
                    m_animator.SetInteger("Animation State", Constants.IDLE_STATE_CODE);
                    break;
                case State.MOVING:
                    m_animator.enabled = true;
                    //m_animator.SetInteger("Animation State", Constants.EMPTY_STATE_CODE);
                    break;
                case State.JUMPING:
                    break;
                case State.PUNCHING:
                    m_animator.enabled = false;
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Constants.PUNCH_SPRITE_PATH);
                    Destroy(gameObject.GetComponent<PolygonCollider2D>());
                    gameObject.AddComponent<PolygonCollider2D>();
                    gameObject.GetComponent<Rigidbody2D>().mass = Constants.PUNCH_MASS;
                    break;
                case State.KICKING:
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
            if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude == 0)
                ChangeState(State.IDLE);

            if (gameObject.GetComponent<Rigidbody2D>().velocity.y != 0)
                ChangeState(State.JUMPING);
        }

        private bool ChangeState()
        {
            return ChangeState(State.IDLE);
        }

        private bool ChangeState(State _state)
        {
            if (!m_STATE_TRANSITIONS[m_currentState].Contains(_state))
                return false;
            m_currentState = _state;
            if (m_STATE_TIMES.ContainsKey(_state))
            {                
                Invoke("ChangeState", m_STATE_TIMES[_state]);
            }
            return true;
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

        }

        void Update() {
            ManageState();
            Draw();
            //Debug.Log(m_inAir);
            //Debug.Log(m_currentState);
        }

    }

}