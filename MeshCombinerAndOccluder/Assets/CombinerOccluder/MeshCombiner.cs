using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshCombiner : MonoBehaviour {

  class MaterialsDescriptor {
    public int[] Ids;

    public MaterialsDescriptor() {
      Ids = new int[0];
    }

    public MaterialsDescriptor(int[] ids) {
      Ids = ids;
    }

    public MaterialsDescriptor(Renderer renderer)
      : this(renderer.sharedMaterials) {
    }

    public MaterialsDescriptor(Material[] materials) {
      Ids = Utils.Map(materials, x => x.GetInstanceID());
    }

    public override Boolean Equals(System.Object obj) {
      var other = obj as MaterialsDescriptor;
      if (other == null) {
        return false;
      }

      if (other.Ids.Length != this.Ids.Length) {
        return false;
      }

      for (int i = 0; i < this.Ids.Length; ++i) {
        if (this.Ids[i] != other.Ids[i]) {
          return false;
        }
      }

      return true;
    }

    public override Int32 GetHashCode() {
      int hash = 17;

      for (int i = 0; i < Ids.Length; ++i) {
        hash = hash * 31 + Ids[i];
      }

      return hash;
    }
  }

  public int NodeSize;
  public int NodeCount;
  public float NodeExpand;
  public GameObject TargetRoot;
  public GameObject DuplicateRoot;
  public List<MeshCombinerNode> Nodes;

#if UNITY_EDITOR
  void OnDrawGizmosSelected() {

    for (int z = 0; z < NodeCount; ++z) {
      for (int x = 0; x < NodeCount; ++x) {
        var zn = z * NodeSize + (NodeSize / 2f);
        var xn = x * NodeSize + (NodeSize / 2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + new Vector3(zn, 0, xn), 0.1f);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position + new Vector3(zn, NodeSize / 2, xn), new Vector3(NodeSize, NodeSize, NodeSize));
      }
    }
  }

  public void Reset() {
    foreach (var n in Nodes.ToArray()) {
      DestroyImmediate(n.gameObject);
    }

    while (DuplicateRoot.transform.childCount > 0) {
      DestroyImmediate(DuplicateRoot.transform.GetChild(0).gameObject);
    }

    // clear nodes
    Nodes.Clear();

    // re-enable target root
    TargetRoot.SetActive(true);


    var scene_path = Path.GetDirectoryName(SceneManager.GetActiveScene().path);
    var meshes_folder = SceneManager.GetActiveScene().name + "_Meshes";
    var meshes_path = scene_path + "/" + meshes_folder;

    // make sure folder exists
    if (AssetDatabase.IsValidFolder(meshes_path)) {
      foreach (var mesh in AssetDatabase.LoadAllAssetsAtPath(meshes_path)) {
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(mesh));
      }
    }
  }

  List<GameObject> FindLightsAndParticles() {
    List<GameObject> result = new List<GameObject>();

    FindLightsAndParticles(TargetRoot.transform, result);

    return result;
  }

  void FindLightsAndParticles(Transform transform, List<GameObject> result) {
    if (transform.GetComponent<ParticleSystem>() || transform.GetComponent<Light>()) {
      result.Add(transform.gameObject);
      return;
    }

    for (int i = 0; i < transform.childCount; ++i) {
      FindLightsAndParticles(transform.GetChild(i), result);
    }
  }

  public void Combine() {
    Reset();

    var renderers = TargetRoot.GetComponentsInChildren<MeshRenderer>().Where(x => x.gameObject.isStatic).ToArray();
    var n = 0f;

    for (int z = 0; z < NodeCount; ++z) {
      for (int x = 0; x < NodeCount; ++x) {
        CombineNode(renderers, x, z);

        EditorUtility.DisplayProgressBar("Mesh Combiner", "Mesh Combiner", ++n / (NodeCount * NodeCount));
      }
    }

    foreach (var duplicate in FindLightsAndParticles()) { // TargetRoot.GetComponentsInChildren<MeshCombinerDuplicate>(true)) {
      var d = Instantiate(duplicate);

      d.transform.position = duplicate.transform.position;
      d.transform.rotation = duplicate.transform.rotation;
      d.transform.localScale = duplicate.transform.localScale;
      d.transform.SetParent(DuplicateRoot.transform, true);
    }

    // remove progress bar
    EditorUtility.ClearProgressBar();

    // disable root
    TargetRoot.SetActive(false);
  }

  public Bounds CalculateBounds(Int32 x, Int32 z) {
    var center = transform.position;

    center.x += (x * NodeSize);
    center.z += (z * NodeSize);

    return new Bounds(center + new Vector3(NodeSize / 2, 0, NodeSize / 2), new Vector3(NodeSize, NodeSize, NodeSize));
  }

  void CombineNode(MeshRenderer[] renderers, Int32 x, Int32 z) {
    var bounds = CalculateBounds(x, z);
    var groups = new Dictionary<MaterialsDescriptor, List<MeshRenderer>>();

    for (int i = 0; i < renderers.Length; ++i) {
      if (bounds.Contains(renderers[i].transform.position)) {
        var descriptor = new MaterialsDescriptor(renderers[i]);
        var group = default(List<MeshRenderer>);

        if (groups.TryGetValue(descriptor, out group) == false) {
          groups.Add(descriptor, group = new List<MeshRenderer>());
        }

        group.Add(renderers[i]);
      }
    }

    if (groups.Count > 0) {
      Debug.LogFormat("[MeshCombiner] Found {0} material groups in node ({1}, {2})", groups.Count, x, z);

      var node_go = new GameObject(string.Format("[MeshCombiner]_Node_{0}_{1}", x, z));
      node_go.transform.position = bounds.center;
      node_go.transform.rotation = Quaternion.identity;
      node_go.transform.localScale = Vector3.one;
      node_go.isStatic = true;

      var groups_array = groups.ToArray();

      for (int i = 0; i < groups_array.Length; ++i) {
        var desc = groups_array[i].Key;
        var group = groups_array[i].Value;

        if (group.Count > 0) {
          Debug.LogFormat("[MeshCombiner] Generating mesh for material group {0} in node ({1}, {2}) (mesh count: {3})", i, x, z, group.Count);

          var group_go = new GameObject(string.Format("Group_{0}", i));
          group_go.transform.parent = node_go.transform;
          group_go.transform.rotation = Quaternion.identity;
          group_go.transform.localScale = Vector3.one;
          group_go.transform.localPosition = Vector3.zero;
          group_go.isStatic = true;

          // generate all sub mesh combines
          var group_sm_combine = Utils.InitArray(desc.Ids.Length, _ => new List<CombineInstance>());

          // fore each renderer in the group
          for (int k = 0; k < group.Count; ++k) {
            var mf = group[k].GetComponent<MeshFilter>();

            // generate a combine instance for each submesh of the mesh it's rendering
            for (int n = 0; n < desc.Ids.Length; ++n) {
              var mi = new CombineInstance();

              // assign mesh
              mi.mesh = mf.sharedMesh;

              // grab submesh index
              mi.subMeshIndex = n;

              // use only l2w transform here
              mi.transform = mf.transform.localToWorldMatrix;

              group_sm_combine[n].Add(mi);
            }
          }

          // generate all sub-meshes (intermediate step)
          var group_sm = new Mesh[group_sm_combine.Length];

          for (int k = 0; k < group_sm.Length; ++k) {
            group_sm[k] = new Mesh();
            group_sm[k].CombineMeshes(group_sm_combine[k].ToArray(), true, true);
          }

          // generate main mesh
          var group_m_combine = new CombineInstance[group_sm.Length];

          for (int k = 0; k < group_sm.Length; ++k) {
            group_m_combine[k] = new CombineInstance();
            group_m_combine[k].mesh = group_sm[k];
            group_m_combine[k].subMeshIndex = 0;
            group_m_combine[k].transform = group_go.transform.worldToLocalMatrix;
          }

          var group_m = new Mesh();
          group_m.name = node_go.name + "_" + group_go.name;
          group_m.CombineMeshes(group_m_combine, false, true);

          // add filter + renderer
          var group_mf = group_go.AddComponent<MeshFilter>();
          var group_mr = group_go.AddComponent<MeshRenderer>();

          // setup asset paths
          var scene_path = Path.GetDirectoryName(SceneManager.GetActiveScene().path);
          var meshes_folder = SceneManager.GetActiveScene().name + "_Meshes";
          var meshes_path = scene_path + "/" + meshes_folder;
          var asset_path = meshes_path + "/" + group_m.name + ".asset";

          // make sure folder exists
          if (AssetDatabase.IsValidFolder(meshes_path) == false) {
            AssetDatabase.CreateFolder(scene_path, meshes_folder);
          }

          // generate UV2s
          Unwrapping.GenerateSecondaryUVSet(group_m);

          // create asset
          AssetDatabase.CreateAsset(group_m, asset_path);

          // store on mesh
          group_mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(asset_path);
          group_mr.sharedMaterials = Utils.Map(desc.Ids, k => EditorUtility.InstanceIDToObject(k)).OfType<Material>().ToArray();
        }
      }

      MeshCombinerNode node;

      node = node_go.AddComponent<MeshCombinerNode>();
      node.Bounds = new Bounds(node_go.transform.position, new Vector3(1, 1, 1));

      foreach (var mf in node.GetComponentsInChildren<MeshFilter>()) {
        var vs = mf.sharedMesh.vertices;

        for (int i = 0; i < vs.Length; ++i) {
          node.Bounds.Encapsulate(node.transform.TransformPoint(vs[i]));
        }
      }

      node.Bounds.Expand(NodeExpand);


      Vector3 center;

      center = node.Bounds.center;
      center.y = node.Bounds.size.y * 0.5f;

      node.Bounds.center = center;

      Nodes.Add(node);

      node_go.transform.parent = transform;
    }
  }
#endif
}
