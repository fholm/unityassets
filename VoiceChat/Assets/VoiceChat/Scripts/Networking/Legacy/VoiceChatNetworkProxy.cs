using UnityEngine;
using System.Collections.Generic;

namespace VoiceChat.Networking.Legacy
{
    public class VoiceChatNetworkProxy : MonoBehaviour
    {
        static int networkIdCounter = 0;

        int assignedNetworkId = -1;
        VoiceChatPlayer player = null;
        Queue<VoiceChatPacket> packets = new Queue<VoiceChatPacket>(16);

        void Start()
        {
            if (GetComponent<NetworkView>().isMine)
            {
                VoiceChatRecorder.Instance.NewSample += OnNewSample;
            }

            if (Network.isServer)
            {
                assignedNetworkId = ++networkIdCounter;
                GetComponent<NetworkView>().RPC("SetNetworkId", GetComponent<NetworkView>().owner, assignedNetworkId);
            }

            if (Network.isClient && (!GetComponent<NetworkView>().isMine || VoiceChatSettings.Instance.LocalDebug))
            {
                gameObject.AddComponent<AudioSource>();
                player = gameObject.AddComponent<VoiceChatPlayer>();
            }
        }

        void OnNewSample(VoiceChatPacket packet)
        {
            packets.Enqueue(packet);
        }

        [RPC]
        void SetNetworkId(int networkId)
        {
            VoiceChatRecorder.Instance.NetworkId = networkId;
        }

        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            int count = packets.Count;

            if (stream.isWriting)
            {
                stream.Serialize(ref count);

                while (packets.Count > 0)
                {
                    VoiceChatPacket packet = packets.Dequeue();
                    stream.WritePacket(packet);

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
                        packets.Enqueue(stream.ReadPacket());

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
                        var packet = stream.ReadPacket();

                        if (player != null)
                        {
                            player.OnNewSample(packet);
                        }
                    }
                }
            }
        }
    } 
}