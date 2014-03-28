using UnityEngine;

public class DemoVikingMover : MonoBehaviour
{
    Vector3 from = new Vector3(-2.5f, 0, 0);
    Vector3 to = new Vector3(2.5f, 0, 0);
    Vector3 previous;
    Quaternion rotation;

    void Start()
    {
        previous = transform.position;
    }

    void Update()
    {
        previous = transform.position;
        transform.position = Vector3.Lerp(from, to, Mathf.PingPong(Time.time, 6) / 6f);
        rotation = Quaternion.LookRotation((transform.position - previous).normalized);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10f);
    }
}