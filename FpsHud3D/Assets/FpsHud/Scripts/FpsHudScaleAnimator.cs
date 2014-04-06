using UnityEngine;
using System.Collections;

public class FpsHudScaleAnimator : MonoBehaviour
{
    float start;

    [SerializeField]
    float duration = 0.3f;

    [SerializeField]
    float frequency = 1f;

    [SerializeField]
    float scaleFrom = 1f;

    [SerializeField]
    float scaleTo = 2f;

    void Update()
    {
        float s = 1;
        float t = Time.time - start;
        float d2 = duration / 2f;

        if (t >= frequency)
        {
            start = Time.time;
            t = 0f;
        }

        if (t / 2 >= d2)
        {
            s = Mathf.Lerp(scaleTo, scaleFrom, ((t - d2) / d2));
        }
        else
        {
            s = Mathf.Lerp(scaleFrom, scaleTo, t / d2);
        }

        transform.localScale = new Vector3(s, s, 1);
    }
}