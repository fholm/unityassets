using UnityEngine;
using System.Collections;

public class ClickToMove : MonoBehaviour
{
    Vector3 targetPoint;
    Vector3 direction;

    [SerializeField]
    Terrain terrain;

    [SerializeField]
    Animation anim;

    void Start()
    {
        targetPoint = transform.position;
    }

    void Update()
    {
        Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = default(RaycastHit);

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit) && hit.transform.GetComponent<Terrain>() != null)
            {
                targetPoint = hit.point;
            }
        }

        Vector3 v = targetPoint - transform.position;

        transform.position += Vector3.ClampMagnitude(v, Mathf.Min(v.magnitude, Time.deltaTime * 2f));

        if ((transform.position - targetPoint).magnitude < 0.1)
        {
            transform.position = targetPoint;
            anim.CrossFade("Idle");
        }
        else
        {
            anim.CrossFade("Walk");
            transform.rotation = Quaternion.LookRotation(v, Vector3.up);
        }

        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position), transform.position.z);
    }
}