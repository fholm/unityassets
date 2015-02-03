using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class DumpMesh {
  [MenuItem("Assets/Create/Dump Mesh To C#")]
  public static void Dump() {
    if (Selection.activeObject is Mesh) {
      Dump(Selection.activeObject as Mesh);
    }
  }

  public static void Dump(Mesh original) {

    Mesh m = new Mesh();
    m.vertices = original.vertices;
    m.triangles = original.triangles;
    m.Optimize();

    var path = Path.Combine("Assets", m.name + "_Mesh.cs");
    var sb = new StringBuilder();

    var v = m.vertices;
    var t = m.triangles;

    sb.AppendLine("using UnityEngine;");

    // BEGIN CLASS
    sb.AppendLine(string.Format("public static class {0}_Mesh {{", m.name));

    // VERTICES
    sb.AppendLine(string.Format("public static readonly Vector3[] Vertices = new Vector3[{0}] {{", v.Length));
    for (int i = 0; i < v.Length; ++i) {
      sb.AppendLine(string.Format("new Vector3({0}f, {1}f, {2}f), ", v[i].x, v[i].y, v[i].z));
    }

    sb.AppendLine("};");

    // TRIANGLES
    sb.AppendLine(string.Format("public static readonly int[] Triangles = new int[{0}] {{", t.Length));
    for (int i = 0; i < t.Length; ++i) {
      sb.AppendLine(t[i].ToString() + ", ");
    }
    sb.AppendLine("};");

    // END CLASS
    sb.AppendLine("}");

    File.WriteAllText(path, sb.ToString());
  }
}
