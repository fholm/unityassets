using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MeshCombinerOcclusion : MonoBehaviour {
  public bool VisualizeInEditor;

  public Camera Camera;
  public MeshCombiner Combiner;

  void OnPreCull() {
#if UNITY_EDITOR
    OnPreCull_Editor();
#else
    OnPreCull_Normal();
#endif
  }

  void OnPreCull_Editor() {
#if UNITY_EDITOR
    if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused) {
      OnPreCull_Normal();
    }
    else {
      if (VisualizeInEditor) {
        OnPreCull_Normal();
      }
      else {
        if (Combiner) {
          for (int i = 0; i < Combiner.Nodes.Count; ++i) {
            Combiner.Nodes[i].gameObject.Show();
          }
        }
      }
    }
#endif
  }

  void OnPreCull_Normal() {
    if (Combiner) {
      var planes = GeometryUtility.CalculateFrustumPlanes(Camera);

      for (int i = 0; i < Combiner.Nodes.Count; ++i) {
        Combiner.Nodes[i].gameObject.Toggle(GeometryUtility.TestPlanesAABB(planes, Combiner.Nodes[i].Bounds));
      }
    }
  }
}
