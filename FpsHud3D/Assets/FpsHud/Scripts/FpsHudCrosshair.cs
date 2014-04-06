using UnityEngine;
using System.Collections;

public class FpsHudCrosshair : FpsHudReticule
{
    float previousSpread = float.MaxValue;

    public int PixelWidth = 4;
    public int PixelHeight = 11;

    public Transform Left;
    public Transform Right;
    public Transform Top;
    public Transform Bottom;

    void Start()
    {
        Top.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.CrosshairColor);
    }

    void Update()
    {
        Spread = Mathf.Clamp01(Spread);

        if (Spread != previousSpread)
        {
            int pixelSpread = Mathf.Clamp(Mathf.RoundToInt(Spread * MaxSpred), MinSpred, MaxSpred);

            Left.position = FpsHudUtils.ToScreenPosition(new Vector3(-PixelHeight - pixelSpread, (PixelWidth / 2), 1));
            Right.position = FpsHudUtils.ToScreenPosition(new Vector3(pixelSpread, (PixelWidth / 2), 1));
            Top.position = FpsHudUtils.ToScreenPosition(new Vector3(-(PixelWidth / 2), PixelHeight + pixelSpread, 1));
            Bottom.position = FpsHudUtils.ToScreenPosition(new Vector3(-(PixelWidth / 2), -pixelSpread, 1));

            previousSpread = Spread;
        }
    }
}