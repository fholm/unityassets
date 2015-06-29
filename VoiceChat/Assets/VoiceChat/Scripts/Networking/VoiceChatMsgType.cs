using UnityEngine.Networking;

namespace VoiceChat.Networking
{
    class VoiceChatMsgType
    {
        public const short Base = MsgType.Highest;

        public const short RequestProxy = Base + 1;
        public const short SpawnProxy   = Base + 2;
        public const short Packet       = Base + 3;
    }
}
