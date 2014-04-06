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
    float textureOffset = 0f;

    void Update()
    {
        if (!target)
        {
            target = FpsHud.Instance.PlayerCamera.transform;
        }

        Vector3 f = target.forward;
        f.y = 0;

        float a = Mathf.Repeat(360f + FpsHudUtils.SignedAngle(north, f.normalized, Vector3.up), 360f) / 360f;
        renderer.material.mainTextureOffset = new Vector2(textureOffset + a, 0);
    }
}