using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Rebound
{

    public struct ColInfo
    {
        public Vector2 velocity;
        public Player.State state;
        public ColInfo(Vector2 _vel, Player.State _state)
        {
            velocity = _vel;
            state = _state;
        }
    }

    public class Player : MonoBehaviour
    {

        public Transform playerCenter, standingTag;

        public enum Direction { LEFT, RIGHT, DOWN, UP }

        public enum State { IDLE, MOVING, JUMPING, PUNCHING, KICKING, RAGDOLLING }

        private State m_currentState; 

        private Animator m_animator;

        private bool m_isFacingLeft = false;

        private bool m_inAir = false;

        private float m_stateStartTime;

        public int m_numOpp = 0;

        private Dictionary<Player.State, float> m_STATE_TIMES = new Dictionary<Player.State, float>() {
                                                            {Player.State.PUNCHING, Constants.PUNCH_TIME},
                                                            {Player.State.KICKING, Constants.KICK_TIME},
                                                            {Player.State.RAGDOLLING, Constants.RAGDOLL_TIME},
                                                        };

        private Dictionary<Player.State, HashSet<State> > m_STATE_TRANSITIONS = new Dictionary<State, HashSet<State>>(); //state transition graph

        void Awake()
        {
            m_animator = gameObject.GetComponent<Animator>();
            gameObject.AddComponent<PolygonCollider2D>();

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
            Debug.Log(m_inAir);
            if (!ChangeState(State.JUMPING) || m_inAir)
                return;
            Debug.Log("here");
            AddVelocity(new Vector2(0, Constants.JUMP_SPEED));
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

            float xDirection = Math.Sign(gameObject.GetComponent<Rigidbody2D>().velocity.x);

            if (xDirection == 0)
            {
                if (m_isFacingLeft)
                    xDirection--;
                else
                    xDirection++;
            }

            AddVelocity(new Vector2(xDirection * Constants.KICK_SPEED, 0));
        }

        public Vector2 GetPosition() {
            return gameObject.GetComponent<Rigidbody2D>().position;
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
                    gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
                    gameObject.GetComponent<Rigidbody2D>().mass = Constants.PLAYER_MASS;
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
                    Destroy(gameObject.GetComponent<PolygonCollider2D>());
                    gameObject.AddComponent<PolygonCollider2D>();
                    gameObject.GetComponent<Rigidbody2D>().mass = Constants.PUNCH_MASS;
                    //Debug.Log(gameObject.GetComponent<SpriteRenderer>().sprite);
                    break;
                case State.KICKING:
                    m_animator.enabled = false;
                    gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Constants.KICK_SPRITE_PATH);
                    Destroy(gameObject.GetComponent<PolygonCollider2D>());
                    gameObject.AddComponent<PolygonCollider2D>();
                    gameObject.GetComponent<Rigidbody2D>().mass = Constants.KICK_MASS;
                    break;
                case State.RAGDOLLING:
                    gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                    break;
                default:
                    break;
            }

        }

        private void ManageState()
        {
            if (!m_inAir && gameObject.GetComponent<Rigidbody2D>().velocity.x == 0)
                ChangeState(State.IDLE);

            if (m_inAir)
                ChangeState(State.JUMPING);
        }

        private bool ChangeState() //treated as a callback
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
            Draw();
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
            gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.GRAVITY_SCALE;
        }

        void Update() 
        {

            Debug.DrawLine(playerCenter.position, standingTag.position);
            m_inAir = !Physics2D.Linecast(playerCenter.position, standingTag.position, 1 << LayerMask.NameToLayer("Solid"));
            ManageState();
            if (gameObject.GetComponent<Rigidbody2D>().velocity.x != 0.0f)
                m_isFacingLeft = gameObject.GetComponent<SpriteRenderer>().flipX = gameObject.GetComponent<Rigidbody2D>().velocity.x < 0.0f;
            Debug.Log(gameObject.tag + ": " + m_currentState);
            Debug.Log(gameObject.GetComponent<Rigidbody2D>().velocity);
            Debug.Log(gameObject.tag + ": " + m_inAir);
        }

        void OnCollisionEnter2D(Collision2D _col) 
        {
            //m_inAir = false;

            if (_col.collider.CompareTag("Enemy") || _col.collider.CompareTag("Player")) {
                _col.collider.SendMessageUpwards("Hit", new ColInfo(gameObject.GetComponent<Rigidbody2D>().velocity, m_currentState));
                if (m_currentState == State.PUNCHING || m_currentState == State.KICKING)
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            }
        }

        public void Hit(ColInfo _colInfo)
        {
            if (_colInfo.state != State.PUNCHING && _colInfo.state != State.KICKING)
                return;

            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(1.5f * _colInfo.velocity.x, 1.5f * _colInfo.velocity.y);
            ChangeState(State.RAGDOLLING);
        }

        public PlayerInfo GetInfo(){
            PlayerInfo curInfo = new PlayerInfo
            {
                velocity = gameObject.GetComponent<Rigidbody2D>().velocity,
                position = gameObject.GetComponent<Rigidbody2D>().position,
                state = m_currentState
            };
            return curInfo;
        }

    }

}