using UnityEngine;

public class OverlayMesh : MonoBehaviour {
  Material[] _originalMaterials;

  Renderer _renderer;
  OverlayObject _overlayObject;

  void Start() {
    _renderer = GetComponent<MeshRenderer>();

    if (!_renderer) {
      _renderer = GetComponent<SkinnedMeshRenderer>();
    }

    if (!_renderer) {
      Debug.LogErrorFormat("Could not find 'Renderer' component on {0}", gameObject.name);

      // destroy it
      Destroy(this);
      return;
    }

    _overlayObject = GetComponentInParent<OverlayObject>();

    if (_overlayObject) {
      _overlayObject.Meshes.Add(this);
    }
    else {
      // destroy it
      Destroy(this);
    }
  }

  void OnDestroy() {
    if (_overlayObject) {
      _overlayObject.Meshes.Remove(this);
    }
  }

  void OnWillRenderObject() {
    if (Camera.current == OverlayCamera.Instance.renderTextureCamera) {
      // store materials
      _originalMaterials = _renderer.sharedMaterials;

      // get replacement materials
      _renderer.sharedMaterials = _overlayObject.GetMaterialArray(_originalMaterials.Length);
    }
  }

  void OnRenderObject() {
    if (Camera.current == OverlayCamera.Instance.renderTextureCamera) {
      if (_originalMaterials != null) {
        // restore all materials
        _renderer.sharedMaterials = _originalMaterials;

        // clear out reference
        _originalMaterials = null;
      }
    }
  }
}
