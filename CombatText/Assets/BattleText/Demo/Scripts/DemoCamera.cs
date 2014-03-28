using UnityEngine;

public class DemoCamera : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed = 0f;

    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed);
    }
}
