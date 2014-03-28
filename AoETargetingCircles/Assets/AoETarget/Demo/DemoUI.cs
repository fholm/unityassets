using UnityEngine;
using System.Collections;

public class DemoUI : MonoBehaviour
{
    [SerializeField]
    GameObject alphaBlend = null;

    [SerializeField]
    GameObject colorBlend = null;

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginVertical();

        if (GUILayout.Button("Alpha Blended") || Input.GetKeyUp(KeyCode.Alpha1))
        {
            alphaBlend.SetActiveRecursively(true);
            colorBlend.SetActiveRecursively(false);
        }

        if (GUILayout.Button("Color Blended") || Input.GetKeyUp(KeyCode.Alpha2))
        {
            colorBlend.SetActiveRecursively(true);
            alphaBlend.SetActiveRecursively(false);
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}