using UnityEngine;

public class FpsHudSplatterQuad : MonoBehaviour
{
    Color color;
    Material material;

    [HideInInspector]
    public float StartTime;

    [HideInInspector]
    public float Duration;

    void Start()
    {
        material = renderer.sharedMaterial;
        color = material.GetColor("_TintColor");
    }

    void Update()
    {
        if (renderer.enabled)
        {
            float t = 1f - ((Time.time - StartTime) / Duration);

            if (t > 0.01f)
            {
                material.SetColor("_TintColor", new Color(color.r, color.g, color.b, color.a * t));
            }
            else
            {
                renderer.enabled = false;
            }
        }
    }
}
