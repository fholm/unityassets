using UnityEngine;
using System.Collections;

public static class FpsHudUtils
{
    public static Vector3 ToScreenPosition(this Vector3 pos)
    {
        pos.x = Mathf.RoundToInt(pos.x);
        pos.y = Mathf.RoundToInt(pos.y);

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsWebPlayer:
            case RuntimePlatform.XBOX360:
                pos.x += 0.5f;
                pos.y += 0.5f;
                break;
        }

        return pos;
    }

    public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return (float)Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
    }
}