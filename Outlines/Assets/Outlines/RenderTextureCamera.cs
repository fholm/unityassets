using System.Collections;
using UnityEngine;

public abstract class RenderTextureCamera<T> : SingletonObject<T> where T : SingletonObject {
  const int WAIT_FRAMES = 10;

  [SerializeField]
  Camera targetCamera;

  public Camera renderTextureCamera {
    get { return targetCamera; }
  }

  public abstract int ScreenWidth { get; }
  public abstract int ScreenHeight { get; }

  public virtual int Depth { get { return 0; } }
  public virtual RenderTextureFormat TextureFormat { get { return RenderTextureFormat.ARGBFloat; } }

  protected virtual void OnRenderTextureCreated() {

  }

  protected void Start() {
    CreateRenderTexture();
  }

  void OnEnable() {
  }

  void OnDisable() {
  }

  void OnDestroy() {
    OnDisable();
  }

  IEnumerator ReleaseRenderTexture(RenderTexture rt) {
    for (int i = 0; i < WAIT_FRAMES; ++i) {
      yield return null;
    }

    if (rt) {
      rt.Release();
    }
  }

  IEnumerator UpdateRenderTexture() {
    for (int i = 0; i < WAIT_FRAMES; ++i) {
      yield return null;
    }

    CreateRenderTexture();
  }

  void CreateRenderTexture() {
    targetCamera.enabled = true;
    targetCamera.targetTexture = new RenderTexture(ScreenWidth, ScreenHeight, Depth, TextureFormat);
    OnRenderTextureCreated();
  }
}
