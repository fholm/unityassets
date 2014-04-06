using UnityEngine;
using System.Collections;

public class FpsHudFrustrumAnchor : MonoBehaviour
{
    public enum AnchorPoint
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    public Vector3 Offset;
    public float Distance = 100f;
    public AnchorPoint Point;
}