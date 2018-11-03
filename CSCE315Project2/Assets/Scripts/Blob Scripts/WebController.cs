﻿using System.Collections;
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

        public void Act(ServerData _data) {
            Correct(_data.position, _data.velocity);
            if(_data.action != null)
                m_player.GetType().GetMethod(_data.action).Invoke(m_player, null);
        }

        public void UpdateTransform(PlayerInfo _data)
        {
            m_player.SetPosition(_data.position);
            gameObject.GetComponent<Rigidbody2D>().velocity = _data.velocity;
        }

        public void Correct(Vector2 _pos, Vector2 _vel) {
            if((m_player.GetPosition() - _pos).magnitude < Constants.CORRECTION_THRESHOLD)
                m_player.SetPosition(_pos);

            //FIXME: use state and veclocity

        }
    }
}
