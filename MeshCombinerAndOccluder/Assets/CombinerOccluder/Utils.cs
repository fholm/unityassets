using System;
using UnityEngine;

public static class Utils {
  public static T[] InitArray<T>(this Int32 amount, T value) {
    T[] result = new T[amount];

    for (Int32 i = 0; i < amount; ++i) {
      result[i] = value;
    }

    return result;
  }

  public static T[] InitArray<T>(this Int32 amount, Func<Int32, T> selector) {
    T[] result = new T[amount];

    for (Int32 i = 0; i < amount; ++i) {
      result[i] = selector(i);
    }

    return result;
  }

  public static B[] Map<A, B>(this A[] array, Func<A, B> map) {
    if (array == null) {
      return null;
    }

    B[] result = new B[array.Length];

    for (int i = 0; i < array.Length; ++i) {
      result[i] = map(array[i]);
    }

    return result;
  }

  public static void Show(this GameObject[] gameObjects) {
    if (gameObjects != null) {
      for (int i = 0; i < gameObjects.Length; ++i) {
        gameObjects[i].SetActive(true);
      }
    }
  }

  public static void Hide(this GameObject[] gameObjects) {
    if (gameObjects != null) {
      for (int i = 0; i < gameObjects.Length; ++i) {
        gameObjects[i].SetActive(false);
      }
    }
  }

  public static void Show(this GameObject gameObject) {
    if (gameObject && !gameObject.activeSelf) {
      gameObject.SetActive(true);
    }
  }

  public static void Hide(this GameObject gameObject) {
    if (gameObject && gameObject.activeSelf) {
      gameObject.SetActive(false);
    }
  }

  public static bool Toggle(this GameObject gameObject) {
    if (gameObject) {
      return gameObject.Toggle(!gameObject.activeSelf);
    }

    return false;
  }

  public static bool Toggle(this GameObject gameObject, bool state) {
    if (gameObject) {
      if (gameObject.activeSelf != state) {
        gameObject.SetActive(state);
      }

      return state;
    }

    return false;
  }

  public static bool Toggle(this Component component, bool state) {
    if (component) {
      return component.gameObject.Toggle(state);
    }

    return false;
  }
}
