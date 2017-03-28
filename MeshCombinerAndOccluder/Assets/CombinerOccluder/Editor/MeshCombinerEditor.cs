using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor {
  public override void OnInspectorGUI() {

    if (GUILayout.Button("Combine", EditorStyles.miniButton)) {
      (target as MeshCombiner).Combine();

      // mark scene as changed
      EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    if (GUILayout.Button("Reset", EditorStyles.miniButton)) {
      (target as MeshCombiner).Reset();

      // mark scene as changed
      EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    base.OnInspectorGUI();
  }
}
