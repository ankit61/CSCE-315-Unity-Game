using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Rebound
{

    public class GameOverText : MonoBehaviour
    {

        public float m_alphaIncrease = 0.01f;
        public Button m_exitBtn;

        private Text m_curText;
        private Color m_curColor;

        private bool m_textAppeared = false;

        // Use this for initialization
        void Awake()
        {
            m_exitBtn.onClick.AddListener(ExitBtnClicked);

            m_curText = gameObject.GetComponent<Text>();
            m_curColor = m_curText.color;
            m_curColor.a = 0f;
            m_curText.color = m_curColor;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_textAppeared)
            {
                return;
            }


            if (System.Math.Abs(m_curText.color.a - 1f) < 0.000001f)
            {
                m_textAppeared = true;
                return;
            }
            float curAlpha = m_curText.color.a;
            m_curColor.a = Mathf.Min(1f, curAlpha + m_alphaIncrease);
            m_curText.color = m_curColor;
        }

        void ExitBtnClicked()
        {
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        }
    }
}
