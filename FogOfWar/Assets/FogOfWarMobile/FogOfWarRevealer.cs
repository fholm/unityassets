using UnityEngine;

public class FogOfWarRevealer : MonoBehaviour
{
    FogOfWar fow;

    [SerializeField]
    float radius = 5f;

    void Start()
    {
        fow = FindObjectOfType(typeof(FogOfWar)) as FogOfWar;
    }

    void Update()
    {
        if (fow != null && fow.UpdateFog)
        {
            fow.Reveal(transform.position, radius);
        }
    }
}
