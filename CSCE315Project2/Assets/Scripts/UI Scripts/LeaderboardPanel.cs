using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rebound
{

    public class LeaderboardPanel : MonoBehaviour
    {

        public Text usernameText, scoreText;

        private string m_leaderboardURL = "http://" + Constants.SERVER_IP + Constants.LEADERBOARD_ENDPONT;

        // Use this for initialization
        void Awake()
        {
            usernameText.text = "";
            scoreText.text = "";
            StartCoroutine(PopulateLeaderboard());
        }


        public IEnumerator PopulateLeaderboard()
        {
            WWW leaderboardReq = WebHelper.CreateGetRequest_WWW(m_leaderboardURL);
            yield return leaderboardReq;
            if (leaderboardReq.error != null)//(loginUserReq.isNetworkError || loginUserReq.isHttpError)
            {
                Debug.Log(leaderboardReq.error);
                PopulateLeaderboardWithConstants();
                yield break;
            }
            else
            {
                Debug.Log("Got respose for login: " + leaderboardReq.text);
                var leaderboardJSON = SimpleJSON.JSON.Parse(leaderboardReq.text);
                var leaderboardList = leaderboardJSON["leaders"].AsArray;
                if(leaderboardList.Count == 0)
                {
                    PopulateLeaderboardWithConstants();
                }
                else
                {
                    PopulateLeaderboardWithConstants(); // TODO : Change this to support the response data
                }
                yield break;
            }
        }

        public void PopulateLeaderboardWithConstants()
        {
            string usernames = "";
            string scores = "";
            for (int i = 0; i < Constants.LEADERBOARD_USERNAMES.Count; i++)
            {
                usernames += Constants.LEADERBOARD_USERNAMES[i] + "\n";
                scores += Constants.LEADERBOARD_SCORES[i] + "\n";
            }
            usernameText.text = usernames;
            scoreText.text = scores;
        }
    }
}
