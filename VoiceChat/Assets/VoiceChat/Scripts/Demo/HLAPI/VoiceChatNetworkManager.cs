using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using VoiceChat.Networking;

namespace VoiceChat.Demo.HLAPI
{
    public class VoiceChatNetworkManager : NetworkManager
    {
        public override void OnStartClient(NetworkClient client)
        {
            VoiceChatNetworkProxy.OnStartClient(client);

            gameObject.AddComponent<VoiceChatUi>();
        }

        public override void OnStopClient()
        {
            VoiceChatNetworkProxy.OnStopClient();

            if (client != null)
                Destroy(GetComponent<VoiceChatUi>());
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            VoiceChatNetworkProxy.OnServerDisconnect(conn);
        }

        public override void OnStartServer()
        {
            VoiceChatNetworkProxy.OnStartServer();

            gameObject.AddComponent<VoiceChatServerUi>();
        }

        public override void OnStopServer()
        {
            VoiceChatNetworkProxy.OnStopServer();

            Destroy(GetComponent<VoiceChatServerUi>());
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);
            VoiceChatNetworkProxy.OnClientConnect(connection);
        }
    }
}
