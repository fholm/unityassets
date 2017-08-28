using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SingleRange))]
public class SingleRangePropertyDrawer : PropertyDrawer {
  public override System.Single GetPropertyHeight(SerializedProperty property, GUIContent label) {
    return 40;
  }

  public override void OnGUI(Rect p, SerializedProperty property, GUIContent label) {
    var min = property.FindPropertyRelative("Min");
    var max = property.FindPropertyRelative("Max");

    var minVal = min.floatValue;
    var maxVal = max.floatValue;

    EditorGUI.BeginChangeCheck();
    EditorGUI.MinMaxSlider(p = p.SetHeight(18), label, ref minVal, ref maxVal, 0, 10);

    p = p.AddY(20);
    p = EditorGUI.PrefixLabel(p, new GUIContent(" "));

    minVal = EditorGUI.FloatField(p.AddX(-15).SetWidth(100), minVal);
    maxVal = EditorGUI.FloatField(p.AddX(p.width - 100).SetWidth(100), maxVal);

    if (EditorGUI.EndChangeCheck()) {
      min.floatValue = Mathf.Min(minVal, maxVal);
      max.floatValue = Mathf.Max(minVal, maxVal);
    }
  }
}
