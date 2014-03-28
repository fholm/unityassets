using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BattleTextRenderer))]
public class BattleTextRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BattleTextRenderer text = target as BattleTextRenderer;

        if (text != null)
        {
            EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);
            text.IsAnimated = EditorGUILayout.Toggle("Is Animated", text.IsAnimated);

            /*
            if (text.IsAnimated)
            {
                text.AnimateInWorld = EditorGUILayout.Toggle("Animate In World", text.AnimateInWorld);
            }
            */

            EditorGUILayout.LabelField("Font Settings", EditorStyles.boldLabel);
            text.FontDefinition = (TextAsset)EditorGUILayout.ObjectField("Font Definition", text.FontDefinition, typeof(TextAsset), false);
            text.FontMaterial = (Material)EditorGUILayout.ObjectField("Font Material", text.FontMaterial, typeof(Material), false);
            text.FontDefinitionType = (BattleTextFont.DefinitionType)EditorGUILayout.EnumPopup("Font Definition Type", text.FontDefinitionType);
            text.GlyphSpacing = EditorGUILayout.FloatField("Glyph Spacing", text.GlyphSpacing);

            if (!text.IsAnimated)
            {
                text.GlyphSize = EditorGUILayout.FloatField("Glyph Size", text.GlyphSize);
            }

            if (!text.IsAnimated)
            {
                EditorGUILayout.LabelField("Orentation Settings", EditorStyles.boldLabel);
                text.LookAtMainCamera = EditorGUILayout.Toggle("Look at Main Camera", text.LookAtMainCamera);
                text.LockXRotation = EditorGUILayout.Toggle("Lock X Rotation", text.LockXRotation);
            }

            if (!text.IsAnimated)
            {
                EditorGUILayout.LabelField("Display Settings", EditorStyles.boldLabel);
                text.InitialText = EditorGUILayout.TextField("Default Text", text.InitialText);
                text.Color = EditorGUILayout.ColorField("Default Color", text.Color);
                text.Visible = EditorGUILayout.Toggle("Visible", text.Visible);
            }
            
            if (GUI.changed)
            {
                if (!Application.isPlaying)
                {
                    EditorUtility.SetDirty(text);
                }
            }
        }
    }
}