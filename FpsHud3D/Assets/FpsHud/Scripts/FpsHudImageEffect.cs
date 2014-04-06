using UnityEngine;
using System.Collections;

public class FpsHudImageEffect : ImageEffectBase
{
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetTexture("_HudTex", FpsHudPerspectiveCamera.Texture);
        Graphics.Blit(source, destination, material);
    }
}