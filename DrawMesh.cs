using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DrawMesh : MonoBehaviour {
  Material material;

  void OnPostRender() {
    if (material == null) {
      material = new Material("Shader \"DrawMesh\" {\n" +
        "Properties { _Color (\"Main Color\", Color) = ( 1, 1, 1, 1 ) }\n" +
        "SubShader { Pass { ZWrite Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha\n" +
        "Colormask RGBA Lighting Off Color[_Color]\n" +
        "} } }");
    }

    material.color = new Color(1f, 0f, 0f, 0.15f);
    material.SetPass(0);
    DrawSolid(SphereMesh.Vertices, SphereMesh.Triangles);

    material.color = new Color(1f, 0f, 0f, 0.75f);
    material.SetPass(0);
    DrawWireFrame(SphereMesh.Vertices, SphereMesh.Triangles);
  }

  void DrawWireFrame(Vector3[] v, int[] t) {
    GL.Begin(GL.LINES);

    for (int i = 0; i < t.Length; i += 3) {
      Vector3 v0 = v[t[i + 0]];
      Vector3 v1 = v[t[i + 1]];
      Vector3 v2 = v[t[i + 2]];

      GL.Vertex(v0);
      GL.Vertex(v1);
      GL.Vertex(v1);
      GL.Vertex(v2);
      GL.Vertex(v2);
      GL.Vertex(v0);
    }

    GL.End();
  }

  void DrawSolid(Vector3[] v, int[] t) {
    GL.Begin(GL.TRIANGLES);

    for (int i = 0; i < t.Length; i += 3) {
      Vector3 v0 = v[t[i + 0]];
      Vector3 v1 = v[t[i + 1]];
      Vector3 v2 = v[t[i + 2]];

      GL.Vertex(v0);
      GL.Vertex(v1);
      GL.Vertex(v2);
    }

    GL.End();
  }
}
