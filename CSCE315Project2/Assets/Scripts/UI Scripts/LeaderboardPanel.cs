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
                PopulateLeaderboard(Constants.LEADERBOARD_USERNAMES, Constants.LEADERBOARD_SCORES);
                yield break;
            }
            else
            {
                Debug.Log("Got respose for login: " + leaderboardReq.text);
                var leaderboardJSON = SimpleJSON.JSON.Parse(leaderboardReq.text);
                var leaderboardList = leaderboardJSON["leaders"].AsArray;
                if(leaderboardList.Count == 0)
                {
                    PopulateLeaderboard(Constants.LEADERBOARD_USERNAMES, Constants.LEADERBOARD_SCORES);
                }
                else
                {
                    List<string> usernameList = new List<string>();
                    List<string> scoreList = new List<string>();
                    int length = Mathf.Min(10, leaderboardList.Count);
                    for (int i = 0; i < length; i++)
                    {
                        usernameList.Add(leaderboardList[i][0]);
                        scoreList.Add(leaderboardList[i][1].ToString());
                    }
                    PopulateLeaderboard(usernameList, scoreList);
                }
                yield break;
            }
        }

        public void PopulateLeaderboard(List<string> usernameList, List<string> scoreList)
        {
            string usernames = "";
            string scores = "";
            for (int i = 0; i < usernameList.Count; i++)
            {
                usernames += usernameList[i] + "\n";
                scores += scoreList[i] + "\n";
            }
            usernameText.text = usernames;
            scoreText.text = scores;
        }
    }
}
