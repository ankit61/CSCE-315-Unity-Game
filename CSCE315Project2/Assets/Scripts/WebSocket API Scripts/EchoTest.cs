using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class EchoTest : MonoBehaviour
{
    public Text statusObj;
    public Text userIDObj;
    // Use this for initialization
    IEnumerator Start()
    {
        WebSocket w = new WebSocket(new Uri("ws://localhost:8080/AAAAA"));
        yield return StartCoroutine(w.Connect());
        string connectStr = "{\"action\" : [], \"data\" : {} }";
        string pingStr = "{\"action\" : [\"ping\"], \"data\" : {} }";
        w.SendString(connectStr);
        statusObj.text = "Connected!";
        long i = 0;
        while (true)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                int newUserIndex = reply.IndexOf("newuser");
                if (newUserIndex != -1){
                    Debug.Log(reply);
                    userIDObj.text = reply.Substring(newUserIndex + 10, reply.Length - newUserIndex -11);
                    Debug.Log(reply.Substring(newUserIndex + 10, reply.Length - newUserIndex - 11));
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