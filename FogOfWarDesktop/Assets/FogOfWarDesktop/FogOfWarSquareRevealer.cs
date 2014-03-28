using System.Linq;
using UnityEngine;

public class FogOfWarSquareRevealer : MonoBehaviour
{
    int frameCount;
    FogOfWar[] fow;

    [SerializeField]
    float size = 10f;

    [SerializeField]
    float fadeDelay = 5f;

    [SerializeField]
    int updateRate = 20;

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
                fow[i].RevealSquare(transform.position, size, fadeDelay);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, Vector3.one * size * 2f);
    }
}