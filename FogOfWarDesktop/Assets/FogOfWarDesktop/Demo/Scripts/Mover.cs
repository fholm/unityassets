using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
    Vector3 from;
    Vector3 to;
    float t;

    void Start()
    {
        t = 0;
        from = transform.position;
        to = new Vector3(Random.Range(-512f, 512f), 1f, Random.Range(-512f, 512f));
    }

    void Update()
    {
        transform.position = Vector3.Lerp(from, to, (t += Time.deltaTime) / 60f);
    }
}
