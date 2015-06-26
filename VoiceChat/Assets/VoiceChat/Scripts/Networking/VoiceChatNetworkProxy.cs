using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace VoiceChat.Networking
{
    public class VoiceChatNetworkProxy : NetworkBehaviour
    {
        public delegate void MessageHandler<T>(T data);
        public static event MessageHandler<VoiceChatPacketMessage> VoiceChatPacketReceived;
        
        private const string ProxyPrefabPath = "VoiceChat_NetworkProxy";
        private static int localProxyId;
        private static Dictionary<int, GameObject> proxies = new Dictionary<int, GameObject>();

        public bool isMine { get { return networkId != 0 && networkId == localProxyId; } }
        
        [SyncVar]
        private int networkId;

        VoiceChatPlayer player = null;
        Queue<VoiceChatPacket> packets = new Queue<VoiceChatPacket>(16);

        void Start()
        {
            if (isMine)
            {
                VoiceChatRecorder.Instance.NewSample += OnNewSample;
                VoiceChatRecorder.Instance.NetworkId = networkId;
            }
            else
            {
                VoiceChatPacketReceived += OnReceivePacket;
            }

            if (Network.isServer)
            {
                //GetComponent<NetworkView>().RPC("SetNetworkId", GetComponent<NetworkView>().owner, assignedNetworkId);
            }

            if (isClient && (!isMine || VoiceChatSettings.Instance.LocalDebug))
            {
                gameObject.AddComponent<AudioSource>();
                player = gameObject.AddComponent<VoiceChatPlayer>();
            }
        }

        private void OnReceivePacket(VoiceChatPacketMessage data)
        {
            if (data.proxyId == networkId)
            {
                player.OnNewSample(data.packet);
            }
        }

        void OnNewSample(VoiceChatPacket packet)
        {

            var packetMessage = new VoiceChatPacketMessage {
                proxyId = (short)localProxyId,
                packet = packet,
            };

            NetworkManager.singleton.client.SendUnreliable(VoiceChatMsgType.Packet, packetMessage);
        }



        void SetNetworkId(int networkId)
        {
            var netIdentity = GetComponent<NetworkIdentity>();
            if (netIdentity.isServer || netIdentity.isClient)
            {
                Debug.LogWarning("Can only set NetworkId before spawning");
                return;
            }

            this.networkId = networkId;
            //VoiceChatRecorder.Instance.NetworkId = networkId;
        }

      
        #region NetworkManager Hooks

        public static void OnStartClient(NetworkClient client)
        {
            client.RegisterHandler(VoiceChatMsgType.Packet, OnClientPacketReceived);
            client.RegisterHandler(VoiceChatMsgType.SpawnProxy, OnProxySpawned);

            var prefab = Resources.Load<GameObject>(ProxyPrefabPath);
            ClientScene.RegisterPrefab(prefab);
        }

        public static void OnStopClient()
        {
            var client = NetworkManager.singleton.client;
            if (client == null) return;

            client.UnregisterHandler(VoiceChatMsgType.Packet);
            client.UnregisterHandler(VoiceChatMsgType.SpawnProxy);
        }

        public static void OnServerDisconnect(NetworkConnection conn)
        {
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

        public static void OnStartServer()
        {
            NetworkServer.RegisterHandler(VoiceChatMsgType.Packet, OnServerPacketReceived);
            NetworkServer.RegisterHandler(VoiceChatMsgType.RequestProxy, OnProxyRequested);
        }

        public static void OnStopServer()
        {
            NetworkServer.UnregisterHandler(VoiceChatMsgType.Packet);
            NetworkServer.UnregisterHandler(VoiceChatMsgType.RequestProxy);
        }

        public static void OnClientConnect(NetworkConnection connection)
        {
            var client = NetworkManager.singleton.client;
            client.Send(VoiceChatMsgType.RequestProxy, new EmptyMessage());
        }
        
        #endregion

        #region Network Message Handlers

        private static void OnProxyRequested(NetworkMessage netMsg)
        {
            var id = netMsg.conn.connectionId;
            netMsg.conn.Send(VoiceChatMsgType.SpawnProxy, new IntegerMessage(id));

            var prefab = Resources.Load<GameObject>(ProxyPrefabPath);
            var proxy = Instantiate<GameObject>(prefab);
            proxy.SendMessage("SetNetworkId", id);

            proxies.Add(id, proxy);
            NetworkServer.Spawn(proxy);

        }

        private static void OnProxySpawned(NetworkMessage netMsg)
        {
            localProxyId = netMsg.ReadMessage<IntegerMessage>().value;
            Debug.Log("Object spawned " + localProxyId);
        }

        private static void OnServerPacketReceived(NetworkMessage netMsg)
        {
            var data = netMsg.ReadMessage<VoiceChatPacketMessage>();

            foreach (var connection in NetworkServer.connections)
            {
                if (connection == null || connection.connectionId == data.proxyId)
                    continue;

                connection.SendUnreliable(VoiceChatMsgType.Packet, data);
            }

            foreach (var connection in NetworkServer.localConnections)
            {
                if (connection == null || connection.connectionId == data.proxyId)
                    continue;

                connection.SendUnreliable(VoiceChatMsgType.Packet, data);
            }

        }

        private static void OnClientPacketReceived(NetworkMessage netMsg)
        {
            if (VoiceChatPacketReceived != null)
            {
                var data = netMsg.ReadMessage<VoiceChatPacketMessage>();
                VoiceChatPacketReceived(data);
            }
        }
        
        #endregion

        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            int count = packets.Count;

            if (stream.isWriting)
            {
                stream.Serialize(ref count);

                while (packets.Count > 0)
                {
                    VoiceChatPacket packet = packets.Dequeue();
                    //stream.WritePacket(packet);

                    // If this packet is the same size as the sample size, we can return it
                    if (packet.Data.Length == VoiceChatSettings.Instance.SampleSize)
                    {
                        VoiceChatBytePool.Instance.Return(packet.Data);
                    }
                }
            }
            else
            {
                if (Network.isServer)
                {
                    stream.Serialize(ref count);

                    for (int i = 0; i < count; ++i)
                    {
                        //packets.Enqueue(stream.ReadPacket());

                        if (Network.connections.Length < 2)
                        {
                            packets.Dequeue();
                        }
                    }
                }
                else
                {
                    stream.Serialize(ref count);

                    for (int i = 0; i < count; ++i)
                    {
                        //var packet = stream.ReadPacket();

                        if (player != null)
                        {
                            //player.OnNewSample(packet);
                        }
                    }
                }
            }
        }
    }
}