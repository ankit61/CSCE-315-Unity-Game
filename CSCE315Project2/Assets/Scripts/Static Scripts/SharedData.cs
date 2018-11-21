using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound
{

    public static class SharedData : object
    {

        private static string m_roomID = Constants.DEFAULT_ROOM_ID;

        public static string RoomID
        {
            get
            {
                return m_roomID;
            }
            set
            {
                m_roomID = value;
            }
        }

        private static string m_username = Constants.PLAYER_USERNAME;

        public static string Username
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }

    }
}