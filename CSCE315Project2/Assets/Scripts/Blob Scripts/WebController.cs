using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
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
            Correct(_data.position, _data.velocity);
            if(_data.action != "null") 
            {
                if(_data.action.Substring(0, 4) == "Move")                
                    m_player.SendMessage(_data.action.Substring(0, 4), (Player.Direction)Enum.Parse(typeof(Player.Direction), _data.action.Substring(4, _data.action.Length - 4)));
                else
                    m_player.SendMessage(_data.action, 0);
            }
        }

        public void Correct(Vector2 _pos, Vector2 _vel) {
            if((m_player.GetPosition() - _pos).magnitude > Constants.CORRECTION_THRESHOLD) {
                Debug.Log("Correcting");
                m_player.SetPosition(_pos);
            }

            //FIXME: use state and veclocity

        }
    }
}
