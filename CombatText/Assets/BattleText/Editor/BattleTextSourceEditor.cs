using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BattleTextSource))]
public class BattleTextSourceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BattleTextSource source = target as BattleTextSource;

        if (source != null)
        {
            source.SourceName = EditorGUILayout.TextField("Name", source.SourceName);

            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
            source.Source = (BattleTextSource.PositionSource)EditorGUILayout.EnumPopup("Source", source.Source);

            switch (source.Source)
            {
                case BattleTextSource.PositionSource.Transform:
                    source.Target = (Transform)EditorGUILayout.ObjectField("Transform", source.Target, typeof(Transform), true);
                    source.FollowTargetPosition = EditorGUILayout.Toggle("Follow", source.FollowTargetPosition);
                    source.TargetWorldOffset = EditorGUILayout.Vector3Field("World Offset", source.TargetWorldOffset);
                    break;

                case BattleTextSource.PositionSource.Screen:
                    source.ScreenPosition = EditorGUILayout.Vector2Field("Position", source.ScreenPosition);
                    break;
            }

            EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
            source.DefaultText = (GameObject)EditorGUILayout.ObjectField("Default Text Prefab", source.DefaultText, typeof(GameObject), false);
            source.DefaultAnimation = (BattleTextAnimation)EditorGUILayout.ObjectField("Default Animation", source.DefaultAnimation, typeof(BattleTextAnimation), false);
            source.SingleInstance = EditorGUILayout.Toggle("Single Instance", source.SingleInstance);
        }
    }
}
