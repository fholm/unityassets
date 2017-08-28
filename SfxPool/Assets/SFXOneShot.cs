using System;
using UnityEngine;

public enum SFXOneShotModes {
  Self,
  Forward
}

public enum SFXChannels {
  Interface,
  Music,
  Ambience,
  Effects
}

[Serializable]
public class SFXOneShot {
  [SerializeField]
  public UnityEngine.Audio.AudioMixerGroup Output;

  [Range(0f, 1f)]
  public Single Chance = 1f;

  [Range(0f, 10f)]
  public Single Interval = 0f;

  [Range(0f, 1f)]
  public Single Volume = 1f;

  [Range(0f, 1f)]
  public Single SpatialBlend = 1f;

  [Range(0f, 10f)]
  public Single PlayDelay = 0;

  [SerializeField]
  public SingleRange Pitch = new SingleRange(1, 1);

  [NonSerialized]
  public Single NextPlayTime;

  [SerializeField]
  public AudioClip[] Clips;
}

public static class SFXOneShotExtensions {
  public static void Play(this SFXOneShot container) {
    Play(container, Vector3.zero);
  }
  public static void Play(this SFXOneShot container, Component component) {
    if (component) {
      Play(container, component.transform.position);
    }
  }

  public static void Play(this SFXOneShot container, Transform transform) {
    if (transform) {
      Play(container, transform.position);
    }
  }

  public static void Play(this SFXOneShot container, GameObject gameObject) {
    if (gameObject) {
      Play(container, gameObject.transform.position);
    }
  }

  public static void Play(this SFXOneShot container, Vector3 position) {
    if (container == null) {
      return;
    }

    if (container.Clips == null) {
      return;
    }

    if ((container.Chance < 0.99f) && (UnityEngine.Random.value > container.Chance)) {
      return;
    }

    if (container.Interval > 0) {
      var t = Time.time;

      if (container.NextPlayTime > t) {
        return;
      }
      else {
        container.NextPlayTime = t + container.Interval;
      }
    }

    SFXOneShotPool.Play(container, position);
  }
}