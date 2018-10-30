using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ConnectReply{
    public long newuser = 0;
}

public class EchoTest : MonoBehaviour
{
    public Text statusObj;
    public Text userIDObj;
    // Use this for initialization 206.189.78.132
    IEnumerator Start()
    {
        WebSocket w = new WebSocket(new Uri("ws://localhost:8080/AAAAA"));
        yield return StartCoroutine(w.Connect());
        string connectStr = "{\"action\" : [], \"data\" : {} }";
        string pingStr = "{\"action\" : [\"ping\"], \"data\" : {} }";
        w.SendString(connectStr);
        long i = 0;
        while (true)
        {
            string reply = w.RecvString();
            //userIDObj.text = reply;
            if (reply != null)
            {
                statusObj.text = "Connected!";
                ConnectReply connectReply = JsonUtility.FromJson<ConnectReply>(reply);
                if(connectReply.newuser != 0){
                    Debug.Log(reply);
                    Debug.Log("Found Reply NewUser: " + connectReply.newuser.ToString());
                    userIDObj.text = connectReply.newuser.ToString();
                }
                w.SendString(pingStr);
            }
            if (w.error != null)
            {
                Debug.LogError("Error: " + w.error);
                break;
            }
            yield return 0;
        }
        w.Close();
    }
}