using UnityEngine;

public class FogOfWarTerrainSampler : FogOfWarHeightSampler
{
    [SerializeField]
    Terrain terrain = null;

    public override float SampleHeight(Vector3 worldPosition)
    {
        if (terrain != null)
        {
            return terrain.SampleHeight(worldPosition);
        }

        return 0f;
    }
}