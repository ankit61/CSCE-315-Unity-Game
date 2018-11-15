using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rebound;
using SimpleJSON;

namespace Rebound
{

    public class WebsocketBase : MonoBehaviour
    {

        private WebSocket m_socket;
        private int m_curPlayerSlot = 0; 
        private List<GameObject> m_playerList;

        public GameObject m_curPlayer = null;

        // Test Server IP : 206.189.214.224
        // Server IP : 206.189.78.132
        // Use this for initialization
        IEnumerator Start()
        {
            m_playerList = new List<GameObject>();
            for (int i = 0; i < Constants.MAX_PLAYERS; i++){ // Instantiate all players
                GameObject playerObj = (GameObject)Instantiate(Resources.Load("Character"));
                playerObj.SetActive(false);
                m_playerList.Add(playerObj);
            }
            m_socket = new WebSocket(new Uri("ws://206.189.78.132:80/room/aaaaaaaa"));

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
                    if(method == null){
                        var playerStates = replyJSON.AsArray;
                        for (int i = 0; i < playerStates.Count; i++)
                        {
                            var playerStateJSON = playerStates[i];
                            if ((playerStateJSON == null) || (i == m_curPlayerSlot))
                                continue;
                            GameObject player = m_playerList[i];
                            BroadcastPayload data = new BroadcastPayload
                            {
                                position = new Vector2(playerStateJSON["position"]["x"].AsFloat, playerStateJSON["position"]["y"].AsFloat),
                                velocity = new Vector2(playerStateJSON["velocity"]["x"].AsFloat, playerStateJSON["velocity"]["y"].AsFloat),
                                state = (Player.State)playerStateJSON["state"].AsInt,
                                action = playerStateJSON["action"]
                            }; 
                            player.GetComponent<WebController>().UpdateTransform(data);
                        }
                    }
                    if (method == "joininfo")
                    {
                        m_curPlayerSlot = replyJSON["slot"].AsInt;
                        m_curPlayer = InstantiatePlayer(m_curPlayerSlot, Constants.PLAYER_TAG);
                        var registeredPlayerSlots = replyJSON["players"].AsArray;
                        for (int i = 0; i < registeredPlayerSlots.Count; i++)
                        {
                            int index = registeredPlayerSlots[i];
                            GameObject selectedPlayer = m_playerList[index];
                            if (selectedPlayer.activeSelf == false)
                            {
                                Debug.Log("Instantiating enemy in slot: " + index.ToString());
                                InstantiatePlayer(index, Constants.ENEMY_TAG);
                            }
                        }
                    }
                    if (method == "newuser"){
                        int playerSlot = replyJSON["slot"].AsInt;
                        Debug.Log("Got newuser in " + playerSlot.ToString());
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
                        GameObject player = m_playerList[playerSlot];
                        if (player.activeSelf == false)
                        {
                            player = InstantiatePlayer(playerSlot, Constants.ENEMY_TAG);
                        }
                        player.GetComponent<WebController>().Act(data);
                    }
                    if(method == "deaduser"){
                        Debug.Log("Got deaduser request");
                        int playerSlot = replyJSON["deaduser"].AsInt;
                        GameObject deadPlayer = m_playerList[playerSlot];
                        Destroy(deadPlayer);
                        m_playerList[playerSlot] = (GameObject)Instantiate(Resources.Load("Character"));
                        m_playerList[playerSlot].SetActive(false);
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
                    string payloadJSON = "{ \"method\" : [\"pos_update\"], \"data\" : " + JsonUtility.ToJson(payloadData) + "}";
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

        public IEnumerator KillUserPlayer(){
            //m_playerList[m_curPlayerSlot].SetActive(false); // TODO - Despawn the user object if required, just deactivates it for now
            Instantiate(Resources.Load("GameOverText"));
            yield return 0;
        }

        public IEnumerator BroadcastAction(string actionID = null)
        {
            BroadcastPayload payloadData = m_curPlayer.GetComponent<Player>().GetInfo();
            payloadData.action = actionID;
            //Debug.Log("Sending Action : " + actionID);
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
            GameObject player = m_playerList[playerSlot];
            bool userControllable = false;
            if (playerTag == Constants.PLAYER_TAG)
            {
                Debug.Log("Instantiating main player");
                player.AddComponent<PlayerController>();
                userControllable = true;
            }
            else{
                player.AddComponent<WebController>();
            }
            player.transform.position = Constants.SPAWN_POINTS[playerSlot];
            player.name = playerSlot.ToString();
            player.tag = playerTag;
            player.GetComponent<Player>().m_webAPI = gameObject.GetComponent<WebsocketBase>();

            string spriteBase = Constants.PLAYER_NAMES[playerSlot % Constants.PLAYER_NAMES.Length];
            //string spriteBase = "Blob";
            string spriteName = spriteBase;
            string animatorName = spriteBase + "_Animation_Controller";
            var sprite = Resources.Load<Sprite>(spriteName);
            player.GetComponent<SpriteRenderer>().sprite = sprite;
            player.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(animatorName);

            player.GetComponent<Player>().InitializePlayer(Constants.PLAYER_NAMES[playerSlot % Constants.PLAYER_NAMES.Length], userControllable);
            player.SetActive(true);
            return player;
        }
    }
}

