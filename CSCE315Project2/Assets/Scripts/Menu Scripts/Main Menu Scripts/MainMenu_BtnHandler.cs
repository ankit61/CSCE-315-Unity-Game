using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Rebound
{   
    public class MainMenu_BtnHandler : MonoBehaviour {

    	public Button m_createGameBtn, m_joinGameBtn, m_logoutBtn;
        public InputField m_accessCodeFld;
        private MainMenu_WebAPI m_webAPI;

        private void Awake()
        {
            // Assign Button event handlers
            m_createGameBtn.onClick.AddListener(CreateBtnClicked);
            m_joinGameBtn.onClick.AddListener(JoinGameBtnClicked);
            m_logoutBtn.onClick.AddListener(LogoutBtnClicked);

            // Variable initialization
            m_webAPI = gameObject.GetComponent<MainMenu_WebAPI>();
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        void CreateBtnClicked()
        {
            StartCoroutine(m_webAPI.GetNewRoom());
        }

        void JoinGameBtnClicked()
        {
            string accessCode = m_accessCodeFld.text;
            if (AuthenticateRoomID(accessCode))
            {
                Debug.Log("Attempting to join room: " + accessCode);
                StartCoroutine(m_webAPI.JoinGame(accessCode, Constants.MAP_1_SCENE_NAME));
            }
            else
            {
                Debug.Log("The access code entered was incorrect.");
            }
        }

        void LogoutBtnClicked(){
            Debug.Log("Logging out");
            SharedData.Username = "";
            SceneManager.LoadScene(Constants.LOGIN_MENU_SCENE_NAME);
        }

        bool AuthenticateRoomID(string _roomID)
        {
            if (_roomID.Length != 8)
                return false;
            return true;
        }
    }
}
