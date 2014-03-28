using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FogOfWar : MonoBehaviour
{
    Mesh mesh = null;
    GameObject plane = null;

    Vector2[] fogData = null;
    float[] fogHeight = null;

    Vector2[] planeData = null;
    Vector3[] planeVertices = null;

    float quadSize = 0f;
    float halfPlaneSize = 0f;
    float halfWorldSize = 0f;

    int vertCount = 0;
    int nodeCount = 0;
    int planeTopLeftRow = 0;
    int planeTopLeftCol = 0;

    [SerializeField]
    float fogSize = 256f;

    [SerializeField]
    float planeSize = 80f;

    [SerializeField]
    float quadResolution = 0.5f;

    [SerializeField]
    Material material;

    [SerializeField]
    Transform target;

    [SerializeField]
    float minLightUndiscovered = 0.02f;

    [SerializeField]
    float minLightDiscovered = 0.2f;

    [SerializeField]
    Color tint = new Color(190f / 255f, 92f / 255f, 0f);

    [SerializeField]
    float fadeTime = 3f;

    [SerializeField]
    FogOfWarHeightSampler heightSampler = null;

    [SerializeField]
    FogOfWarLineOfSightChecker losChecker = null;

    [SerializeField]
    int fogOfWarLayer = 31;

    Vector3 worldOrigin { get { return transform.position; } }
    Vector3 planeOrigin { get { return plane.transform.position; } }
    Vector3 targetOrigin { get { return target.position; } }

    public int NodeCount { get { return nodeCount; } }
    public float[] HeightData { get { return fogHeight; } }
    public Vector2[] VisibilityData { get { return fogData; } }

    void Start()
    {
        // Calculate sizes
        quadSize = 1f / quadResolution;
        halfPlaneSize = planeSize * 0.5f;
        halfWorldSize = fogSize * 0.5f;
        vertCount = (int)(planeSize * quadResolution) + 1;
        nodeCount = (int)(fogSize * quadResolution) + 1;

        // Setup plane object
        plane = new GameObject("FogOfWar - Plane");
        plane.layer = fogOfWarLayer;
        plane.AddComponent<MeshFilter>();
        plane.AddComponent<MeshRenderer>();

        // We don't care for shadows
        plane.GetComponent<MeshRenderer>().castShadows = false;
        plane.GetComponent<MeshRenderer>().receiveShadows = false;

        // Set material on plane
        plane.GetComponent<MeshRenderer>().material = material;

        // Grab mesh and clear it
        mesh = plane.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // Offset
        Vector3 o = new Vector3(-halfPlaneSize, 0, halfPlaneSize);

        // Vertices
        Vector3[] vs = this.planeVertices = new Vector3[vertCount * vertCount];

        // UV data
        Vector2[] uv = this.planeData = new Vector2[vs.Length];

        // Triangles
        int[] ts = new int[(vertCount - 1) * (vertCount - 1) * 6];

        // Setup vertices
        for (int r = 0; r < vertCount; ++r)
        {
            for (int c = 0; c < vertCount; ++c)
            {
                vs[(r * vertCount) + c] = o + new Vector3(c * quadSize, 0f, -(r * quadSize));
            }
        }

        // Setup UV data
        for (int i = 0; i < vs.Length; ++i)
        {
            uv[i] = new Vector2(-128f, minLightUndiscovered);
        }

        // Setup triangles
        int t = 0;

        for (int r = 0; r < vertCount - 1; r++)
        {
            for (int c = 0; c < vertCount - 1; c++)
            {
                ts[t] = (r * vertCount) + c;
                ts[t + 1] = (r * vertCount) + c + 1;
                ts[t + 2] = ((r + 1) * vertCount) + c; 

                ts[t + 3] = ((r + 1) * vertCount) + c;
                ts[t + 4] = (r * vertCount) + c + 1;
                ts[t + 5] = ((r + 1) * vertCount) + c + 1;

                t += 6;
            }
        }

        // Set data on plane mesh
        mesh.vertices = vs;
        mesh.uv = uv;
        mesh.triangles = ts;
        mesh.RecalculateNormals();
        
        // Setup fog data
        fogData = new Vector2[nodeCount * nodeCount];

        if (heightSampler != null)
        {
            fogHeight = new float[nodeCount * nodeCount];
        }

        for (int r = 0; r < nodeCount; ++r)
        {
            for (int c = 0; c < nodeCount; ++c)
            {
                fogData[(r * nodeCount) + c] = new Vector2(-256f, minLightUndiscovered);

                if (heightSampler != null)
                {
                    fogHeight[(r * nodeCount) + c] = heightSampler.SampleHeight(FogToWorld(r, c));
                }
            }
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            int r;
            int c;

            WorldToFog(targetOrigin, out r, out c);

            r -= vertCount / 2;
            c -= vertCount / 2;

            r = Mathf.Clamp(r, 0, nodeCount - 1);
            c = Mathf.Clamp(c, 0, nodeCount - 1);

            if (Mathf.Abs(planeTopLeftRow - r) > 5 || Mathf.Abs(planeTopLeftCol - c) > 5)
            {
                int posRow;
                int posCol;

                WorldToFog(targetOrigin, out posRow, out posCol);

                planeTopLeftRow = r;
                planeTopLeftCol = c;

                plane.transform.position = FogToWorld(posRow, posCol);

                copyHeight();
            }

            copyData();

            plane.renderer.material.SetColor("_Color", tint);
            plane.renderer.material.SetFloat("_FadeTime", fadeTime);
            plane.renderer.material.SetFloat("_WorldTime", Time.time);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(fogSize, 1f, fogSize));
    }

    /// <summary>
    /// Checks if a world position is revaled in the fog or not
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>True if revealed, False otherwise</returns>
    public bool IsRevealed(Vector3 pos)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);

        return fogData[(r * nodeCount) + c].x >= Time.time;
    }

    /// <summary>
    /// Checks if a world position has been discovered or not
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>True if discovered, False otherwise</returns>
    public bool IsDiscovered(Vector3 pos)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);

        return fogData[(r * nodeCount) + c].y > minLightUndiscovered || minLightUndiscovered == minLightDiscovered;
    }

    /// <summary>
    /// Checks if a world position has not been discovered
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <returns>True if not discovered, False otherwise</returns>
    public bool IsUndiscovered(Vector3 pos)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);

        return fogData[(r * nodeCount) + c].y < minLightDiscovered;
    }

    /// <summary>
    /// Clamps a position to the closest fog nodes world position
    /// </summary>
    /// <param name="pos">The world position to clamp</param>
    /// <returns>The clamped world position</returns>
    public Vector3 ClampToFog(Vector3 pos)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);

        return FogToWorld(r, c);
    }

    /// <summary>
    /// Converts a world position to fog row and column index
    /// </summary>
    /// <param name="pos">The world position</param>
    /// <param name="r">The resulting row</param>
    /// <param name="c">The resulting column</param>
    public void WorldToFog(Vector3 pos, out int r, out int c)
    {
        pos = transform.InverseTransformPoint(pos);

        c = Mathf.CeilToInt((pos.x + halfWorldSize) * quadResolution);
        r = Mathf.CeilToInt((halfWorldSize - pos.z) * quadResolution);

        c = Mathf.Clamp(c, 0, nodeCount - 1);
        r = Mathf.Clamp(r, 0, nodeCount - 1);
    }

    /// <summary>
    /// Converts a fog row and column to a world position
    /// </summary>
    /// <param name="row">The row to convert</param>
    /// <param name="col">The column to convert</param>
    /// <returns>The resulting world position</returns>
    public Vector3 FogToWorld(int row, int col)
    {
        Vector3 p = transform.position;

        p.x -= halfWorldSize;
        p.z += halfWorldSize;

        p.x += (col * quadSize);
        p.z -= (row * quadSize);

        return p;
    }

    /// <summary>
    /// Reveals the fog of war node closest to this position
    /// </summary>
    /// <param name="pos">The position</param>
    /// <param name="fadeDelay">The fade delay</param>
    public void RevealPosition(Vector3 pos, float fadeDelay)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);
        
        int n = r + (nodeCount * c);

        if (r >= 0 && r < nodeCount && c >= 0 && c < nodeCount && n < fogData.Length)
        {
            fogData[n].x = Time.time + fadeDelay;
            fogData[n].y = minLightDiscovered;
        }
    }

    /// <summary>
    /// Reveals a square of size around a world position
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <param name="fadeDelay"></param>
    public void RevealSquare(Vector3 pos, float size, float fadeDelay)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);

        int index = r + (nodeCount * c);

        size *= quadResolution;

        if (r >= 0 && r < nodeCount && c >= 0 && c < nodeCount && index < fogData.Length)
        {
            int row = Mathf.Clamp(r - (int)size, 0, nodeCount);
            int rowEnd = Mathf.Clamp(r + (int)size, 0, nodeCount);
            int colEnd = Mathf.Clamp(c + (int)size, 0, nodeCount);

            for (; row < rowEnd; ++row)
            {
                int col = Mathf.Clamp(c - (int)size, 0, nodeCount);

                for (; col < colEnd; ++col)
                {
                    int n = (row * nodeCount) + col;
                    fogData[n].x = Time.time + fadeDelay;
                    fogData[n].y = minLightDiscovered;
                }
            }
        }
    }

    public void RevealCircle(Vector3 pos, float radius, float maxHeight, float fadeDelay, float autoRevealRadius)
    {
        int r;
        int c;

        WorldToFog(pos, out r, out c);

        int index = r + (nodeCount * c);

        if (r >= 0 && r < nodeCount && c >= 0 && c < nodeCount && index < fogData.Length)
        {
            radius *= quadResolution;

            int row = Mathf.Clamp(r - (int)radius, 0, int.MaxValue);
            int rowEnd = Mathf.Clamp(r + (int)radius, 0, nodeCount);
            int colEnd = Mathf.Clamp(c + (int)radius, 0, nodeCount);

            float sqrRadius = radius * radius;
            float sqrAutoReveal = autoRevealRadius * autoRevealRadius;
            float time = Time.time + fadeDelay;
            float sqrMagnitude = 0f;

            Vector3 v = Vector3.zero;
            Vector3 ct = new Vector3(c, 0f, r);

            for (; row < rowEnd; ++row)
            {
                int col = Mathf.Clamp(c - (int)radius, 0, int.MaxValue);

                for (; col < colEnd; ++col)
                {
                    v.x = col;
                    v.z = row;

                    sqrMagnitude = (v - ct).sqrMagnitude;

                    if (sqrMagnitude < sqrRadius)
                    {
                        int n = (row * nodeCount) + col;

                        if (sqrMagnitude > sqrAutoReveal)
                        {
                            if (heightSampler != null)
                            {
                                if (fogHeight[n] - pos.y >= maxHeight)
                                {
                                    continue;
                                }

                                if (losChecker != null && !losChecker.HasLineOfSight(this, r, c, row, col, pos.y, maxHeight))
                                {
                                    continue;
                                }
                            }
                        }

                        if (fogData[n].x < time)
                        {
                            fogData[n].x = time;
                            fogData[n].y = minLightDiscovered;
                        }
                    }
                }
            }
        }
    }

    void copyHeight()
    {
        if (heightSampler != null)
        {
            int pRow = 0;
            int pCol = 0;

            for (int fRow = planeTopLeftRow; pRow < vertCount; ++fRow, ++pRow)
            {
                pCol = 0;

                for (int fCol = planeTopLeftCol; pCol < vertCount; ++fCol, ++pCol)
                {
                    planeVertices[(pRow * vertCount) + pCol].y = fogHeight[(fRow * nodeCount) + fCol];
                }
            }

            mesh.vertices = planeVertices;
        }
    }

    void copyData()
    {
        int pRow = 0;
        int pCol = 0;

        for (int fRow = planeTopLeftRow; pRow < vertCount; ++fRow, ++pRow)
        {
            pCol = 0;

            for (int fCol = planeTopLeftCol; pCol < vertCount; ++fCol, ++pCol)
            {
                planeData[(pRow * vertCount) + pCol] = fogData[(fRow * nodeCount) + fCol];
            }
        }

        mesh.uv = planeData;
    }
}