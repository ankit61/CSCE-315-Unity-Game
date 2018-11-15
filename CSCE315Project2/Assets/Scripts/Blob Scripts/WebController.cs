using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Rebound
{
    public class WebController : MonoBehaviour
    {

        Player m_player;

        void Awake() {
            m_player = gameObject.GetComponent<Player>();
        }

        public void Act(BroadcastPayload _data)
        {
            m_player.SendMessage(_data.action, 0);
        }

        public void UpdateTransform(BroadcastPayload _data)
        {
            m_player.SetPosition(_data.position);
            gameObject.GetComponent<Rigidbody2D>().velocity = _data.velocity;
        }
    }
}
