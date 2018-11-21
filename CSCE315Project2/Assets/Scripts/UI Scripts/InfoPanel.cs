using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Rebound
{

    public class InfoPanel : MonoBehaviour {

        public List<Image> m_images;
        public List<Text> m_usernames;
        public List<Text> m_crosses;

        // Use this for initialization
        private void Awake()
        {
            for (int i = 0; i < m_images.Count; i++){
                m_images[i].gameObject.SetActive(false);
                m_usernames[i].gameObject.SetActive(false);
                m_crosses[i].gameObject.SetActive(false);
            }
        }

        public void InitializeUser(int _slot, string _username)
        {
            m_images[_slot].gameObject.SetActive(true);
            m_usernames[_slot].text = _username;
            m_usernames[_slot].gameObject.SetActive(true);
        }

        public void KillUser(int _slot)
        {
            m_crosses[_slot].gameObject.SetActive(true);
        }

        public IEnumerator UpdateAction(Player.State _state, int _moves)
        {
            yield return null;
        }

        public void RemoveUser(int _slot)
        {
            m_images[_slot].gameObject.SetActive(false);
            m_usernames[_slot].gameObject.SetActive(false);
            m_crosses[_slot].gameObject.SetActive(false);
        }

    }
}