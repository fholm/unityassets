using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VoiceChat
{
    [CustomEditor(typeof(VoiceChatSettings))]
    public class VoiceChatSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VoiceChatSettings settings = target as VoiceChatSettings;

            if (settings != null)
            {
                settings.LocalDebug = EditorGUILayout.Toggle("Local Debug", settings.LocalDebug);
                settings.Preset = (VoiceChatPreset)EditorGUILayout.EnumPopup("Codec", settings.Preset);
                EditorGUILayout.LabelField("Frequency", settings.Frequency.ToString());
                EditorGUILayout.LabelField("Sample Size", settings.SampleSize.ToString());

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(settings);
                }
            }
        }
    }
    
}