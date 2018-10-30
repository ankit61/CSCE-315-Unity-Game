﻿using UnityEngine;
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

    private WebSocket m_socket;

    // Server IP : 206.189.78.132
    // Use this for initialization
    IEnumerator Start()
    {
        m_socket = new WebSocket(new Uri("ws://206.189.78.132:8080/AAAAA"));
        yield return StartCoroutine(m_socket.Connect());
        string connectStr = "{\"action\" : [], \"data\" : {} }";
        m_socket.SendString(connectStr);
        while (true)
        {
            string reply = m_socket.RecvString();
            if (reply != null)
            {
                statusObj.text = "Connected!";
                ConnectReply connectReply = JsonUtility.FromJson<ConnectReply>(reply);
                if (connectReply.newuser != 0)
                {
                    Debug.Log(reply);
                    userIDObj.text = connectReply.newuser.ToString();
                }
                break;
            }
            if (m_socket.error != null)
            {
                userIDObj.text = m_socket.error;
                break;
            }
            yield return 0;
        }
        /*
        m_socket.Close();
        */
    }

    IEnumerator PingServer()
    {
        string pingStr = "{\"action\" : [\"ping\"], \"data\" : {} }";
        m_socket.SendString(pingStr);
        while (true)
        {
            string reply = m_socket.RecvString();
            if (reply != null)
            {
                Debug.Log(reply);
                break;
            }
            if (m_socket.error != null)
            {
                userIDObj.text = m_socket.error;
                break;
            }
            yield return 0;
        }
    }

    public IEnumerator BroadcastAction()
    {
        string pingStr = "{\"action\" : [\"action\"], \"data\" : {} }";
        m_socket.SendString(pingStr);
        while (true)
        {
            string reply = m_socket.RecvString();
            if (reply != null)
            {
                Debug.Log(reply);
                break;
            }
            if (m_socket.error != null)
            {
                userIDObj.text = m_socket.error;
                break;
            }
            yield return 0;
        }
    }
}