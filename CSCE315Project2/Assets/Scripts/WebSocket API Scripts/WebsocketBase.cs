using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConnectReply
{
    public long newuser = 0;
}

public class WebsocketBase : MonoBehaviour {

    private WebSocket m_socket;

    // Server IP : 206.189.78.132
    // Use this for initialization
    IEnumerator Start()
    {
        m_socket = new WebSocket(new Uri("ws://206.189.78.132:8080/AAAAA"));
        yield return StartCoroutine(m_socket.Connect());
        string connectStr = "{\"action\" : [], \"data\" : {} }";
        m_socket.SendString(connectStr);
        StartCoroutine(StartListener());
    }

    IEnumerator StartListener()
    {
        while (true)
        {
            string reply = m_socket.RecvString();
            if (reply != null)
            {
                Debug.Log(reply);
                ConnectReply connectReply = JsonUtility.FromJson<ConnectReply>(reply);
                if (connectReply.newuser != 0)
                {
                    Debug.Log("New user connected!");
                }
            }
            if (m_socket.error != null)
            {
                Debug.LogError(m_socket.error);
            }
            yield return 0;
        }
    }

    IEnumerator PingServer()
    {
        string pingStr = "{\"action\" : [\"ping\"], \"data\" : {} }";
        m_socket.SendString(pingStr);
        yield return 0;
    }

    public IEnumerator BroadcastAction(string actionID = "Basic Action")
    {
        string actionJSONStr = "{\"action\" : [\"action\"], \"data\" : { \"actionID\" : \"" + actionID + "\"} }";
        m_socket.SendString(actionJSONStr);
        yield return 0;
    }

    IEnumerator CloseConnection()
    {
        StopCoroutine(StartListener());
        m_socket.Close();
        yield return 0;
    }
}
