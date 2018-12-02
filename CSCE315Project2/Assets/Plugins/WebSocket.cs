using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Events;

public class WebSocket
{
	private Uri mUrl;

	public WebSocket(Uri url)
	{
		mUrl = url;

		string protocol = mUrl.Scheme;
		if (!protocol.Equals("ws") && !protocol.Equals("wss"))
			throw new ArgumentException("Unsupported protocol: " + protocol);
	}

	public void SendString(string str)
	{
		Send(Encoding.UTF8.GetBytes (str));
	}

	public List<string> RecvString(int maxCount = 5)
	{
		List<byte[]> retvalList = Recv(maxCount);
        List<string> returnList = new List<string>();

        for(int i = 0; i < retvalList.Count; i++)
        {
            returnList.Add(Encoding.UTF8.GetString(retvalList[i]));
        }

		return returnList;
	}
#if UNITY_WEBGL && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern int SocketCreate (string url);

	[DllImport("__Internal")]
	private static extern int SocketState (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketSend (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern void SocketRecv (int socketInstance, byte[] ptr, int length);

	[DllImport("__Internal")]
	private static extern int SocketRecvLength (int socketInstance);

	[DllImport("__Internal")]
	private static extern void SocketClose (int socketInstance);

	[DllImport("__Internal")]
	private static extern int SocketError (int socketInstance, byte[] ptr, int length);

	int m_NativeRef = 0;

	public void Send(byte[] buffer)
	{
		SocketSend (m_NativeRef, buffer, buffer.Length);
	}

    public List<byte[]> Recv(int maxCount = 5)
	{
        List<byte[]> returnList = new List<byte[]>();
		
		int length = SocketRecvLength (m_NativeRef);
		if (length == 0)
			return null;
		byte[] buffer = new byte[length];
		SocketRecv (m_NativeRef, buffer, length);
		returnList.Add(buffer);
		return returnList;
	}

	public IEnumerator Connect()
	{
		m_NativeRef = SocketCreate (mUrl.ToString());

		while (SocketState(m_NativeRef) == 0)
			yield return 0;
	}
 
	public void Close()
	{
		SocketClose(m_NativeRef);
	}

	public string error
	{
		get {
			const int bufsize = 1024;
			byte[] buffer = new byte[bufsize];
			int result = SocketError (m_NativeRef, buffer, bufsize);

			if (result == 0)
				return null;

			return Encoding.UTF8.GetString (buffer);				
		}
	}
#else
    WebSocketSharp.WebSocket m_Socket;
	Queue<byte[]> m_Messages = new Queue<byte[]>();
	bool m_IsConnected = false;
	string m_Error = null;

	public IEnumerator Connect()
    {
        m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString());
        m_Socket.OnMessage += (sender, e) => BaseListener(e.RawData);
        m_Socket.OnOpen += (sender, e) => m_IsConnected = true;
        m_Socket.OnError += (sender, e) => m_Error = e.Message;

        m_Socket.ConnectAsync();
		while (!m_IsConnected && m_Error == null)
			yield return 0;
	}

    public void BaseListener(byte[] _data)
    {
        if(Encoding.UTF8.GetString(_data) != null)
        {
            m_Messages.Enqueue(_data);
        }
    }

    public void AddMessageListener(Action<string> _listener){
        m_Socket.OnMessage += (sender, e) => _listener(Encoding.UTF8.GetString(e.RawData));
    }

	public void Send(byte[] buffer)
	{
		m_Socket.Send(buffer);
	}

	public List<byte[]> Recv(int maxCount = 5)
	{
        List<byte[]> returnList = new List<byte[]>();
		if (m_Messages.Count == 0)
			return returnList;
        int numMessages = m_Messages.Count;
        int maxMessages = Mathf.Min(numMessages, maxCount);
        for (int i = 0; i < numMessages; i++)
        {
            returnList.Add(m_Messages.Dequeue());
        }
		return returnList;
	}

	public void Close()
	{
		m_Socket.Close();
	}

	public string error
	{
		get {
			return m_Error;
		}
	}
#endif 
}