using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Rebound;
using SimpleJSON;

namespace Rebound
{

    public class WebsocketBase : MonoBehaviour
    {

        private WebSocket m_socket;
        private int m_curPlayerSlot = 0; 
        private List<GameObject> m_playerList;
        private string m_roomId;
        private string m_wsUrlBase = "ws://" + Constants.SERVER_IP + "/room/";
        private string m_incScoreUrl = "http://" + Constants.SERVER_IP + Constants.INCREASE_SCORE_ENDPONT;
        private GameObject m_curPlayer = null;
        private PlayerInfoPanel m_infoPanel; 

        public Text m_accessCodeText;
        
        // Use this for initialization
        IEnumerator Start()
        {
            // Init player prefabs
            m_playerList = new List<GameObject>();
            for (int i = 0; i < Constants.MAX_PLAYERS; i++){ // Instantiate all players
                GameObject playerObj = (GameObject)Instantiate(Resources.Load("Character"));
                playerObj.SetActive(false);
                m_playerList.Add(playerObj);
            }

            string roomId = SharedData.RoomID;
            Debug.Log("Connecting to room: " + roomId);
            m_roomId = roomId;
            m_socket = new WebSocket(new Uri(m_wsUrlBase + roomId));
            //m_socket.AddMessageListener(HandleMessage); // FIXME : This is used to fix the desync issues, but cannot be added due to threading in Unity

            InitializeScene(roomId);

            yield return StartCoroutine(m_socket.Connect());

            StartCoroutine(SendJoinRequest());
            StartCoroutine(StartListener()); // Comment this out when testing desync issues
            StartCoroutine(StartServerUpdator());
        }

        IEnumerator SendJoinRequest()
        {
            var dataJSON = JSON.Parse("{}");
            dataJSON["username"] = SharedData.Username;
            string payloadJSON = "{ \"method\" : [\"join\"], \"data\" : {\"username\" : \"" + SharedData.Username + "\"}}";
            Debug.Log(payloadJSON);
            m_socket.SendString(payloadJSON);
            yield return 0;
        }


        IEnumerator StartListener()
        {
            while (true)
            {
                List<string> replies = m_socket.RecvString(5);
                for (int i = 0; i < replies.Count; i++)
                {
                    string reply = replies[i];
                    if (reply != null)
                    {
                        HandleMessage(reply);
                    }
                    if (m_socket.error != null)
                    {
                        Debug.LogError(m_socket.error);
                    }
                }
                yield return 0;
            }
        }

