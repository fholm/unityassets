using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
public class OverlayEffect : MonoBehaviour {
  [SerializeField]
  OverlayCamera overlayCamera;

  [SerializeField]
  Shader overlayShader;

  [NonSerialized]
  Material overlayMaterial;

  [SerializeField]
  [Range(0, 1)]
  Int32 MinCompare;

  void OnRenderImage(RenderTexture source, RenderTexture destination) {
    if (!overlayMaterial) {
      overlayMaterial = new Material(overlayShader);
    }

    // push overlay mask into material
    overlayMaterial.SetFloat("_MinCompare", MinCompare);
    overlayMaterial.SetFloat("_ScreenWidth", Screen.width);
    overlayMaterial.SetFloat("_ScreenHeight", Screen.height);
    overlayMaterial.SetTexture("_OverlayTex", overlayCamera.RenderTexture);

    // render it
    Graphics.Blit(source, destination, overlayMaterial);
  }
}
