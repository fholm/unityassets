using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VoiceChat.Networking.Legacy
{
    public class VoiceChatNetworkUtils
    {
        public static VoiceChatNetworkProxy CreateProxy()
        {
            if (!Network.isClient)
            {
                Debug.LogError("You're not a client in the unity networking");
                return null;
            }

            GameObject prefab = Resources.Load("Legacy/VoiceChat_NetworkProxy") as GameObject;
            GameObject instance = Network.Instantiate(prefab, Vector3.zero, Quaternion.identity, 0) as GameObject;
            return instance.GetComponent<VoiceChatNetworkProxy>();
        }
    }
}
