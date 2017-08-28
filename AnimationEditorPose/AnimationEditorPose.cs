using System;
using UnityEngine;

[ExecuteInEditMode]
public class AnimationEditorPose : MonoBehaviour {
	public Animator Animator;
	public AnimationClip Clip;

	[Range(0, 1)]
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

      Time = Mathf.Clamp01(Time);
      Clip.SampleAnimation(Animator.gameObject, Clip.length * Time);
    }
#endif
	}
}
