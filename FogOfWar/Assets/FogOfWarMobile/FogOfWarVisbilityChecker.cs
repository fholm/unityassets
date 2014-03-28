using UnityEngine;
using System.Collections;

public class FogOfWarVisbilityChecker : MonoBehaviour
{
    FogOfWar fow;

    void Start()
    {
        fow = FindObjectOfType(typeof(FogOfWar)) as FogOfWar;
    }

    void LateUpdate()
    {
        if (fow != null && fow.UpdateFog)
        {
            renderer.enabled = fow.IsVisible(transform.position);
        }
    }
}