using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RecalcNormals : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
        GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
    }
}
