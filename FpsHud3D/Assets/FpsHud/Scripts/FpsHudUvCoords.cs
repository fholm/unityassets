using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FpsHudUvCoords : MonoBehaviour
{
    [SerializeField]
    Vector2[] uv = new Vector2[4];

    void Start()
    {
        GetComponent<MeshFilter>().mesh.uv = uv;
    }
}