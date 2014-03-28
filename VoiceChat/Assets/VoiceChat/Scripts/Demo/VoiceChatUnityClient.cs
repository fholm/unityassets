using UnityEngine;
using System.Collections;

public class VoiceChatUnityClient : MonoBehaviour
{
    VoiceChatNetworkProxy proxy;

    public int Port = 15000;
    public string Address = "127.0.0.1";

    void Start()
    {
        Network.Connect(Address, Port);
    }

    void OnConnectedToServer()
    {
        proxy = VoiceChatUtils.CreateProxy();
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        GameObject.Destroy(proxy.gameObject);
    }
}