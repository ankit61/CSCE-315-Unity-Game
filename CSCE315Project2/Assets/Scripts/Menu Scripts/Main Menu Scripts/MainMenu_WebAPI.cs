using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using SimpleJSON;

namespace Rebound
{
    // Test Server IP : 206.189.214.224
    // Server IP : 206.189.78.132
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
            UnityWebRequest roomReq = UnityWebRequest.Get(m_roomReqUrl);
            yield return roomReq.SendWebRequest();

            if (roomReq.isNetworkError || roomReq.isHttpError)
            {
                Debug.Log(roomReq.error);
                StartCoroutine(ShowErrorMessage(m_createErrorMessage));
            }
            else
            {
                var replyJSON = JSON.Parse(roomReq.downloadHandler.text);
                Debug.Log(roomReq.downloadHandler.text);
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
            WWWForm form = new WWWForm();
            form.AddField("room", _roomID);
            UnityWebRequest roomCheckReq = UnityWebRequest.Post(m_roomCheckUrl, form);
            roomCheckReq.SendWebRequest();

            if (roomCheckReq.isNetworkError || roomCheckReq.isHttpError)
            {
                Debug.Log(roomCheckReq.error);
                return false;
            }
            else
            {
                return false;
            }
            
        }
    }
}
