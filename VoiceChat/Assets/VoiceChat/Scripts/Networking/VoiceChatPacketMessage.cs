using UnityEngine;
using UnityEngine.Networking;

namespace VoiceChat.Networking
{
    public class VoiceChatPacketMessage : MessageBase
    {
        public short proxyId;
        public VoiceChatPacket packet;

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(proxyId);
            writer.Write(packet.PacketId);
            writer.Write((short)packet.Compression);
            writer.Write(packet.Length);
            writer.WriteBytesFull(packet.Data);
        }

        public override void Deserialize(NetworkReader reader)
        {
            proxyId = reader.ReadInt16();
            packet.PacketId = reader.ReadUInt64();
            packet.Compression = (VoiceChatCompression)reader.ReadInt16();
            packet.Length = reader.ReadInt32();
            packet.Data = reader.ReadBytesAndSize();
        }
    }
}
