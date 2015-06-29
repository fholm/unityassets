using UnityEngine;
using System.Collections;

namespace VoiceChat.Demo
{
    public class VoiceChatServerUi : MonoBehaviour
    {
        void OnGUI()
        {
            int w = Screen.width / 2;
            int h = Screen.height / 2;

            GUI.Label(new Rect(w - 50, h - 10, 100, 20), "Server Running");
        }
    } 
}
