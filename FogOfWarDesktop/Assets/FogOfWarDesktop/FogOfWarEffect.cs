using UnityEngine;
using System.Collections;

public class FogOfWarEffect : ImageEffectBase
{
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture("_FogTex", FogOfWarCamera.Texture);
        Graphics.Blit(source, destination, material);
    }
}
