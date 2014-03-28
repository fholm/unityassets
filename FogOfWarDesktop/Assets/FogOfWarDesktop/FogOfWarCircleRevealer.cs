using UnityEngine;
using System.Linq;

public class FogOfWarCircleRevealer : MonoBehaviour
{
    int frameCount;
    FogOfWar[] fow;

    [SerializeField]
    float radius = 15f;

    [SerializeField]
    float autoRevealRadius = 5f;

    [SerializeField]
    float sightHeight = 1f;

    [SerializeField]
    float fadeDelay = 1f;

    [SerializeField]
    int updateRate = 10;

    void Start()
    {
        fow = FindObjectsOfType(typeof(FogOfWar)).Cast<FogOfWar>().ToArray();
        frameCount = Random.Range(0, updateRate);
    }

    void Update()
    {
        if (++frameCount >= updateRate)
        {
            frameCount = 0;

            for (int i = 0; i < fow.Length; ++i)
            {
                fow[i].RevealCircle(transform.position, radius, sightHeight, fadeDelay, autoRevealRadius);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoRevealRadius);
    }
}