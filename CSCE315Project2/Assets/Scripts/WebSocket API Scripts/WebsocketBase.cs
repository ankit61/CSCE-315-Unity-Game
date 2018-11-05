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
        public string method = "";
        public int slot = -1;
        public List<int> players;
    }

    public class UpdateReply
    {
        public string method;
        public long sockethash = 0;
        public BroadcastPayload data;
    }

    public class WebsocketBase : MonoBehaviour
    {

        private WebSocket m_socket;
        private int m_curPlayerSlot = 0; 
        private List<GameObject> playerList;

        public GameObject m_curPlayer = null;

        // Test Server IP : 206.189.214.224
        // Server IP : 206.189.78.132
        // Use this for initialization
        IEnumerator Start()
        {
            playerList = new List<GameObject>();
            for (int i = 0; i < Constants.MAX_PLAYERS; i++){ // Instantiate all players
                GameObject playerObj = (GameObject)Instantiate(Resources.Load("Character"));
                playerObj.SetActive(false);
                playerList.Add(playerObj);
            }
            m_socket = new WebSocket(new Uri("ws://206.189.78.132:80/AAAAA"));
            yield return StartCoroutine(m_socket.Connect());
            string connectStr = "{\"method\" : [], \"data\" : {}}";
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
                    var replyJSON = JSON.Parse(reply);
                    string method = replyJSON["method"];
                    if (method == "joininfo")
                    {
                        m_curPlayerSlot = replyJSON["slot"].AsInt;
                        m_curPlayer = InstantiatePlayer(m_curPlayerSlot, Constants.PLAYER_TAG);
                        var registeredPlayerSlots = replyJSON["players"].AsArray;
                        for (int i = 0; i < registeredPlayerSlots.Count; i++)
                        {
                            int index = registeredPlayerSlots[i];
                            GameObject selectedPlayer = playerList[index];
                            if (selectedPlayer.activeSelf == false)
                            {
                                Debug.Log("Instantiating enemy in slot: " + index.ToString());
                                InstantiatePlayer(index, Constants.ENEMY_TAG);
                            }
                        }
                    }
                    if (method == "newuser"){
                        int playerSlot = replyJSON["slot"].AsInt;
                        InstantiatePlayer(playerSlot, Constants.ENEMY_TAG);
                    }
                    if (method == "action" && (replyJSON["slot"].AsInt != m_curPlayerSlot))
                    {
                        int playerSlot = replyJSON["slot"].AsInt;
                        BroadcastPayload data = new BroadcastPayload
                        {
                            position = new Vector2(replyJSON["data"]["position"]["x"].AsFloat, replyJSON["data"]["position"]["y"].AsFloat),
                            velocity = new Vector2(replyJSON["data"]["velocity"]["x"].AsFloat, replyJSON["data"]["velocity"]["y"].AsFloat),
                            state = (Player.State)replyJSON["data"]["state"].AsInt,
                            action = replyJSON["data"]["action"]
                        };
                        GameObject player = playerList[playerSlot];
                        if (player.activeSelf == false)
                        {
                            player = InstantiatePlayer(playerSlot, Constants.ENEMY_TAG);
                        }
                        if (data.action != "null")
                        {
                            Debug.Log("Got action broadcast : " + data.action);
                            player.GetComponent<WebController>().Act(data);
                        }
                        else
                        {
                            player.GetComponent<WebController>().UpdateTransform(data);
                        }
                    }
                    if(method == "deaduser"){
                        Debug.Log("Got deaduser request");
                        int playerSlot = replyJSON["deaduser"].AsInt;
                        GameObject deadPlayer = playerList[playerSlot];
                        Destroy(deadPlayer);
                        playerList[playerSlot] = (GameObject)Instantiate(Resources.Load("Character"));
                        playerList[playerSlot].SetActive(false);
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
                if(m_curPlayer == null){
                    yield return 0;
                    continue;
                }
                if ((Time.frameCount % Constants.UPDATE_FREQUENCY) == 0)
                {
                    BroadcastPayload payloadData = m_curPlayer.GetComponent<Player>().GetInfo();
                    string payloadJSON = "{ \"method\" : [\"action\"], \"data\" : " + JsonUtility.ToJson(payloadData) + "}";
                    m_socket.SendString(payloadJSON);
                }
                yield return 0;
            }
        }

        IEnumerator PingServer()
        {
            string pingStr = "{\"method\" : [\"ping\"], \"data\" : {} }";
            m_socket.SendString(pingStr);
            yield return 0;
        }

        public IEnumerator BroadcastAction(string actionID = null)
        {
            BroadcastPayload payloadData = m_curPlayer.GetComponent<Player>().GetInfo();
            payloadData.action = actionID;
            Debug.Log("Sending Action : " + actionID);
            string payloadJSON = "{ \"method\" : [\"action\"], \"data\" : " + JsonUtility.ToJson(payloadData) + "}";
            m_socket.SendString(payloadJSON);
            yield return 0;
        }

        public void OnDestroy(){
            StopCoroutine(StartListener());
            StopCoroutine(StartServerUpdator());
            m_socket.Close();
        }

        private GameObject InstantiatePlayer(int playerSlot, string playerTag){
            GameObject player = playerList[playerSlot];
            if (playerTag == Constants.PLAYER_TAG)
            {
                Debug.Log("Instantiating main player");
                player.AddComponent<PlayerController>();
            }
            else{
                player.AddComponent<WebController>();
            }
            player.transform.position = Constants.SPAWN_POINTS[playerSlot];
            player.name = playerSlot.ToString();
            player.tag = playerTag;
            player.GetComponent<Player>().m_webAPI = gameObject.GetComponent<WebsocketBase>();

            //string spriteBase = Constants.PLAYER_NAMES[playerSlot];
            string spriteBase = "Frog";
            string spriteName = spriteBase;
            string animatorName = spriteBase + "_Animation_Controller";
            var sprite = Resources.Load<Sprite>(spriteName);
            player.GetComponent<SpriteRenderer>().sprite = sprite;
            player.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(animatorName);

            player.GetComponent<Player>().InitializePlayer(Constants.PLAYER_NAMES[playerSlot % Constants.PLAYER_NAMES.Length]);
            player.SetActive(true);
            return player;
        }
    }
}

