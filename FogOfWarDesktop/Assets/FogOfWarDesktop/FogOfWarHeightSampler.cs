using UnityEngine;
using System.Collections;

public abstract class FogOfWarHeightSampler : MonoBehaviour
{
    public abstract float SampleHeight(Vector3 worldPosition);
}