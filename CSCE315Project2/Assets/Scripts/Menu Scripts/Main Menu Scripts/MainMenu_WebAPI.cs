using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;

namespace Rebound
{
    public class MainMenu_WebAPI : MonoBehaviour
    {
        private string m_roomReqUrl = "http://" + Constants.SERVER_IP + "/requestroom";
        private string m_roomCheckUrl = "http://" + Constants.SERVER_IP + "/checkroom";

        public Text m_createErrorMessage;
        public Text m_joinErrorMessage;

        private void Awake()
        {
            m_createErrorMessage.color = new Color(m_createErrorMessage.color.r, m_createErrorMessage.color.g, m_createErrorMessage.color.b, 0);
            m_joinErrorMessage.color = new Color(m_joinErrorMessage.color.r, m_joinErrorMessage.color.g, m_joinErrorMessage.color.b, 0);
        }

        public IEnumerator GetNewRoom()
        {
            WWW roomReq = WebHelper.CreateGetRequest_WWW(m_roomReqUrl);
            yield return roomReq;

            if (roomReq.error != null)
            {
                Debug.Log(roomReq.error);
                StartCoroutine(ShowErrorMessage(m_createErrorMessage));
            }
            else
            {
                var replyJSON = JSON.Parse(roomReq.text);
                Debug.Log(roomReq.text);
                string roomId = replyJSON["room"];
                yield return StartCoroutine(JoinGame(roomId, Constants.MAP_1_SCENE_NAME, false));
            }
        }

        public IEnumerator JoinGame(string _roomID, string _mapSceneName, bool verifyRoom = true)
        {
            SharedData.RoomID = _roomID;

            if ((_roomID == "00000000") || (!verifyRoom))
            {
                SceneTransitionManager.LoadScene(_mapSceneName);
                yield break;
            }

            bool roomAuthResult = AuthenticateRoomID(_roomID);

            if (roomAuthResult)
            {
                SceneTransitionManager.LoadScene(_mapSceneName);
            }
            else
            {
                Debug.Log("Incorrect Room ID");
                StartCoroutine(ShowErrorMessage(m_joinErrorMessage));
            }
            yield break;
        }

        public IEnumerator ShowErrorMessage(Text _text)
        {
            StartCoroutine(TextFunctions.FadeTextToFullAlpha(1, _text));
            yield return new WaitForSecondsRealtime(3.0f);
            StartCoroutine(TextFunctions.FadeTextToZeroAlpha(1, _text));
            yield return 0;
        }

        public bool AuthenticateRoomID(string _roomID)
        {
            if (_roomID.Length != 8)
            {
                return false;
            }
            
            string postData = "{\"room\" : \"" + _roomID + "\"}";
            WWW roomCheckReq = WebHelper.CreatePostJsonRequest_WWW(m_roomCheckUrl, postData);
            while (!roomCheckReq.isDone)
            {
                // Wait for request to finish
            }

            if (roomCheckReq.error != null)
            {
                Debug.Log(roomCheckReq.error);
                return false;
            }
            else
            {
                Debug.Log("Authenticate room response: " + roomCheckReq.text);
                var roomCheckResponseJSON = JSON.Parse(roomCheckReq.text);
                bool roomExists = roomCheckResponseJSON["exists"].AsBool;

                if (roomExists)
                    return true;
                else
                    return false;
            }
            
        }
    }
}
