using System;
using UnityEngine;

[Serializable]
public struct SingleRange {
  public Single Min;
  public Single Max;

  public SingleRange(Single min, Single max) {
    Min = min;
    Max = max;
  }

  public Single GetRandom() {
    return GetNormalized(UnityEngine.Random.value);
  }

  public Single GetNormalized(Single value) {
    return Mathf.Lerp(Min, Max, Mathf.Clamp01(value));
  }
}
