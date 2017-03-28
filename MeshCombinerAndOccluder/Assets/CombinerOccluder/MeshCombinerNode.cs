using UnityEngine;

public class MeshCombinerNode : MonoBehaviour {
  public Bounds Bounds;

  void OnDrawGizmosSelected() {
    UtilsDebug.DrawGizmosBox(Bounds.center, Bounds.size, Color.green);
  }
}
