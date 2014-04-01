using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PiratesOnlineHexagonGrid))]
public class PiratesOnlineHexagonGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
        {
            PiratesOnlineHexagonGrid grid = target as PiratesOnlineHexagonGrid;

            if (grid != null)
            {
                grid.GridX = (short)EditorGUILayout.IntField("Grid X", grid.GridX);
                grid.GridZ = (short)EditorGUILayout.IntField("Grid Z", grid.GridZ);
            }
        }
    }
}