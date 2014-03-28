using UnityEngine;

public class DemoPlatformMover : MonoBehaviour
{
    float travelTime;
    float startTime;
    float moveTime;

    public Vector3 Min = new Vector3(0, 0, 0);
    public Vector3 Max = new Vector3(0, 10, 0);
    public float MoveSpeed = 1f;

    void Start()
    {
        startTime = 0;
        moveTime = ((Max - Min).magnitude / MoveSpeed) * 2f;
        transform.position = Min;
    }

    void Update()
    {
        travelTime += Time.deltaTime;

        float dt = travelTime - startTime;
        float t = Mathf.Repeat(dt, moveTime);

        if (t > moveTime / 2)
        {
            t = moveTime - t;
        }

        transform.position = Vector3.Lerp(Min, Max, t / (moveTime / 2f));
    }
}