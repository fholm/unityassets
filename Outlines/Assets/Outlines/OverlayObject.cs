using System;
using System.Collections.Generic;
using UnityEngine;

public class OverlayObject : MonoBehaviour {
  Int32 _lastLayer = -1;

  // cache for material arrays
  Dictionary<Int32, Material[]> _materialArrays;

  // meshes for this object
  [NonSerialized]
  public List<OverlayMesh> Meshes = new List<OverlayMesh>();

  // material
  public Material Material;

  // 
  public Material[] GetMaterialArray(Int32 size) {
    if (_materialArrays == null) {
      _materialArrays = new Dictionary<Int32, Material[]>();
    }

    Material[] array;

    if (_materialArrays.TryGetValue(size, out array) == false) {
      array = new Material[size];

      for (Int32 i = 0; i < array.Length; ++i) {
        array[i] = Material;
      }

      _materialArrays.Add(size, array);
    }

    return array;
  }

  public void SetLayer(String layer) {
    SetLayer(LayerMask.NameToLayer(layer));
  }

  public void SetLayer(Int32 layer) {
    if (_lastLayer != layer) {
      // cache so we dont update more than we need to
      _lastLayer = layer;

      for (Int32 i = 0; i < Meshes.Count; ++i) {
        if (Meshes[i]) {
          Meshes[i].gameObject.layer = layer;
        }
      }
    }
  }
}
