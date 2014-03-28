using UnityEngine;
using System.Collections;

public class FogOfWarRenderer : MonoBehaviour
{
    [SerializeField]
    FogOfWar fogOfWar;

    void Update()
    {
        if (fogOfWar != null && fogOfWar.enabled && fogOfWar.gameObject.active)
        {
            renderer.enabled = fogOfWar.IsRevealed(transform.position);
        }
    }
}