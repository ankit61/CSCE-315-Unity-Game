using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Rebound {
	
	public class PlayerInfo : MonoBehaviour {

		public Image m_img;
		public Text m_username;
		public Text m_cross;
		public List<Image> m_kicks;
		public List<Image> m_missiles;
		private Dictionary<Player.State, List<Image>> m_availableMoves;

		// Use this for initialization
		void Awake () {
			m_cross.gameObject.SetActive(false);
			m_username.gameObject.SetActive(false);
			m_img.gameObject.SetActive(false);
			for(int i = 0; i < m_kicks.Count; i++)
				m_kicks[i].gameObject.SetActive(false);
			for(int i = 0; i < m_missiles.Count; i++)
				m_missiles[i].gameObject.SetActive(false);

			m_availableMoves = new Dictionary<Player.State, List<Image>>() {
																		{Player.State.KICKING, m_kicks}, 
																		{Player.State.MISSILE, m_missiles}, 
																	};
		}

		public void InitializePlayerInfo(string _username, string _spritePath) {
			m_img.sprite = Resources.Load<Sprite>(_spritePath);
			m_username.text = _username;
            m_username.gameObject.SetActive(true);
            m_img.gameObject.SetActive(true);
            foreach (KeyValuePair<Player.State, int> entry in Constants.NUM_AVAILABLE_ACTIONS)
				MakeVisible(entry.Value, m_availableMoves[entry.Key]);
		}

        public void InitializeOpponentInfo(string _username, string _spritePath)
        {
            m_img.sprite = Resources.Load<Sprite>(_spritePath);
            m_username.text = _username;
            m_username.gameObject.SetActive(true);
            m_img.gameObject.SetActive(true);
			foreach (KeyValuePair<Player.State, int> entry in Constants.NUM_AVAILABLE_ACTIONS)
				MakeVisible(entry.Value, m_availableMoves[entry.Key]);
        }

        public void DisableInfo()
        {
            m_cross.gameObject.SetActive(false);
            m_username.gameObject.SetActive(false);
            m_img.gameObject.SetActive(false);
            for (int i = 0; i < m_kicks.Count; i++)
                m_kicks[i].gameObject.SetActive(false);
            for (int i = 0; i < m_missiles.Count; i++)
                m_missiles[i].gameObject.SetActive(false);
        }

        public void Die() {
			m_cross.gameObject.SetActive(true);
		}
		
		public IEnumerator UpdateAction(Player.State _state, int numActions) {
			if(m_availableMoves.ContainsKey(_state) && numActions <= m_availableMoves[_state].Count) {
				MakeVisible(numActions, m_availableMoves[_state]);
				yield return null;
			}
			else
				throw new Exception("invalid state/numActions passed");
		}

		private void MakeVisible(int _numVisible, List<Image> _images) {
			for(int i = 0; i < _numVisible; i++)
				_images[i].gameObject.SetActive(true);
			for(int i = _numVisible; i < _images.Count; i++)
				_images[i].gameObject.SetActive(false);
		}
	}

}