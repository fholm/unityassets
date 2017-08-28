using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils  {
  public static bool TryPickRandom<T>(this T[] array, out T value) {
    if (array != null && array.Length > 0) {
      value = array.PickRandom();
      return true;
    }

    value = default(T);
    return false;
  }

  public static T PickRandom<T>(this T[] array) {
    return array[UnityEngine.Random.Range(0, array.Length)];
  }

  public static Rect SetWidth(this Rect r, float w) {
    r.width = w;
    return r;
  }

  public static Rect SetHeight(this Rect r, float h) {
    r.height = h;
    return r;
  }

  public static Rect AddX(this Rect r, float x) {
    r.x += x;
    return r;
  }

  public static Rect AddY(this Rect r, float y) {
    r.y += y;
    return r;
  }

}
