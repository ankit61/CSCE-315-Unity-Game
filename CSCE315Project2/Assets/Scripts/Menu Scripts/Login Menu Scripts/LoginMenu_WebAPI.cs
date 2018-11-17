using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Rebound
{
    public class LoginMenu_WebAPI : MonoBehaviour
    {
        private string m_roomReqUrl = "http://206.189.214.224:80/loginuser";

        public IEnumerator LoginUser(string _username)
        {

            Debug.Log("Logging in user");
            SharedData.Username = _username;

            // TODO : Add username Authentication here

            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
            yield return 0;

            /*
            UnityWebRequest roomReq = UnityWebRequest.Get(m_roomReqUrl);
            yield return roomReq.SendWebRequest();

            if (roomReq.isNetworkError || roomReq.isHttpError)
            {
                Debug.Log(roomReq.error);
                // TODO : Add some sort of message to the user that they cannot login
            }
            else
            {
                var replyJSON = JSON.Parse(roomReq.downloadHandler.text);
                string roomId = replyJSON["room"];
                yield return StartCoroutine(JoinGame(roomId, Constants.MAP_1_SCENE_NAME));

            }
            */

        }
    }
}
