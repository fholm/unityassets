using UnityEngine;
using System.Collections;

namespace VoiceChat
{
    public enum VoiceChatCompression : byte
    {
        /*
        Raw, 
        RawZlib, 
        */
        Alaw,
        AlawZlib,
        Speex
    } 
}
