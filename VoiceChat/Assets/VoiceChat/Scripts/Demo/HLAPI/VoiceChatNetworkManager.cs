using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using VoiceChat.Networking;

namespace VoiceChat.Demo.HLAPI
{
    public class VoiceChatNetworkManager : NetworkManager
    {
        public delegate void MessageHandler<T>(T data);
        public static event MessageHandler<VoiceChatPacketMessage> VoiceChatPacketReceived;

        public GameObject voiceChatProxyPrefab;
        private Dictionary<int, GameObject> proxies = new Dictionary<int, GameObject>();

        public static VoiceChatNetworkManager singleton
        {
            get { return NetworkManager.singleton as VoiceChatNetworkManager; }
        }

        public override void OnStartClient(NetworkClient client)
        {
            client.RegisterHandler(VoiceChatMsgType.Packet, OnVoiceChatPacketReceived);
            client.RegisterHandler(VoiceChatMsgType.SpawnProxy, OnProxySpawned);

            ClientScene.RegisterPrefab(voiceChatProxyPrefab);
        }

        public override void OnStopClient()
        {
            if (client == null) return;

            client.UnregisterHandler(VoiceChatMsgType.Packet);
            client.UnregisterHandler(VoiceChatMsgType.SpawnProxy);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            var id = conn.connectionId;

            if (!proxies.ContainsKey(id))
            {
                Debug.LogWarning("Proxy destruction requested for client " + id + " but proxy wasn't registered");
                return;
            }

            var proxy = proxies[id];
            NetworkServer.Destroy(proxy);

            proxies.Remove(id);
        }

        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler(VoiceChatMsgType.Packet, OnVoiceChatPacketReceived);
            NetworkServer.RegisterHandler(VoiceChatMsgType.RequestProxy, OnProxyRequested);
        }

        public override void OnStopServer()
        {
            NetworkServer.UnregisterHandler(VoiceChatMsgType.Packet);
            NetworkServer.UnregisterHandler(VoiceChatMsgType.RequestProxy);
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);

            client.Send(VoiceChatMsgType.RequestProxy, new EmptyMessage());
        }

        private void OnProxyRequested(NetworkMessage netMsg)
        {
            var id = netMsg.conn.connectionId;
            netMsg.conn.Send(VoiceChatMsgType.SpawnProxy, new IntegerMessage(id));

            var proxy = Instantiate<GameObject>(voiceChatProxyPrefab);
            proxy.SendMessage("SetNetworkId", id);

            proxies.Add(id, proxy);
            NetworkServer.Spawn(proxy);

        }

        private void OnProxySpawned(NetworkMessage netMsg)
        {
            var localProxyId = netMsg.ReadMessage<IntegerMessage>().value;
            Debug.Log("Object spawned " + localProxyId);

            VoiceChatNetworkProxy.LocalProxyId = localProxyId;
        }

        private void OnVoiceChatPacketReceived(NetworkMessage netMsg)
        {
            if (VoiceChatPacketReceived != null)
            {
                var data = netMsg.ReadMessage<VoiceChatPacketMessage>();
                VoiceChatPacketReceived(data);
            }
        }
    }
}
