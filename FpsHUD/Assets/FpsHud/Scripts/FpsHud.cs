using UnityEngine;
using System.Collections;

public class FpsHud : MonoBehaviour
{
    static FpsHud instance = null;

    public static FpsHud Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FpsHud)) as FpsHud;
            }

            return instance;
        }
    }

    public Camera PlayerCamera;
    public Camera ActiveCamera;
    public Color PrimaryColor = Color.white;
    public Color SecondaryColor = new Color(64f / 255f, 143f / 255f, 169f / 255f, 51f / 255f);

    void Start()
    {
        PlayerCamera.depthTextureMode = DepthTextureMode.Depth;
        ActiveCamera = PlayerCamera;
        UpdateColors(PrimaryColor, SecondaryColor);
    }

    void SetColor(string child, string property, Color color)
    {
        Transform t = transform.Find(child);

        if (t && t.renderer && t.renderer.sharedMaterial)
        {
            t.renderer.sharedMaterial.SetColor(property, color);
        }
    }

    public void UpdateColors(Color primary, Color secondary)
    {
        PrimaryColor = primary;
        SecondaryColor = secondary;

        SetColor("Compass/Arrow", "_TintColor", primary);
        SetColor("Compass/Background", "_TintColor", primary);
        SetColor("Health/Beat", "_TintColor", primary);

        SetColor("Health/BeatBackground", "_TintColor", secondary);
        SetColor("Clip/BulletMarker(Clone)", "_TintColor", secondary);
        SetColor("MiniMap/Background", "_TintColor", secondary);
    }
}