        private void HandleMessage(string _msg){
            var replyJSON = JSON.Parse(_msg);
            string method = replyJSON["method"];

            if (method == "joininfo")
            {
                m_curPlayerSlot = replyJSON["slot"].AsInt;
                m_curPlayer = InstantiatePlayer(m_curPlayerSlot, Constants.PLAYER_TAG, SharedData.Username);
                var registeredPlayerSlots = replyJSON["players"].AsArray;
                for (int i = 0; i < registeredPlayerSlots.Count; i++)
                {
                    var infoTuple = registeredPlayerSlots[i].AsArray;
                    int index = (int)infoTuple[0];
                    string playerUsername = (string)infoTuple[1];
                    GameObject selectedPlayer = m_playerList[index];
                    if (selectedPlayer.activeSelf == false)
                    {
                        Debug.Log("Instantiating enemy in slot: " + playerUsername);
                        InstantiatePlayer(index, Constants.ENEMY_TAG, playerUsername);
                    }
                }
            }
            if (method == "newuser")
            {
                int playerSlot = replyJSON["slot"].AsInt;
                string playerUsername = replyJSON["username"].Value;
                if (playerUsername == "")
                    playerUsername = playerSlot.ToString();
                Debug.Log("Got newuser in " + playerSlot.ToString() + "with username: " + playerUsername);
                InstantiatePlayer(playerSlot, Constants.ENEMY_TAG , playerUsername); 
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
                if (data.action == "null")
                {
                    player.GetComponent<WebController>().UpdateTransform(data);
                }
                else if(data.action == "player_death"){
                    Debug.Log("Killing player: " + playerSlot);
                    player.GetComponent<WebController>().KillPlayer();
                    m_infoPanel.KillUser(playerSlot);
                }
                else
                {
                    player.GetComponent<WebController>().Act(data);
                }
            }
            if (method == "deaduser")
            {
                Debug.Log("Got deaduser request");
                var info = replyJSON["deaduser"].AsArray;
                int playerSlot = (int)info[0];
                string playerUsername = (string)info[1];
                GameObject deadPlayer = m_playerList[playerSlot];
                Destroy(deadPlayer);
                m_playerList[playerSlot] = (GameObject)Instantiate(Resources.Load("Character"));
                m_playerList[playerSlot].SetActive(false);
                m_infoPanel.DisableInfoSlot(playerSlot);
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

        public IEnumerator KillUserPlayer(string _lastHitByPlayer)
        {
            AudioPlayer.PlayRandomDeathSound(gameObject.GetComponent<AudioSource>());
            Debug.Log("Killing user player");
            m_playerList[m_curPlayerSlot].SetActive(false); // TODO - Despawn the user object if required, just deactivates it for now
            m_infoPanel.KillUser(m_curPlayerSlot);
            Instantiate(Resources.Load("GameOverText"));
            StartCoroutine(BroadcastAction("player_death"));

            if(_lastHitByPlayer != null)
            {
                string postData = "{\"username\" : \"" + _lastHitByPlayer + "\"}";
                WWW increaseScoreReq = WebHelper.CreatePostJsonRequest_WWW(m_incScoreUrl, postData);
                yield return increaseScoreReq;
            }

            yield return 0;
        }

        public IEnumerator BroadcastAction(string actionID = null)
        {
            BroadcastPayload payloadData = m_curPlayer.GetComponent<Player>().GetInfo();
            payloadData.action = actionID;
            string logJSON = "{ \n\"data\" : " + JsonUtility.ToJson(payloadData) + ", \n\"timestamp\" : " + Time.time.ToString() + "\n},";
            string payloadJSON = "{ \"method\" : [\"action\"], \"data\" : " + JsonUtility.ToJson(payloadData) + "}";
            m_socket.SendString(payloadJSON);
            yield return 0;
        }

        public void OnDestroy(){
            StopCoroutine(StartListener());
            StopCoroutine(StartServerUpdator());
            SharedData.PreviousRoomID = m_roomId;
            m_socket.Close();
        }

        private GameObject InstantiatePlayer(int _playerSlot, string _playerTag, string _username){
            GameObject player = m_playerList[_playerSlot];
            bool userControllable = false;
            if (_playerTag == Constants.PLAYER_TAG)
            {
                Debug.Log("Instantiating main player");
                player.AddComponent<PlayerController>();
                userControllable = true;
            }
            else{
                player.AddComponent<WebController>();
            }
            player.transform.position = Constants.SPAWN_POINTS[_playerSlot];
            player.name = _playerSlot.ToString();
            player.tag = _playerTag;
            

            string spriteBase = Constants.PLAYER_NAMES[_playerSlot % Constants.PLAYER_NAMES.Length];
            string spriteName = spriteBase;
            string animatorName = spriteBase + "_Animation_Controller";
            var sprite = Resources.Load<Sprite>(spriteName);
            player.GetComponent<SpriteRenderer>().sprite = sprite;
            player.GetComponent<Animator>().runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(animatorName);

            PlayerInfo curPlayerInfo;
            if(_playerTag == Constants.PLAYER_TAG)
            {
                curPlayerInfo = m_infoPanel.InitializePlayerInfo(_playerSlot, _username, spriteBase);
            }
            else
            {
                curPlayerInfo = m_infoPanel.InitializeOpponentInfo(_playerSlot, _username, spriteBase);
            }

            player.GetComponent<Player>().InitializePlayer(spriteBase, userControllable, gameObject.GetComponent<WebsocketBase>(), curPlayerInfo, _username);
            player.SetActive(true);

            return player;
        }

        private void InitializeScene(string _accessCode){
            m_accessCodeText.text = "Access Code: " + _accessCode;

            GameObject InfoPanel = (GameObject)Instantiate(Resources.Load("PlayerInfoPanel"));
            m_infoPanel = InfoPanel.GetComponent<PlayerInfoPanel>();
        }
    }
}

