using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FogOfWar : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vs;
    Vector2[] uvs;

    int vertCount;
    int frameCount;
    float nodeSize;
    float halfPlaneSize;
    float revealValue = 0f;

    [SerializeField]
    int planeSize = 30;

    [SerializeField]
    float nodeResolution = 1f;

    [SerializeField]
    float fogInstantDarkness = 0.75f;

    [SerializeField]
    bool useFadingOrBlockyShader = false;

    [SerializeField]
    int updateEveryNFrames = 10;

    [SerializeField]
    Terrain terrain = null;

    [HideInInspector]
    public bool UpdateFog = true;

    [HideInInspector]
    public bool ClearAll = false;

    void Start()
    {
        if (useFadingOrBlockyShader)
        {
            fogInstantDarkness = -100f;
        }

        frameCount = updateEveryNFrames;

        halfPlaneSize = (float)planeSize * 0.5f;
        nodeSize = 1f / (float)nodeResolution;
        vertCount = ((int)(planeSize * nodeResolution)) + 1;

        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        Vector3 offset = new Vector3(-halfPlaneSize, 0, -halfPlaneSize);
        Vector3[] vs = this.vs = new Vector3[vertCount * vertCount];
        Vector2[] uvs = this.uvs = new Vector2[vs.Length];
        int[] tris = new int[(vertCount - 1) * (vertCount - 1) * 6];

        for (int r = 0; r < vertCount; ++r)
        {
            for (int c = 0; c < vertCount; ++c)
            {
                int i = r + (vertCount * c);

                vs[i] = offset + new Vector3(r * nodeSize, 0f, c * nodeSize);

                if (terrain != null)
                {
                    vs[i].y = terrain.SampleHeight(transform.TransformPoint(new Vector3(vs[i].x, 0, vs[i].z)));
                }
            }
        }

        if (useFadingOrBlockyShader)
        {
            for (int i = 0; i < vs.Length; ++i)
            {
                uvs[i] = new Vector2(-100f, -100f);
            }
        }
        else
        {
            for (int i = 0; i < vs.Length; ++i)
            {
                uvs[i] = new Vector2(fogInstantDarkness, fogInstantDarkness);
            }
        }

        int t = 0;

        for (int y = 0; y < vertCount - 1; y++)
        {
            for (int x = 0; x < vertCount - 1; x++)
            {
                tris[t] = (y * vertCount) + x;
                tris[t + 1] = ((y + 1) * vertCount) + x;
                tris[t + 2] = (y * vertCount) + x + 1;

                tris[t + 3] = ((y + 1) * vertCount) + x;
                tris[t + 4] = ((y + 1) * vertCount) + x + 1;
                tris[t + 5] = (y * vertCount) + x + 1;
                t += 6;
            }
        }

        mesh.vertices = vs;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
    }

    void Update()
    {
        frameCount += 1;
        UpdateFog = frameCount >= updateEveryNFrames || UpdateFog || ClearAll;

        if (ClearAll)
        {
            for (int i = 0; i < uvs.Length; ++i)
            {
                uvs[i].x = fogInstantDarkness;
            }

            ClearAll = false;
        }

        if (UpdateFog)
        {
            if (useFadingOrBlockyShader)
            {
                revealValue = Time.time;
            }
            else
            {
                //If we're updating fog, make it all black
                for (int i = 0; i < uvs.Length; ++i)
                {
                    uvs[i].x = fogInstantDarkness;
                }
            }
        }

        renderer.material.SetFloat("_WorldTime", revealValue);
    }

    void LateUpdate()
    {
        if (UpdateFog)
        {
            // Set UVs
            mesh.uv = uvs;

            // Clear frame count
            frameCount = 0;

            // Clear upate flag
            UpdateFog = false;
        }
    }

    public bool IsVisible(Vector3 pos)
    {
        pos = transform.InverseTransformPoint(pos);

        int r = Mathf.RoundToInt(((planeSize / 2f) + pos.x) * nodeResolution);
        int c = Mathf.RoundToInt(((planeSize / 2f) + pos.z) * nodeResolution);
        int center = r + (vertCount * c);

        if (r >= 0 && r < vertCount && c >= 0 && c < vertCount && center < uvs.Length)
        {
            return uvs[center].x != fogInstantDarkness;
        }

        return false;
    }

    public void Reveal(Vector3 pos, float radius)
    {
        pos = transform.InverseTransformPoint(pos);

        int r = Mathf.RoundToInt(((planeSize / 2f) + pos.x) * nodeResolution);
        int c = Mathf.RoundToInt(((planeSize / 2f) + pos.z) * nodeResolution);
        int center = r + (vertCount * c);

        if (r >= 0 && r < vertCount && c >= 0 && c < vertCount && center < uvs.Length)
        {
            radius *= nodeResolution;

            int row = Mathf.Clamp(r - (int)radius, 0, int.MaxValue);
            int col = Mathf.Clamp(c - (int)radius, 0, int.MaxValue);

            int row_end = Mathf.Clamp(r + (int)radius, 0, vertCount);
            int col_end = Mathf.Clamp(c + (int)radius, 0, vertCount);

            float sqr = radius * radius;

            Vector3 v = Vector3.zero;
            Vector3 ct = new Vector3(r, c);

            for (; row < row_end; ++row)
            {
                col = Mathf.Clamp(c - (int)radius, 0, int.MaxValue);

                for (; col < col_end; ++col)
                {
                    v = new Vector3(row, col);

                    if ((ct - v).sqrMagnitude < sqr)
                    {
                        uvs[row + (vertCount * col)].x = revealValue;
                    }
                }
            }
        }
    }
}
