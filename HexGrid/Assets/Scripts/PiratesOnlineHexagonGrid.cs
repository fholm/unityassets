using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PiratesOnlineHexagonGrid : MonoBehaviour
{
    #region Static

    static int gridCount = 0;
    static int[] gridTriangles = null;
    static Vector3[] gridVertices = null;
    static Vector2[] gridTexcoords = null;

    public static Mesh CreatePlaceholderMesh()
    {
        float w = PiratesOnlineConstants.GridWidth;
        float h = PiratesOnlineConstants.GridHeight;

        Mesh mesh = new Mesh();
        mesh.name = "Placeholder Quad";
        mesh.vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(0, 0, h),
                new Vector3(w, 0, h),
                new Vector3(w, 0, 0)
            };

        mesh.uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0)
            };

        mesh.triangles = new int[6] 
            {
                0, 1, 3,
                3, 1, 2
            };

        return mesh;
    }

    public static Mesh CreateMesh()
    {
        if (gridTriangles == null || gridVertices == null || gridTexcoords == null)
        {
            gridTriangles = new int[PiratesOnlineConstants.GridTiles * PiratesOnlineConstants.IndicesPerHexagon];
            gridVertices = new Vector3[PiratesOnlineConstants.GridTiles * PiratesOnlineConstants.VerticesPerHexagon];
            gridTexcoords = new Vector2[PiratesOnlineConstants.GridTiles * PiratesOnlineConstants.VerticesPerHexagon];

            int i = 0;
            float xSpace = 1.5f * PiratesOnlineConstants.SideLength;
            float zSpace = 2.0f * PiratesOnlineConstants.Inradius;

            for (int x = 0; x < PiratesOnlineConstants.GridSize; ++x)
            {
                for (int z = 0; z < PiratesOnlineConstants.GridSize; ++z, ++i)
                {
                    Vector3 pos = new Vector3(x * xSpace, 0, z * zSpace + (x & 1) * PiratesOnlineConstants.Inradius);

                    for (int v = 0; v < PiratesOnlineConstants.VerticesPerHexagon; ++v)
                    {
                        gridVertices[(i * PiratesOnlineConstants.VerticesPerHexagon) + v] = pos + PiratesOnlineHexagon.Vertices[v];
                    }

                    for (int t = 0; t < PiratesOnlineConstants.IndicesPerHexagon; ++t)
                    {
                        gridTriangles[(i * PiratesOnlineConstants.IndicesPerHexagon) + t] = (i * PiratesOnlineConstants.VerticesPerHexagon) + PiratesOnlineHexagon.Indices[t];
                    }

                    System.Array.Copy(PiratesOnlineHexagon.TexCoords, 0, gridTexcoords, i * PiratesOnlineConstants.VerticesPerHexagon, PiratesOnlineConstants.VerticesPerHexagon);
                }
            }
        }

        Mesh mesh = new Mesh();

        mesh.name = PiratesOnlineConstants.GridSize + " x " + PiratesOnlineConstants.GridSize + " Hexagon Grid #" + (gridCount++);
        mesh.vertices = gridVertices;
        mesh.triangles = gridTriangles;
        mesh.uv = gridTexcoords;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    #endregion

    Mesh mesh;
    Vector2[] uv;
    MeshFilter filter;

    public int GridX;
    public int GridZ;

    void Start()
    {
        if (uv == null)
        {
            InitMesh();
        }
    }

    void OnDrawGizmos()
    {
        Vector3 p = transform.position;

        p.x += PiratesOnlineConstants.GridWidth / 2;
        p.z += PiratesOnlineConstants.GridHeight / 2;

        Gizmos.DrawWireCube(p, mesh.bounds.size);
    }

    public void InitMesh()
    {
        // Grab mesh filter
        filter = (MeshFilter)GetComponent(typeof(MeshFilter));

        // Create our mesh
        mesh = CreateMesh();

        // Copy uv array for local use
        uv = mesh.uv;

        // Set mesh on filter
        filter.sharedMesh = mesh;
    }

    public void UpdateMesh(PiratesOnlineNode node)
    {
        // Fast path if we only have one atlas
        if (node.Atlases.Length == 1)
        {
            // Use 0 submeshes
            mesh.subMeshCount = 0;

            // Set uv and triangles for the whole mesh
            mesh.uv = gridTexcoords;
            mesh.triangles = gridTriangles;

            // Set renderer material
            renderer.materials = new Material[1] { PiratesOnlineHexagonWorld.GetAtlasMaterial(node.Atlases[0].Atlas) };

            // TODO: Calculate UV coords
        }

        // Slower path for two or more atlases
        else
        {
            // First, create triangle arrays for all sub atlases
            for (int i = 0; i < node.Atlases.Length; ++i)
            {
                node.Atlases[i].AssignedTiles = 0;
                node.Atlases[i].Triangles = new int[node.Atlases[i].Tiles * PiratesOnlineConstants.IndicesPerHexagon];
            }

            // Secondly, setup triangle indices and uv coords
            for (int i = 0; i < PiratesOnlineConstants.GridTiles; ++i)
            {
                byte subAtlas = node.Tiles[i].SubAtlas;
                PiratesOnlineNode.SubAtlas atlas = node.Atlases[subAtlas];

                // Copy indices into the triangle list of this sub-mesh
                for (int t = 0; t < PiratesOnlineConstants.IndicesPerHexagon; ++t)
                {
                    atlas.Triangles[(atlas.AssignedTiles * PiratesOnlineConstants.IndicesPerHexagon) + t]
                        = (i * PiratesOnlineConstants.VerticesPerHexagon) + PiratesOnlineHexagon.Indices[t];
                }

                // TODO: Calculate UV coords

                // Increase amount of assigned tiles by 1
                node.Atlases[subAtlas].AssignedTiles += (byte)1;
            }

            // Third, set uv coordinates on mesh and assign submesh count
            mesh.uv = uv;
            mesh.subMeshCount = node.Atlases.Length;

            // Also create materials array
            Material[] materials = new Material[node.Atlases.Length];

            // Fourth, set sub mesh triangles and create shared materials array
            for (int i = 0; i < node.Atlases.Length; ++i)
            {
                // Set sub mesh triangles
                mesh.SetTriangles(node.Atlases[i].Triangles, i);

                // Clear triangles array
                node.Atlases[i].Triangles = null;

                // Set material in materials array
                materials[i] = PiratesOnlineHexagonWorld.GetAtlasMaterial(node.Atlases[i].Atlas);
            }

            // Last, assign shared materials to renderer
            renderer.sharedMaterials = materials;
        }
    }
}