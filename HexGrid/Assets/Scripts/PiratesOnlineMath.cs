using UnityEngine;
using System.Collections;

public static class PiratesOnlineMath
{
    public const float Sqrt3 = 1.732050808f;

    public static float Inradius(float sideLength)
    {
        return 0.5f * Sqrt3 * sideLength;
    }

    public static float GridWidth(int count)
    {
        return PiratesOnlineConstants.SideLength * 1.5f * count;
    }

    public static float GridHeight(int count)
    {
        return PiratesOnlineConstants.Inradius * 2.0f * count;
    }

    public static short Clamp(short value, short min, short max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}