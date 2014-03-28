using UnityEngine;
using System.Collections;

public class BattleTextSource : MonoBehaviour
{
    public enum PositionSource
    {
        Transform, 
        Screen
    }

    GameObject instance;

    public Transform Target;
    public string SourceName = "";
    public BattleTextAnimation DefaultAnimation;
    public PositionSource Source;
    public bool FollowTargetPosition;
    public Vector2 ScreenPosition;
    public Vector3 TargetWorldOffset;
    public bool SingleInstance = false;
    public GameObject DefaultText;

    void Start()
    {
        if (Source == PositionSource.Transform && Target == null)
        {
            Target = transform;
        }
    }

    public void DisplayText(string text)
    {
        DisplayText(text, DefaultText, DefaultAnimation.DefaultColor);
    }

    public void DisplayText(string text, BattleTextAnimation animation)
    {
        DisplayText(text, DefaultText, animation, animation.DefaultColor);
    }

    public void DisplayText(string text, Color color)
    {
        DisplayText(text, DefaultText, color);
    }

    public void DisplayText(string text, Color color, BattleTextAnimation animation)
    {
        DisplayText(text, DefaultText, animation, color);
    }

    public void DisplayText(string text, GameObject prefab)
    {
        DisplayText(text, prefab, DefaultAnimation.DefaultColor);
    }

    public void DisplayText(string text, GameObject prefab, BattleTextAnimation animation)
    {
        DisplayText(text, prefab, animation, animation.DefaultColor);
    }

    public void DisplayText(string text, GameObject prefab, Color color)
    {
        DisplayText(text, prefab, DefaultAnimation, color);
    }

    public void DisplayText(string text, GameObject prefab, BattleTextAnimation animation, Color color)
    {
        if (!string.IsNullOrEmpty(text))
        {
            if (SingleInstance && instance != null)
            {
                GameObject.Destroy(instance);
            }

            switch (Source)
            {
                case PositionSource.Screen:
                    instance = BattleTextAnimator.SpawnText(text, prefab, animation, ScreenPosition, color);
                    break;

                case PositionSource.Transform:
                    if (FollowTargetPosition)
                    {
                        instance = BattleTextAnimator.SpawnText(text, prefab, animation, Target, TargetWorldOffset, color);
                    }
                    else
                    {
                        instance = BattleTextAnimator.SpawnText(text, prefab, animation, Target.position + TargetWorldOffset, color);
                    }
                    break;
            }

            // Set source specific properties
            instance.layer = gameObject.layer;

            if (!SingleInstance)
            {
                instance = null;
            }
        }
    }
}

public static class BattleTextSourceExtensions
{
    public static BattleTextSource GetBattleTextSource(this GameObject go, string sourceName)
    {
        foreach (BattleTextSource source in go.GetComponents<BattleTextSource>())
        {
            if (source.SourceName == sourceName)
            {
                return source;
            }
        }

        return null;
    }

    public static BattleTextSource GetBattleTextSource(this MonoBehaviour mb, string sourceName)
    {
        return GetBattleTextSource(mb.gameObject, sourceName);
    }
}