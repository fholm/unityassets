using UnityEngine;
using System.Collections;

public class FogOfWarRayCastSampler : FogOfWarHeightSampler {

    [SerializeField]
    float height = 2048f;

    [SerializeField]
    float distance = 4096f;

    [SerializeField]
    Vector3 direction = Vector3.down;

    [SerializeField]
    LayerMask layers = -1;

    public override float SampleHeight(Vector3 worldPosition)
    {
        worldPosition.y = height;

        RaycastHit hit;

        if (Physics.Raycast(worldPosition, direction, out hit, distance, layers))
        {
            return hit.point.y;
        }

        return 0f;
    }
}
