using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PiratesOnlineMenuItems
{
    public static Mesh[] MergeMeshes(params CombineInstance[][] targets)
    {
        Mesh[] meshes = new Mesh[targets.Length];

        for (int i = 0; i < meshes.Length; ++i)
        {
            meshes[i] = new Mesh();
            meshes[i].CombineMeshes(targets[i], true, true);
        }

        return meshes;
    }

    static int MaterialIndex(Material[] materials, Material material)
    {
        return ArrayUtility.IndexOf(materials, material);
    }

    static T[] InitArray<T>(int size)
        where T : class, new()
    {
        T[] array = new T[size];

        for (int i = 0; i < size; ++i)
        {
            array[i] = new T();
        }

        return array;
    }

    [MenuItem("Assets/Create/Hexagon")]
    static void CreateHexagon()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = PiratesOnlineHexagon.Vertices;
        mesh.triangles = PiratesOnlineHexagon.Indices;
        mesh.uv = PiratesOnlineHexagon.TexCoords;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        AssetDatabase.CreateAsset(mesh, "Assets/Hexagon.asset");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("GameObject/Flip Winding Order")]
    static void FlipWindingOrder()
    {
        Mesh m = Selection.activeGameObject.GetComponent<MeshFilter>().mesh;
        int[] triangles = m.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int tmp = triangles[i + 2];
            triangles[i + 2] = triangles[i + 1];
            triangles[i + 1] = tmp;
        }

        m.triangles = triangles;

        AssetDatabase.CreateAsset(m, "Assets/FlippedMesh.asset");
        AssetDatabase.SaveAssets();
    }

    [MenuItem("GameObject/Combine Into Submeshes")]
    static void CombineMesh()
    {
        MeshFilter[] filters =
            Selection.gameObjects
                .Select(o => o.GetComponent<MeshFilter>())
                .Where(f => f != null && f.sharedMesh != null && f.GetComponent<MeshRenderer>() != null)
                .ToArray();

        MeshRenderer[] renderers =
            filters
                .Select(f => f.GetComponent<MeshRenderer>())
                .ToArray();

        Material[] materials =
            renderers
                .SelectMany(r => r.sharedMaterials)
                .Distinct()
                .ToArray();

        Mesh[] meshes =
            filters
                .Select(f => f.sharedMesh)
                .ToArray();

        if (filters.Length > 0)
        {
            if (materials.Length == 1)
            {
                MergeMeshes();
            }
            else
            {
                Mesh mesh = new Mesh();
                mesh.subMeshCount = materials.Length;

                List<Vector3> vertices = new List<Vector3>();
                List<Vector2> texcoords = new List<Vector2>();
                List<int>[] triangles = InitArray<List<int>>(mesh.subMeshCount);

                for (int m = 0; m < meshes.Length; ++m)
                {
                    Mesh oldMesh = meshes[m];

                    for (int s = 0; s < oldMesh.subMeshCount; ++s)
                    {
                        int index = MaterialIndex(materials, renderers[m].sharedMaterials[s]);
                        int[] oldMeshTriangles = oldMesh.GetTriangles(s).ToArray();
                        triangles[index].AddRange(oldMeshTriangles.Select(i => i + vertices.Count));
                    }

                    vertices.AddRange(oldMesh.vertices.Select(x => renderers[m].transform.localToWorldMatrix.MultiplyPoint(x)));
                    texcoords.AddRange(oldMesh.uv);
                }

                mesh.vertices = vertices.ToArray();
                mesh.uv = texcoords.ToArray();

                for (int s = 0; s < mesh.subMeshCount; ++s)
                {
                    mesh.SetTriangles(triangles[s].ToArray(), s);
                }

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();

                AssetDatabase.CreateAsset(mesh, "Assets/CombinedMesh.asset");
                AssetDatabase.SaveAssets();
            }
        }
    }

    [MenuItem("GameObject/Clone Mesh")]
    static void CloneMesh()
    {
        MeshFilter filter = Selection.activeGameObject.GetComponent<MeshFilter>();

        if (filter != null)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh != null && mesh.vertexCount > 0)
            {
                Mesh clone = new Mesh();

                clone.vertices = mesh.vertices;
                clone.uv = mesh.uv;
                clone.uv1 = mesh.uv1;
                clone.uv2 = mesh.uv2;
                clone.triangles = mesh.triangles;
                clone.colors = mesh.colors;
                clone.tangents = mesh.tangents;
                clone.normals = mesh.normals;
                clone.boneWeights = mesh.boneWeights;
                clone.bindposes = mesh.bindposes;

                if (mesh.subMeshCount > 1)
                {
                    clone.subMeshCount = mesh.subMeshCount;

                    for (int i = 0; i < mesh.subMeshCount; ++i)
                    {
                        clone.SetTriangles(mesh.GetTriangles(i), i);
                    }
                }

                AssetDatabase.CreateAsset(clone, "Assets/ClonedMesh.asset");
                AssetDatabase.SaveAssets();
            }
        }
    }

    [MenuItem("GameObject/Merge Into One Mesh")]
    static void MergeIntoOneMeshes()
    {
        if (Selection.gameObjects.Length > 0)
        {
            GameObject[] objects =
                Selection.gameObjects
                    .Where(x => x.GetComponent<MeshFilter>() != null)
                    .ToArray();

            Mesh[] meshes =
                objects
                    .Select(x => x.GetComponent<MeshFilter>().sharedMesh)
                    .ToArray();
            if (meshes.Length > 0)
            {
                CombineInstance[] combine = new CombineInstance[meshes.Length];

                for (int i = 0; i < combine.Length; ++i)
                {
                    combine[i].mesh = meshes[i];
                    combine[i].transform = objects[i].transform.localToWorldMatrix;
                }

                Mesh m = new Mesh();

                m.CombineMeshes(combine, true, true);
                m.RecalculateBounds();
                m.RecalculateNormals();

                AssetDatabase.CreateAsset(m, "Assets/MergedMesh.asset");
                AssetDatabase.SaveAssets();
            }
        }
    }
}
