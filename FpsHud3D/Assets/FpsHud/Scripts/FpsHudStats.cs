using UnityEngine;
using System.Collections;

public class FpsHudStats : MonoBehaviour
{
    [SerializeField]
    Transform healthIcon;

    [SerializeField]
    Transform shieldIcon;

    void Start()
    {
        healthIcon = transform.Find("HealthIcon");
        shieldIcon = transform.Find("ShieldIcon");

        // Set colors
        healthIcon.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.IconColor);
        shieldIcon.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.IconColor);
    }
}