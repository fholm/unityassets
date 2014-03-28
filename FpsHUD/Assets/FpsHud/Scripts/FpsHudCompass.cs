using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class FpsHudCompass : MonoBehaviour
{
    [SerializeField]
    Vector3 north = Vector3.forward;

    [SerializeField]
    Transform target = null;

    [SerializeField]
    float northOffset = 0.55f;

    void Start()
    {
        if (!target)
        {
            target = transform;
        }
    }

    void Update()
    {
        if (renderer)
        {
            Vector3 f = target.forward;
            f.y = 0;

            float a = Mathf.Repeat(360f + SignedAngle(north, f.normalized, Vector3.up), 360f) / 360f;
            renderer.material.mainTextureOffset = new Vector2(northOffset + a, 0);
        }
    }

    static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return (float)Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
    }
}