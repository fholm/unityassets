using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour {
  public static List<HealthBar> Active = new List<HealthBar>();

  public float HealthPercentage {
    get { return _healthPercentage; }
  }

  public float HealthBarWidth {
    get { return 60; }
  }

  public float HealthBarAlpha {
    get { return _opacity; }
  }

  public float HealthBarYOffset {
    get { return 1.5f; }
  }

  public Color HealthBarColor {
    get { return _color; }
  }

  [Range(0f, 1f)]
  [SerializeField]
  float _opacity = 1f;

  [SerializeField]
  Color _color = Color.green;

  float _healthPercentage;

  void Awake() {
    _healthPercentage = Random.value;
  }

  void OnEnable() {
    Active.Add(this);
  }

  void OnDisable() {
    Active.Remove(this);
  }
}
