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

        private string m_roomReqUrl = "http://206.189.214.224:80/requestroom";

        public IEnumerator GetNewRoom()
        {
            UnityWebRequest roomReq = UnityWebRequest.Get(m_roomReqUrl);
            yield return roomReq.SendWebRequest();

            if (roomReq.isNetworkError || roomReq.isHttpError)
            {
                Debug.Log(roomReq.error);
                // TODO : Add some sort of message to the user that the game cannot connect to the server
            }
            else
            {
                var replyJSON = JSON.Parse(roomReq.downloadHandler.text);
                string roomId = replyJSON["room"];
                yield return StartCoroutine(JoinGame(roomId, Constants.MAP_1_SCENE_NAME));
            }
        }

        public IEnumerator JoinGame(string _roomID, string _mapSceneName)
        {
            SharedData.RoomID = _roomID;
            SceneManager.LoadScene(_mapSceneName);
            yield return 0;
        }
    }
}
