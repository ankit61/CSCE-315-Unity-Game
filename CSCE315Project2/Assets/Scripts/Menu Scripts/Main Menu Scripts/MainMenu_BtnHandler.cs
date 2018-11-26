using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Rebound
{   
    public class MainMenu_BtnHandler : MonoBehaviour {

    	public Button m_createGameBtn, m_joinGameBtn, m_logoutBtn, m_controlsMenuBtn;
        public InputField m_accessCodeFld;
        private MainMenu_WebAPI m_webAPI;

        private void Awake()
        {
            // Assign Button event handlers
            m_createGameBtn.onClick.AddListener(CreateBtnClicked);
            m_joinGameBtn.onClick.AddListener(JoinGameBtnClicked);
            m_logoutBtn.onClick.AddListener(LogoutBtnClicked);
            m_controlsMenuBtn.onClick.AddListener(ControlsMenuBtnClicked);

            // Variable initialization
            m_webAPI = gameObject.GetComponent<MainMenu_WebAPI>();

            if(SharedData.RoomID != Constants.DEFAULT_ROOM_ID){
                m_accessCodeFld.text = SharedData.RoomID;
            }
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

        void ControlsMenuBtnClicked()
        {
            SceneTransitionManager.LoadScene(Constants.CONTROLS_SCENE_NAME);
        }

        void JoinGameBtnClicked()
        {
            string accessCode = m_accessCodeFld.text;
            StartCoroutine(m_webAPI.JoinGame(accessCode, Constants.MAP_1_SCENE_NAME));
        }

        void LogoutBtnClicked(){
            Debug.Log("Logging out");
            SharedData.Username = "";
            SceneManager.LoadScene(Constants.LOGIN_MENU_SCENE_NAME);
        }
    }
}
