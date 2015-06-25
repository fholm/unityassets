using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace VoiceChat.Networking.Legacy
{
    public static class VoiceChatExtensions
    {
        [StructLayout(LayoutKind.Explicit)]
        struct PackShort
        {
            [FieldOffset(0)]
            public short Short;

            [FieldOffset(0)]
            public byte Byte0;

            [FieldOffset(1)]
            public byte Byte1;
        }

        public static void WritePacket(this BitStream stream, VoiceChatPacket packet)
        {
            PackShort pack = new PackShort();
            short compression = (short)(byte)packet.Compression;

            stream.Serialize(ref packet.Length);
            stream.Serialize(ref compression);

            for (int i = 0; i < packet.Length; i += 2)
            {
                pack.Byte0 = packet.Data[i];

                if (i + 1 < packet.Length)
                {
                    pack.Byte1 = packet.Data[i + 1];
                }

                stream.Serialize(ref pack.Short);
            }
        }

        public static VoiceChatPacket ReadPacket(this BitStream stream)
        {
            short compression = 0;
            PackShort pack = new PackShort();
            VoiceChatPacket packet = new VoiceChatPacket();

            stream.Serialize(ref packet.Length);
            stream.Serialize(ref compression);

            packet.Compression = (VoiceChatCompression)(byte)compression;
            packet.Data = VoiceChatBytePool.Instance.Get();

            for (int i = 0; i < packet.Length; i += 2)
            {
                stream.Serialize(ref pack.Short);

                packet.Data[i] = pack.Byte0;

                if (i + 1 < packet.Length)
                {
                    packet.Data[i + 1] = pack.Byte1;
                }
            }

            return packet;
        }
    } 
}