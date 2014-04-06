using UnityEngine;
using System.Collections;

public class FpsHudReticule : MonoBehaviour
{
    public int MinSpred = 5;
    public int MaxSpred = 75;
    public float Spread = 0f;

    void Start()
    {
        if (renderer)
        {
            renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.CrosshairColor);
        }
    }
}