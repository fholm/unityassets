using UnityEngine;
using System.Collections;

public struct VoiceChatPacket
{
    public VoiceChatCompression Compression;
    public int Length;
    public byte[] Data;
    public int NetworkId;
}
