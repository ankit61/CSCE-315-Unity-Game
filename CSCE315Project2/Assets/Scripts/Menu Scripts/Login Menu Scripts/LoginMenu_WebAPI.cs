using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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
                WWWForm form = new WWWForm();
                form.AddField("username", _username);
                UnityWebRequest loginUserReq = UnityWebRequest.Post(m_userLoginURL, form);
                yield return loginUserReq.SendWebRequest();

                if (loginUserReq.isNetworkError || loginUserReq.isHttpError)
                {
                    Debug.Log(loginUserReq.error);
                    StartCoroutine(ShowErrorMessage(m_errorMessage));
                }
                else
                {
                    Debug.Log(loginUserReq.downloadHandler.text);
                    UnityWebRequest userStatusReq = UnityWebRequest.Post(m_userStatusURL, form); // "{\"username\" : \"" + _username +  "\"}"
                    yield return userStatusReq.SendWebRequest();

                    if (userStatusReq.isNetworkError || userStatusReq.isHttpError)
                    {
                        Debug.Log(userStatusReq.error);
                        StartCoroutine(ShowErrorMessage(m_errorMessage));
                    }
                    else
                    {
                        Debug.Log(userStatusReq.downloadHandler.text);
                    }
                }
            }
        }


        public IEnumerator ShowErrorMessage(Text _message)
        {
            StartCoroutine(TextFunctions.FadeTextToFullAlpha(1, _message));
            yield return new WaitForSecondsRealtime(3.0f);
            StartCoroutine(TextFunctions.FadeTextToZeroAlpha(1, _message));
            yield return 0;
        }

    }
}
