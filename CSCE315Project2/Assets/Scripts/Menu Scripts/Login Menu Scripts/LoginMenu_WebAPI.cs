using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text;

namespace Rebound
{
    public class LoginMenu_WebAPI : MonoBehaviour
    {
        private string m_userLoginURL = "http://" + Constants.SERVER_IP + "/adduser";
        private string m_userStatusURL = "http://" + Constants.SERVER_IP + "/statususer";

        public Text m_errorMessage;

        private void Awake()
        {
            m_errorMessage.color = new Color(m_errorMessage.color.r, m_errorMessage.color.g, m_errorMessage.color.b, 0);
        }

        public IEnumerator LoginUser(string _username)
        {
            Debug.Log("Logging in user");
            SharedData.Username = _username;

            if(_username == "Debug")
            {
                SceneTransitionManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
                yield break;
            }
            else
            {
                string postData = "{\"username\" : \"" + _username + "\"}";
                WWW loginUserReq = WebHelper.CreatePostJsonRequest_WWW(m_userLoginURL, postData);
                yield return loginUserReq;
                
                if (loginUserReq.error != null)//(loginUserReq.isNetworkError || loginUserReq.isHttpError)
                {
                    Debug.Log(loginUserReq.error);
                    StartCoroutine(ShowErrorMessage(m_errorMessage, loginUserReq.error));
                }
                else
                {
                    Debug.Log("Got respose for login: " + loginUserReq.text);
                    SceneTransitionManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
                    yield break;
                }
            }
        }


        public IEnumerator ShowErrorMessage(Text _message, string _errorMessage)
        {
            _message.text = _errorMessage;
            StartCoroutine(TextFunctions.FadeTextToFullAlpha(1, _message));
            yield return new WaitForSecondsRealtime(3.0f);
            StartCoroutine(TextFunctions.FadeTextToZeroAlpha(1, _message));
            yield return 0;
        }


    }
}
