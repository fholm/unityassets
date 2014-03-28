using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BattleTextRenderer))]
public class BattleTextAnimator : MonoBehaviour
{
    float startTime;
    int sizeAnimationIndex;
    float sizeAnimationStart;
    Vector3 animatedPosition;
    BattleTextRenderer textRenderer;
    BattleTextPositionAnimation[] positions;

    internal Transform Target;
    internal Vector3 WorldOffset;
    internal Vector3 RandomOffset;
    internal Vector3 ScreenPosition;
    internal BattleTextAnimation Animation;
    internal float Scale = 1f;

    void Start()
    {
        startTime = sizeAnimationStart = Time.time;
        textRenderer = GetComponent<BattleTextRenderer>();
    }

    void Update()
    {
        float t = Time.time - startTime;

        if (t > Animation.Time)
        {
            Destroy(gameObject);
        }
        else
        {
            Vector3 realPosition;

            if (Target != null)
            {
                if (Animation.AnimateInWorld)
                {
                    realPosition = Target.position + WorldOffset;
                }
                else
                {
                    realPosition = BattleTextCamera.Instance.WorldToTextPosition(Target.position + WorldOffset);
                }
            }
            else
            {
                realPosition = ScreenPosition;
            }

            // Position animation
            for (int i = 0; i < positions.Length; ++i)
            {
                BattleTextPositionAnimation anim = positions[i];

                if (anim.Start <= t)
                {
                    float pt = (t - anim.Start) / anim.Time;

                    if (pt <= 1f)
                    {
                        if (anim.Falloff > 0f)
                        {
                            animatedPosition += anim.Amount * Time.deltaTime * Mathf.Lerp(1f, 0f, pt * anim.Falloff);
                        }
                        else
                        {
                            animatedPosition += anim.Amount * Time.deltaTime;
                        }
                    }
                }
            }

            // Size animation
            while (sizeAnimationIndex < Animation.Size.Length)
            {
                BattleTextSizeAnimation anim = Animation.Size[sizeAnimationIndex];

                float sz = (Time.time - sizeAnimationStart) / anim.Time;

                if (sz > 1)
                {
                    sizeAnimationStart = Time.time;
                    sizeAnimationIndex += 1;
                    continue;
                }

                textRenderer.GlyphSize = Mathf.Lerp(anim.From, anim.To, sz);
                break;
            }

            // Apply scaling
            textRenderer.GlyphSize = textRenderer.GlyphSize * Scale;

            // Add all positions together
            realPosition += Animation.FixedOffset;
            realPosition += RandomOffset;
            realPosition += animatedPosition;

            if (!Animation.AnimateInWorld)
            {
                realPosition.z = 1;
            }

            // Set our position
            transform.position = realPosition;
        }
    }

    public static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Transform target,
        Vector3 targetWorldOffset)
    {
        return SpawnText(
            text,
            prefab,
            animation,
            target,
            targetWorldOffset,
            Vector2.zero,
            animation.DefaultColor
        );
    }

    public static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Transform target,
        Vector3 targetWorldOffset,
        Color color)
    {
        return SpawnText(
            text,
            prefab,
            animation,
            target,
            targetWorldOffset,
            Vector2.zero,
            color
        );
    }

    public static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Vector3 worldPosition,
        Color color)
    {
        return SpawnText(
            text,
            prefab,
            animation,
            null,
            Vector3.zero,
            BattleTextCamera.Instance.WorldToTextPosition(worldPosition),
            color
        );
    }

    public static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Vector3 worldPosition)
    {
        return SpawnText(
            text,
            prefab,
            animation,
            null,
            Vector3.zero,
            BattleTextCamera.Instance.WorldToTextPosition(worldPosition),
            animation.DefaultColor
        );
    }

    public static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Vector2 screenPosition,
        Color color)
    {
        return SpawnText(
            text,
            prefab,
            animation,
            null,
            Vector3.zero,
            screenPosition,
            color
        );
    }

    public static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Vector2 screenPosition)
    {
        return SpawnText(
            text,
            prefab,
            animation,
            null,
            Vector3.zero,
            screenPosition,
            animation.DefaultColor
        );
    }

    static GameObject SpawnText(
        string text,
        GameObject prefab,
        BattleTextAnimation animation,
        Transform target,
        Vector3 targetWorldOffset,
        Vector2 screenPosition,
        Color color)
    {
        GameObject instance = (GameObject)GameObject.Instantiate(prefab);
        BattleTextRenderer renderer = instance.GetComponent<BattleTextRenderer>();
        BattleTextAnimator animator = instance.AddComponent<BattleTextAnimator>();

        if (!renderer.IsAnimated)
        {
            Debug.LogError(string.Format("[BattleTextAnimator] BattleTextRenderer script on '{0}' does not have 'Is Animated' set", prefab.name));

            // Disable object
            instance.SetActiveRecursively(false);

            // Destroy it
            GameObject.Destroy(instance);

            return null;
        }

        // Renderer settings
        renderer.InitialText = text;
        renderer.Color = color;
        renderer.FadeDelay = animation.FadeDelay;
        renderer.LockXRotation = false;
        renderer.LookAtMainCamera = animation.AnimateInWorld;

        // Animator settings

        animator.positions = animation.ClonePosition();
        animator.Target = target;
        animator.Animation = animation;
        animator.WorldOffset = targetWorldOffset;
        animator.ScreenPosition = screenPosition;
        animator.RandomOffset = new Vector3(
            UnityEngine.Random.Range(-animation.RandomOffset.x, animation.RandomOffset.x),
            UnityEngine.Random.Range(-animation.RandomOffset.y, animation.RandomOffset.y),
            UnityEngine.Random.Range(-animation.RandomOffset.z, animation.RandomOffset.z)
        );

        if (animation.HasRandomPosition)
        {
            for (int i = 0; i < animator.positions.Length; ++i)
            {
                if (animator.positions[i].Random)
                {
                    Vector3 a = animator.positions[i].Amount;
                    animator.positions[i].Amount.x = Random.Range(-a.x, a.x);
                    animator.positions[i].Amount.y = Random.Range(-a.y, a.y);
                    animator.positions[i].Amount.z = Random.Range(-a.z, a.z);
                }
            }
        }

        return instance;
    }
}