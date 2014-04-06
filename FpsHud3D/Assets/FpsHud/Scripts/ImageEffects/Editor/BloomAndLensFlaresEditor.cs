// Type: BloomAndLensFlaresEditor
// Assembly: Assembly-UnityScript-Editor-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\Fredrik\Projects\UnityAssets\FpsHud3D\Library\ScriptAssemblies\Assembly-UnityScript-Editor-firstpass.dll

using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (BloomAndLensFlares))]
[Serializable]
public class BloomAndLensFlaresEditor : Editor
{
  public SerializedProperty tweakMode;
  public SerializedProperty screenBlendMode;
  public SerializedObject serObj;
  public SerializedProperty hdr;
  public SerializedProperty sepBlurSpread;
  public SerializedProperty useSrcAlphaAsMask;
  public SerializedProperty bloomIntensity;
  public SerializedProperty bloomThreshhold;
  public SerializedProperty bloomBlurIterations;
  public SerializedProperty lensflares;
  public SerializedProperty hollywoodFlareBlurIterations;
  public SerializedProperty lensflareMode;
  public SerializedProperty hollyStretchWidth;
  public SerializedProperty lensflareIntensity;
  public SerializedProperty lensflareThreshhold;
  public SerializedProperty flareColorA;
  public SerializedProperty flareColorB;
  public SerializedProperty flareColorC;
  public SerializedProperty flareColorD;
  public SerializedProperty blurWidth;
  public SerializedProperty lensFlareVignetteMask;

  public BloomAndLensFlaresEditor()
      : base()
  {

  }

  public virtual void OnEnable()
  {
    this.serObj = new SerializedObject(this.target);
    this.screenBlendMode = this.serObj.FindProperty("screenBlendMode");
    this.hdr = this.serObj.FindProperty("hdr");
    this.sepBlurSpread = this.serObj.FindProperty("sepBlurSpread");
    this.useSrcAlphaAsMask = this.serObj.FindProperty("useSrcAlphaAsMask");
    this.bloomIntensity = this.serObj.FindProperty("bloomIntensity");
    this.bloomThreshhold = this.serObj.FindProperty("bloomThreshhold");
    this.bloomBlurIterations = this.serObj.FindProperty("bloomBlurIterations");
    this.lensflares = this.serObj.FindProperty("lensflares");
    this.lensflareMode = this.serObj.FindProperty("lensflareMode");
    this.hollywoodFlareBlurIterations = this.serObj.FindProperty("hollywoodFlareBlurIterations");
    this.hollyStretchWidth = this.serObj.FindProperty("hollyStretchWidth");
    this.lensflareIntensity = this.serObj.FindProperty("lensflareIntensity");
    this.lensflareThreshhold = this.serObj.FindProperty("lensflareThreshhold");
    this.flareColorA = this.serObj.FindProperty("flareColorA");
    this.flareColorB = this.serObj.FindProperty("flareColorB");
    this.flareColorC = this.serObj.FindProperty("flareColorC");
    this.flareColorD = this.serObj.FindProperty("flareColorD");
    this.blurWidth = this.serObj.FindProperty("blurWidth");
    this.lensFlareVignetteMask = this.serObj.FindProperty("lensFlareVignetteMask");
    this.tweakMode = this.serObj.FindProperty("tweakMode");
  }

