using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rebound
{

    public static class SharedData : object
    {

        private static string m_roomID = "00000000";

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

        private static string m_username;

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