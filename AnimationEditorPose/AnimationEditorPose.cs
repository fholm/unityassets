using System;
using UnityEngine;

[ExecuteInEditMode]
public class AnimationEditorPose : MonoBehaviour {
  public Animator Animator;
  public AnimationClip Clip;
  public Single Time;

  void Update() {
    if (Application.isPlaying) {
      Destroy(this);
      return;
    }

#if UNITY_EDITOR
    if (Clip) {
      // try to grab animator
      if (!Animator) {
        Animator = gameObject.GetComponentInParent<Animator>();

        if (!Animator) {
          Animator = gameObject.GetComponentInChildren<Animator>();

          if (!Animator) {
            return;
          }
        }
      }


      Time = Mathf.Clamp(Time, 0, Clip.length);
      Clip.SampleAnimation(Animator.gameObject, Time);
    }
#endif
  }
}
