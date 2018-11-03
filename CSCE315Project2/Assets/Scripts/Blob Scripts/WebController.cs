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
            m_player.GetType().GetMethod(_data.action).Invoke(m_player, null);
        }

        public void Correct(Vector2 _pos, Vector2 _vel) {
            

        }
    }
}
