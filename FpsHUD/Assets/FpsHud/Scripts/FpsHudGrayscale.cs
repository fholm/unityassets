using UnityEngine;

public class FpsHudGrayscale : MonoBehaviour
{
    static FpsHudGrayscale instance = null;

    public static FpsHudGrayscale Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FpsHudGrayscale)) as FpsHudGrayscale;
            }

            return instance;
        }
    }

    Material material;

    public Shader Shader = null;
    public float Amount = 0f;

    void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if (!Shader || !Shader.isSupported)
        {
            enabled = false;
            return;
        }

        instance = this;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        GetMaterial().SetFloat("_Amount", Mathf.Clamp01(Amount));
        Graphics.Blit(source, destination, GetMaterial());
    }

    void OnDisable()
    {
        if (material)
        {
            DestroyImmediate(material);
        }
    }

    Material GetMaterial()
    {
        if (material == null)
        {
            material = new Material(Shader);
            material.hideFlags = HideFlags.HideAndDontSave;
        }

        return material;
    }
}