using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXOneShotPoolSource : MonoBehaviour {
  public Single Delay;
  public AudioSource AudioSource;

  void Reset() {
    AudioSource = GetComponent<AudioSource>();
  }
}
