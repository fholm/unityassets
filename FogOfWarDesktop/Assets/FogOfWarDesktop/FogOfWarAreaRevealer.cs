using UnityEngine;
using System.Collections;
using System.Linq;

public class FogOfWarAreaRevealer : MonoBehaviour
{
    int frameCount;
    FogOfWar[] fow;

    [SerializeField]
    float radius = 20f;

    [SerializeField]
    int updateRate = 10;

    [SerializeField]
    LayerMask layers = 0;

    void Start()
    {
        fow = FindObjectsOfType(typeof(FogOfWar)).Cast<FogOfWar>().ToArray();
    }

    void LateUpdate()
    {
        if (++frameCount >= updateRate)
        {
            frameCount = 0;

            if (Physics.CheckSphere(transform.position, radius, layers))
            {
                for (int i = 0; i < fow.Length; ++i)
                {
                    fow[i].RevealCircle(transform.position, radius, float.MaxValue, float.MaxValue, radius);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
