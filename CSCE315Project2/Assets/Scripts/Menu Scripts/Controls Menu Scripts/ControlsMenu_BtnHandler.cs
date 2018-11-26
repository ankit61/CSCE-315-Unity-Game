using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rebound
{

    public class ControlsMenu_BtnHandler : MonoBehaviour {

        public Button m_backBtn;

        private void Awake()
        {
            m_backBtn.onClick.AddListener(BackBtnClicked);
        }

        public void BackBtnClicked()
        {
            SceneTransitionManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        }
    }
}
