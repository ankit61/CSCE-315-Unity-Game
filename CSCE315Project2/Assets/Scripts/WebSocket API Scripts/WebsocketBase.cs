using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rebound;
using SimpleJSON;

namespace Rebound
{

    public class ConnectReply
    {
        public long newuser = 0;
    }

    public class UpdateReply
    {
        public long sockethash = 0;
        public BroadcastPayload data;
    }

    public class ServerUpdatePayload
    {
        public List<string> action = new List<string> { "action" };
        public BroadcastPayload data;
    }

    public class WebsocketBase : MonoBehaviour
    {

        private WebSocket m_socket;
        private long m_curHash = 0;
        private int m_numSpawned = 0;

        public GameObject m_curPlayer;

        // Test Server IP : 206.189.214.224
        // Server IP : 206.189.78.132
        // Use this for initialization
        IEnumerator Start()
        {
            m_socket = new WebSocket(new Uri("ws://206.189.214.224:8080/AAAAA"));
            yield return StartCoroutine(m_socket.Connect());
            string connectStr = "{\"action\" : [], \"data\" : {} }";
            m_socket.SendString(connectStr);

            StartCoroutine(StartListener());
            StartCoroutine(StartServerUpdator());
        }

        IEnumerator StartListener()
        {
            while (true)
            {
                string reply = m_socket.RecvString();
                if (reply != null)
                {
                    ConnectReply connectReply = JsonUtility.FromJson<ConnectReply>(reply);
                    UpdateReply updateReply = JsonUtility.FromJson<UpdateReply>(reply);
                    var replyJSON = JSON.Parse(reply);
                    if (connectReply.newuser != 0)
                    {
                        Debug.Log(reply);
                        if(m_curHash == 0){ // When the player has not been initialized
                            Debug.Log("Connected to server!");
                            m_curHash = connectReply.newuser;

                            // Instantiate Player ---- TODO --- This should be moved to after getting a 'Connected' from the server
                            GameObject player = InstantiatePlayer("Player", "Player");
                            m_curPlayer = player;
                        }
                        else{
                            InstantiatePlayer(connectReply.newuser.ToString(), "Enemy");
                        }
                    }
                    if ((updateReply.sockethash != 0) && (updateReply.sockethash != m_curHash)){
                        BroadcastPayload data = new BroadcastPayload
                        {
                            position = new Vector2(replyJSON["data"]["position"]["x"].AsFloat, replyJSON["data"]["position"]["y"].AsFloat),
                            velocity = new Vector2(replyJSON["data"]["velocity"]["x"].AsFloat, replyJSON["data"]["velocity"]["y"].AsFloat),
                            state = (Player.State)replyJSON["data"]["state"].AsInt,
                            action = replyJSON["data"]["action"]
                        };
                        GameObject player = GameObject.Find(updateReply.sockethash.ToString());
                        if (player == null){
                            player = InstantiatePlayer(updateReply.sockethash.ToString(), "Enemy");
                        }
                        if(data.action != "null"){
                            Debug.Log("Got action broadcast : " + data.action);
                            player.GetComponent<WebController>().Act(data);
                        }
                        else
                        {
                            player.GetComponent<WebController>().UpdateTransform(data);
                        }
                    }
                    if (replyJSON["deaduser"] != null){
                        Debug.Log(reply);
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
            while (true)
            {
                if ((Time.frameCount % Constants.UPDATE_FREQUENCY) == 0)
                {
                    BroadcastPayload payloadData = m_curPlayer.GetComponent<Player>().GetInfo();
                    string payloadJSON = "{ \"action\" : [\"action\"], \"data\" : " + JsonUtility.ToJson(payloadData) + "}";
                    m_socket.SendString(payloadJSON);
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

        public IEnumerator BroadcastAction(string actionID = null)
        {
            BroadcastPayload payloadData = m_curPlayer.GetComponent<Player>().GetInfo();
            payloadData.action = actionID;
            Debug.Log("Sending Action : " + actionID);
            string payloadJSON = "{ \"action\" : [\"action\"], \"data\" : " + JsonUtility.ToJson(payloadData) + "}";
            m_socket.SendString(payloadJSON);
            yield return 0;
        }

        IEnumerator CloseConnection()
        {
            StopCoroutine(StartListener());
            StopCoroutine(StartServerUpdator());
            m_socket.Close();
            yield return 0;
        }

        private GameObject InstantiatePlayer(string playerName, string playerTag){
            GameObject player = (GameObject)Instantiate(Resources.Load("Character"));
            if (playerTag == "Player")
            {
                player.AddComponent<PlayerController>();
            }
            else{
                player.AddComponent<WebController>();
            }
            player.transform.position = Constants.SPAWN_POINTS[m_numSpawned % Constants.SPAWN_POINTS.Count];
            player.name = playerName;
            player.tag = playerTag;
            player.GetComponent<Player>().m_webAPI = gameObject.GetComponent<WebsocketBase>();
            player.GetComponent<Player>().InitializePlayer(Constants.PLAYER_NAMES[m_numSpawned % Constants.PLAYER_NAMES.Length]);
            m_numSpawned++;
            return player;
        }
    }
}

