using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public static class WebHelper {
    public static UnityWebRequest CreatePostJsonRequest(string _url, string _postData)
    {
        UnityWebRequest request = new UnityWebRequest(_url, "POST");
        request.chunkedTransfer = false;

        byte[] bodyRaw = Encoding.UTF8.GetBytes(_postData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-type", "application/json");
        request.SetRequestHeader("Content-length", _postData.Length.ToString());
        return request;
    }
	
    public static WWW CreatePostJsonRequest_WWW(string _url, string _postData)
    {
        WWW www;
        Dictionary<string, string> postHeader = new Dictionary<string, string>();
        postHeader.Add("Content-Type", "application/json");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(_postData);
        www = new WWW(_url, bodyRaw, postHeader);
        return www;
    }


    public static WWW CreateGetRequest_WWW(string _url)
    {
        WWW www;
        www = new WWW(_url);
        return www;
    }
}
