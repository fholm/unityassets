using UnityEngine;
using System.Collections;

namespace VoiceChat.Demo.LegacyNetworking
{
    public class VoiceChatUnityServer : MonoBehaviour
    {
        public int Port = 15000;
        public int MaxConnections = 8;

        void Start()
        {
            Network.InitializeServer(MaxConnections, Port, false);
            MonoBehaviour.Destroy(GetComponent<VoiceChatUnityClient>());
        }

        void OnPlayerDisconnected(NetworkPlayer player)
        {
            Network.RemoveRPCs(player);
            Network.DestroyPlayerObjects(player);
        }
    } 
}
