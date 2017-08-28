using System;
using System.Collections.Generic;
using UnityEngine;

public class SFXOneShotPool : SingletonObject<SFXOneShotPool> {
  [SerializeField]
  SFXOneShotPoolSource _sourceTemplate;

  [NonSerialized]
  Stack<SFXOneShotPoolSource> _sourcePool;

  [NonSerialized]
  List<SFXOneShotPoolSource> _sourceActiveList;

  public static void Play(SFXOneShot oneshot, Vector3 position) {
    if (Instance) {
      Instance.PlayInstance(oneshot, position);
    }
  }

  void Update() {
    var dt = Time.deltaTime;

    if (_sourceActiveList != null) {
      for (Int32 i = 0; i < _sourceActiveList.Count; ++i) {
        SFXOneShotPoolSource source;

        source = _sourceActiveList[i];
        source.Delay -= dt;

        if (source.Delay < 0 && source.AudioSource.isPlaying == false) {
          // clear clip
          source.AudioSource.clip = null;

          // push back on pool
          _sourcePool.Push(source);

          // remove from active list
          _sourceActiveList.RemoveAt(i);

          // step index back once
          --i;
        }
      }
    }
  }

  void PlayInstance(SFXOneShot oneshot, Vector3 position) {
    if (_sourceActiveList == null) {
      _sourceActiveList = new List<SFXOneShotPoolSource>();
    }

    AudioClip clip;

    if (oneshot.Clips.TryPickRandom(out clip)) {
      var src = GetSource();

      src.AudioSource.clip = clip;
      src.AudioSource.pitch = oneshot.Pitch.GetRandom();
      src.AudioSource.volume = oneshot.Volume;
      src.AudioSource.spatialBlend = oneshot.SpatialBlend;
      src.AudioSource.outputAudioMixerGroup = oneshot.Output;

      if (oneshot.PlayDelay > 0) {
        src.Delay = oneshot.PlayDelay * 1.1f;
        src.AudioSource.PlayDelayed(oneshot.PlayDelay);
      }
      else {
        src.Delay = 0;
        src.AudioSource.Play();
      }

      // set position
      src.transform.position = position;

      // add to active list
      _sourceActiveList.Add(src);
    }
  }

  SFXOneShotPoolSource GetSource() {
    if (_sourcePool == null) {
      _sourcePool = new Stack<SFXOneShotPoolSource>();
    }

    SFXOneShotPoolSource source;

    if (_sourcePool.Count > 0) {
      source = _sourcePool.Pop();
    }
    else {
      source = Instantiate(_sourceTemplate);
      source.transform.parent = transform;
    }

    return source;
  }
}
