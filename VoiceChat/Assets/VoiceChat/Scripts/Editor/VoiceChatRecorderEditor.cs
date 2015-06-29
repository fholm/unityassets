using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VoiceChat
{
    [CustomEditor(typeof(VoiceChatRecorder))]
    public class VoiceChatRecorderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VoiceChatRecorder recorder = target as VoiceChatRecorder;

            if (Application.isPlaying && Network.isClient)
            {
                EditorGUILayout.LabelField("Available Devices", EditorStyles.boldLabel);

                foreach (string device in recorder.AvailableDevices)
                {
                    EditorGUILayout.LabelField(device);
                }

                EditorGUILayout.LabelField("Selected Device", EditorStyles.boldLabel);

                int index = Mathf.Clamp(Array.IndexOf(recorder.AvailableDevices, recorder.Device), 0, Int32.MaxValue) + 1;
                string[] devices = new string[1] { "Default" }.Concat(recorder.AvailableDevices).ToArray();

                index = EditorGUILayout.Popup(index, devices);

                if (index != 0)
                {
                    recorder.Device = devices[index];
                }
                else
                {
                    recorder.Device = null;
                }

                if (recorder.IsRecording)
                {
                    if (GUILayout.Button("Stop Recording"))
                    {
                        recorder.StopRecording();
                    }
                }
                else
                {
                    if (GUILayout.Button("Start Recording"))
                    {
                        recorder.StartRecording();
                    }
                }
            }
            else
            {
                DrawDefaultInspector();
            }
        }
    } 
}