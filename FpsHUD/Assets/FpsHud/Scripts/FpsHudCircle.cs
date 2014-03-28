using UnityEngine;
using System.Collections;

public class FpsHudCircle : FpsHudReticule
{
    float previousSpread = float.MaxValue;

    public int Size = 64;
    public Transform TopLeft;
    public Transform TopRight;
    public Transform BottomLeft;
    public Transform BottomRight;

    Vector3 topLeftOffset;
    Vector3 topRightOffset;
    Vector3 bottomLeftOffset;
    Vector3 bottomRightOffset;

    Vector3 topLeftVector = new Vector3(-1, 1).normalized;
    Vector3 topRightVector = new Vector3(1, 1).normalized;
    Vector3 bottomLeftVector = new Vector3(-1, -1).normalized;
    Vector3 bottomRightVector = new Vector3(1, -1).normalized;

    void Start()
    {
        topLeftOffset = new Vector3(-Size, Size, 1);
        topRightOffset = new Vector3(0, Size, 1);
        bottomLeftOffset = new Vector3(-Size, 0, 1);
        bottomRightOffset = new Vector3(0, 0, 1);
    }

    void Update()
    {
        Spread = Mathf.Clamp01(Spread);

        if (Spread != previousSpread)
        {
            int pixelSpread = Mathf.Clamp(Mathf.RoundToInt(Spread * MaxSpred), MinSpred, MaxSpred);

            TopLeft.position = FpsHudUtils.ToScreenPosition(topLeftOffset + (topLeftVector * pixelSpread));
            TopRight.position = FpsHudUtils.ToScreenPosition(topRightOffset + (topRightVector * pixelSpread));
            BottomLeft.position = FpsHudUtils.ToScreenPosition(bottomLeftOffset + (bottomLeftVector * pixelSpread));
            BottomRight.position = FpsHudUtils.ToScreenPosition(bottomRightOffset + (bottomRightVector * pixelSpread));

            previousSpread = Spread;
        }
    }
}
