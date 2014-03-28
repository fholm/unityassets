using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class RTSCamera : MonoBehaviour
{
    [SerializeField]
    bool scrollEnabled = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            scrollEnabled = !scrollEnabled;
        }

        if (!scrollEnabled)
        {
            return;
        }

        float s = Time.deltaTime * 8f;

        Vector3 mpos = Input.mousePosition;
        mpos.y = Screen.height - mpos.y;

        if (mpos.y < 15)
        {
            transform.position += Vector3.forward * s;
        }
        else if (mpos.y > Screen.height - 15)
        {
            transform.position += Vector3.back * s; 
        }

        if (mpos.x < 15)
        {
            transform.position += Vector3.left * s;
        }
        else if (mpos.x > Screen.width - 15)
        {
            transform.position += Vector3.right * s; 
        }
    }
}