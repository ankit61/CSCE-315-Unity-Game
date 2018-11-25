using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Rebound
{
    public class WebController : MonoBehaviour
    {

        private Player m_player;
        private bool m_playerAlive;

        void Awake() {
            m_player = gameObject.GetComponent<Player>();
            m_playerAlive = true;
        }

        public void Act(BroadcastPayload _data)
        {
            Correct(_data.position, _data.velocity);
            m_player.SendMessage(_data.action, 0);
        }

        public void UpdateTransform(BroadcastPayload _data)
        {
            m_player.SetPosition(_data.position);
            gameObject.GetComponent<Rigidbody2D>().velocity = _data.velocity;
        }

        public void Correct(Vector2 _pos, Vector2 _vel) {
            if((m_player.GetPosition() - _pos).magnitude < Constants.CORRECTION_THRESHOLD)
                m_player.SetPosition(_pos);
            //FIXME: use state and veclocity
        }

        public void KillPlayer(){
            m_player.Die();
            Debug.Log("Killing Player");
            m_player.gameObject.SetActive(false);
            m_playerAlive = false;
        }
    }
}
