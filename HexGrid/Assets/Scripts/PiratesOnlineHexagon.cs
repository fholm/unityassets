using UnityEngine;
using System.Linq;

public static class PiratesOnlineHexagon
{
    public static readonly Vector3[] Vertices
        = new Vector3[6]
        {
            new Vector3(1, 0, 0),
            new Vector3(0.5f, 0, -0.8660254f),
            new Vector3(-0.5f, 0, -0.8660254f),
            new Vector3(-1, 0, 0),
            new Vector3(-0.5f, 0, 0.8660254f),
            new Vector3(0.5f, 0, 0.8660254f)
        };

    public static readonly int[] Indices
        = new int[12]
        {
            0, 1, 2,
            2, 5, 0,
            2, 3, 5,
            3, 4, 5
        };

    public static readonly Vector2[] TexCoords
        = new Vector2[]
        {
            new Vector2(1, 0.5f),
            new Vector2(0.75f, 0.0669873f),
            new Vector2(0.25f, 0.0669873f),
            new Vector2(0, 0.5f),
            new Vector2(0.25f, 0.9330127f),
            new Vector2(0.75f, 0.9330127f)
        };
}
