using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rebound;

namespace Rebound{

    public class ConnectReply
    {
        public long newuser = 0;
    }

    public class ServerUpdatePayload
    {
        public List<string> action = new List<string>{"action"};
        public PlayerInfo data;
    }

    public class WebsocketBase : MonoBehaviour
    {

        private WebSocket m_socket;

        public int m_sendUpdateFrequency = 10;

        public GameObject m_curPlayer;

        // Server IP : 206.189.78.132
        // Use this for initialization
        IEnumerator Start()
        {
            m_socket = new WebSocket(new Uri("ws://206.189.78.132:8080/AAAAA"));
            yield return StartCoroutine(m_socket.Connect());
            string connectStr = "{\"action\" : [], \"data\" : {} }";
            m_socket.SendString(connectStr);

            // Instantiate Player ---- TODO --- This should be moved to after getting a 'Connected' from the server
            GameObject player = (GameObject)Instantiate(Resources.Load("Character"));
            player.AddComponent<PlayerController>();
            player.transform.position = new Vector2(0.0f, 35.0f);
            m_curPlayer = player;

            StartCoroutine(StartListener());
            //StartCoroutine(StartServerUpdator());
        }

        IEnumerator StartListener()
        {
            while (true)
            {
                string reply = m_socket.RecvString();
                if (reply != null)
                {
                    Debug.Log(reply);
                    ConnectReply connectReply = JsonUtility.FromJson<ConnectReply>(reply);
                    if (connectReply.newuser != 0)
                    {
                        Debug.Log("New user connected!");
                    }
                }
                if (m_socket.error != null)
                {
                    Debug.LogError(m_socket.error);
                }
                yield return 0;
            }
        }

        IEnumerator StartServerUpdator()
        {
            while(true)
            {
                if ((Time.frameCount % m_sendUpdateFrequency) == 0)
                {
                    PlayerInfo playerInfo = m_curPlayer.GetComponent<Player>().GetInfo();
                    //ServerUpdatePayload payload = new ServerUpdatePayload();
                    //payload.data = playerInfo;
                    string payloadJSON = "{ \"action\" : [\"action\"], \"data\" : " + JsonUtility.ToJson(playerInfo) + "}";
                    Debug.Log(payloadJSON);
                    //m_socket.SendString(payloadJSON);
                }
                yield return 0; 
            }
        }

        IEnumerator PingServer()
        {
            string pingStr = "{\"action\" : [\"ping\"], \"data\" : {} }";
            m_socket.SendString(pingStr);
            yield return 0;
        }

        public IEnumerator BroadcastAction(string actionID = "Basic Action")
        {
            string actionJSONStr = "{\"action\" : [\"action\"], \"data\" : { \"actionID\" : \"" + actionID + "\"} }";
            m_socket.SendString(actionJSONStr);
            yield return 0;
        }

        IEnumerator CloseConnection()
        {
            StopCoroutine(StartListener());
            StopCoroutine(StartServerUpdator());
            m_socket.Close();
            yield return 0;
        }
    }


}

