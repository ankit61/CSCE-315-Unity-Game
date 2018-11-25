using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound
{

    public class PlayerInfoPanel : MonoBehaviour
    {
        public List<GameObject> m_infoObjs;
        private List<PlayerInfo> m_playerInfos = new List<PlayerInfo>();
        // Use this for initialization
        void Awake()
        {
            for(int i = 0; i < m_infoObjs.Count; i++)
            {
                m_playerInfos.Add(m_infoObjs[i].GetComponent<PlayerInfo>());
            }
        }

        public PlayerInfo InitializePlayerInfo(int _playerSlot, string _username, string _spritePath)
        {
            m_playerInfos[_playerSlot].InitializePlayerInfo(_username, _spritePath);
            return m_playerInfos[_playerSlot];
        }

        public PlayerInfo InitializeOpponentInfo(int _playerSlot, string _username, string _spritePath)
        {
            m_playerInfos[_playerSlot].InitializeOpponentInfo(_username, _spritePath);
            return m_playerInfos[_playerSlot];
        }

        public void KillUser(int _playerSlot)
        {
            m_playerInfos[_playerSlot].Die();
        }
    }
}
