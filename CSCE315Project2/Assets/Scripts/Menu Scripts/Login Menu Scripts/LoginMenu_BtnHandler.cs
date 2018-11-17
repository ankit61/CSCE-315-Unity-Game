using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rebound
{
    public class LoginMenu_BtnHandler : MonoBehaviour
    {
        public Button m_loginBtn;
        public InputField m_usernameFld;

        private LoginMenu_WebAPI m_webAPI;

        // Use this for initialization
        private void Awake()
        {
            // Assign Button event handlers
            m_loginBtn.onClick.AddListener(LoginBtnClicked);

            //Variable initialization
            m_webAPI = gameObject.GetComponent<LoginMenu_WebAPI>();
        }

        private void LoginBtnClicked(){
            string username = m_usernameFld.text;
            StartCoroutine(m_webAPI.LoginUser(username));
        }

    }
}
