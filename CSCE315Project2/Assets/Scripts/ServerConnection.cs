using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerConnection : MonoBehaviour {
	
	private WebSocket w;

	void Start () {
		w = new WebSocket(new Uri("ws://0.0.0.0:8080/AAAAA"));
		w.Connect();
	}
	
	void Update () {
		HandleKeypress();
	}

	void HandleKeypress() 
	{
		if (Input.GetKeyDown("p")){
			Debug.Log(w);
		}
	}

	/* 
	private void HandleReply() {

	}
	*/
}
