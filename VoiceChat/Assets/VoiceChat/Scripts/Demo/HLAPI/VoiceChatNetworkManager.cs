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
            VoiceChatNetworkProxy.OnManagerStartClient(client);

            gameObject.AddComponent<VoiceChatUi>();
        }

        public override void OnStopClient()
        {
            VoiceChatNetworkProxy.OnManagerStopClient();

            if (client != null)
                Destroy(GetComponent<VoiceChatUi>());
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            VoiceChatNetworkProxy.OnManagerServerDisconnect(conn);
        }

        public override void OnStartServer()
        {
            VoiceChatNetworkProxy.OnManagerStartServer();

            gameObject.AddComponent<VoiceChatServerUi>();
        }

        public override void OnStopServer()
        {
            VoiceChatNetworkProxy.OnManagerStopServer();

            Destroy(GetComponent<VoiceChatServerUi>());
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);
            VoiceChatNetworkProxy.OnManagerClientConnect(connection);
        }
    }
}
