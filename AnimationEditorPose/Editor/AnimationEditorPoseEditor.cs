using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimationEditorPose))]
public class AnimationEditorPoseEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    // grab poser component
    var poser = target as AnimationEditorPose;
    if (!poser) {
      return;
    }

    // try to grab animator
    var animator = poser.gameObject.GetComponentInParent<Animator>();

    if (!animator) {
      animator = poser.gameObject.GetComponentInChildren<Animator>();

      if (!animator) {
        return;
      }
    }

    // grab animator controller
    var ac = (AnimatorController)animator.runtimeAnimatorController;


    foreach (var layer in ac.layers) {
      foreach (var state in layer.stateMachine.states) {
        var clip = state.state.motion as AnimationClip;
        if (clip) {
          if (GUILayout.Button(state.state.name)) {
            poser.Clip = clip;
          }
        }
      }
    }
  }
}
