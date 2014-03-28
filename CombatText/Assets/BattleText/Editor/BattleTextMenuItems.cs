using UnityEngine;
using System.Collections;
using UnityEditor;

public static class BattleTextMenuItems
{
    [MenuItem("Assets/Create/Battle Text Animation")]
    public static void CreateMyAsset()
    {
        BattleTextAnimation asset = ScriptableObject.CreateInstance<BattleTextAnimation>();

        AssetDatabase.CreateAsset(asset, "Assets/BattleText/Fonts/Animations/NewAnimation.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}