using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PiratesOnlineHexagonWorld))]
public class PiratesOnlineHexagonWorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            /*
            EditorGUILayout.LabelField("Loaded Grids", PiratesOnlineHexagonWorld.activeGrids.Count.ToString());
            EditorGUILayout.LabelField("Recycled Grids", PiratesOnlineHexagonWorld.recycledGrids.Count.ToString());
            */
        }
    }
}