  public override void OnInspectorGUI()
  {
    this.serObj.Update();

    //GUILayout.Label(RuntimeServices.op_Addition(RuntimeServices.op_Addition("HDR ", this.hdr.get_enumValueIndex() != 0 ? (this.hdr.get_enumValueIndex() != 1 ? "disabled, " : "forced on, ") : "auto detected, "), (double) this.useSrcAlphaAsMask.floatValue >= 0.100000001490116 ? " using alpha channel glow information" : " ignoring alpha channel glow information"), EditorStyles.get_miniBoldLabel(), new GUILayoutOption[0]);

    EditorGUILayout.PropertyField(this.tweakMode, new GUIContent("Tweak mode"), new GUILayoutOption[0]);
    EditorGUILayout.PropertyField(this.screenBlendMode, new GUIContent("Blend mode"), new GUILayoutOption[0]);
    EditorGUILayout.PropertyField(this.hdr, new GUIContent("HDR"), new GUILayoutOption[0]);
    Camera camera = (this.target as BloomAndLensFlares).camera;
    if ((UnityEngine.Object)camera != (UnityEngine.Object)null && this.screenBlendMode.enumValueIndex == 0 && (camera.hdr && this.hdr.enumValueIndex == 0 || this.hdr.enumValueIndex == 1))
      EditorGUILayout.HelpBox("Screen blend is not supported in HDR. Using 'Add' instead.", (MessageType) 1);
    if (1 == this.tweakMode.intValue)
      EditorGUILayout.PropertyField(this.lensflares, new GUIContent("Cast lens flares"), new GUILayoutOption[0]);
    EditorGUILayout.Separator();
    EditorGUILayout.PropertyField(this.bloomIntensity, new GUIContent("Intensity"), new GUILayoutOption[0]);
    this.bloomThreshhold.floatValue = (EditorGUILayout.Slider("Threshhold", this.bloomThreshhold.floatValue, -0.05f, 4f, new GUILayoutOption[0]));
    this.bloomBlurIterations.intValue = (EditorGUILayout.IntSlider("Blur iterations", this.bloomBlurIterations.intValue, 1, 4, new GUILayoutOption[0]));
    this.sepBlurSpread.floatValue = (EditorGUILayout.Slider("Blur spread", this.sepBlurSpread.floatValue, 0.1f, 10f, new GUILayoutOption[0]));
    if (1 == this.tweakMode.intValue)
      this.useSrcAlphaAsMask.floatValue = (EditorGUILayout.Slider(new GUIContent("Use alpha mask", "Make alpha channel define glowiness"), this.useSrcAlphaAsMask.floatValue, 0.0f, 1f, new GUILayoutOption[0]));
    else
      this.useSrcAlphaAsMask.floatValue = (0.0f);
    if (1 == this.tweakMode.intValue)
    {
      EditorGUILayout.Separator();
      if (this.lensflares.boolValue)
      {
        if (this.tweakMode.intValue != 0)
          EditorGUILayout.PropertyField(this.lensflareMode, new GUIContent("Lens flare mode"), new GUILayoutOption[0]);
        else
          this.lensflareMode.enumValueIndex = (0);
        EditorGUILayout.PropertyField(this.lensFlareVignetteMask, new GUIContent("Lens flare mask", "This mask is needed to prevent lens flare artifacts"), new GUILayoutOption[0]);
        EditorGUILayout.PropertyField(this.lensflareIntensity, new GUIContent("Local intensity"), new GUILayoutOption[0]);
        this.lensflareThreshhold.floatValue = (EditorGUILayout.Slider("Local threshhold", this.lensflareThreshhold.floatValue, 0.0f, 1f, new GUILayoutOption[0]));
        if (this.lensflareMode.intValue == 0)
        {
          EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorA, new GUIContent("1st Color"), new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorB, new GUIContent("2nd Color"), new GUILayoutOption[0]);
          EditorGUILayout.EndHorizontal();
          EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorC, new GUIContent("3rd Color"), new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorD, new GUIContent("4th Color"), new GUILayoutOption[0]);
          EditorGUILayout.EndHorizontal();
        }
        else if (this.lensflareMode.intValue == 1)
        {
          EditorGUILayout.PropertyField(this.hollyStretchWidth, new GUIContent("Stretch width"), new GUILayoutOption[0]);
          this.hollywoodFlareBlurIterations.intValue = (EditorGUILayout.IntSlider("Blur iterations", this.hollywoodFlareBlurIterations.intValue, 1, 4, new GUILayoutOption[0]));
          EditorGUILayout.PropertyField(this.flareColorA, new GUIContent("Tint Color"), new GUILayoutOption[0]);
        }
        else if (this.lensflareMode.intValue == 2)
        {
          EditorGUILayout.PropertyField(this.hollyStretchWidth, new GUIContent("Stretch width"), new GUILayoutOption[0]);
          this.hollywoodFlareBlurIterations.intValue = (EditorGUILayout.IntSlider("Blur iterations", this.hollywoodFlareBlurIterations.intValue, 1, 4, new GUILayoutOption[0]));
          EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorA, new GUIContent("1st Color"), new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorB, new GUIContent("2nd Color"), new GUILayoutOption[0]);
          EditorGUILayout.EndHorizontal();
          EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorC, new GUIContent("3rd Color"), new GUILayoutOption[0]);
          EditorGUILayout.PropertyField(this.flareColorD, new GUIContent("4th Color"), new GUILayoutOption[0]);
          EditorGUILayout.EndHorizontal();
        }
      }
    }
    else
      this.lensflares.boolValue = (false);
    this.serObj.ApplyModifiedProperties();
  }
}
