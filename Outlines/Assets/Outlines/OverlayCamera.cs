using System;
using System.Collections;
using UnityEngine;

public class OverlayCamera : RenderTextureCamera<OverlayCamera> {
  public RenderTexture RenderTexture {
    get { return renderTextureCamera.targetTexture; }
  }

  public override Int32 ScreenWidth {
    get { return Screen.width; }
  }

  public override Int32 ScreenHeight {
    get { return Screen.height; }
  }
}
