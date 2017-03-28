using UnityEngine;

public static class UtilsDebug {
  static public void DrawGizmosBox(Transform transform, Vector3 size) {
    DrawGizmosBox(transform, size, false);
  }

  static public void DrawGizmosBox(Transform transform, Vector3 size, bool selected) {
    DrawGizmosBox(transform, size, selected, new Color(0.0f, 0.7f, 1f));
  }

  static public void DrawGizmosBox(Transform transform, Vector3 size, Color color) {
    DrawGizmosBox(transform, size, false, color);
  }

  static public void DrawGizmosBox(Transform transform, Vector3 size, bool selected, Color color) {
    DrawGizmosBox(transform, size, selected, color, Vector3.zero);
  }

  static public void DrawGizmosBox(Transform transform, Vector3 size, bool selected, Color color, Vector3 offset) {
    color.a = selected ? 0.3f : 0.1f;

    Gizmos.color = color;
    Gizmos.matrix = transform.localToWorldMatrix;
    Gizmos.DrawCube(offset, size);

    color.a = selected ? 0.5f : 0.3f;

    Gizmos.color = color;
    Gizmos.DrawWireCube(offset, size);
    Gizmos.matrix = Matrix4x4.identity;
  }

  static public void DrawGizmosBox(Vector3 center, Vector3 size, Color color) {
    color.a = 0.5f;

    Gizmos.color = color;
    Gizmos.DrawCube(center, size);

    color.a = 0.75f;

    Gizmos.color = color;
    Gizmos.DrawWireCube(center, size);
  }
}
