using UnityEngine;

[ExecuteInEditMode]
public class UiCamera : MonoBehaviour
{
    void Start()
    {
        if (!camera)
        {
            gameObject.AddComponent<Camera>();
        }
        
        camera.orthographic = true;
        camera.nearClipPlane = 0;
        camera.farClipPlane = 500f;
        camera.depth = 100;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.cullingMask = 1 << gameObject.layer;
        transform.position = new Vector3(0, 0, -(500f / 2f));
    }

    void Awake()
    {
        Start();
    }

    void OnEnable()
    {
        Start();
    }

    void Update()
    {
        if (camera)
        {
            camera.orthographicSize = Screen.height / 2;
        }
    }

    void OnRenderObject()
    {
        Update();
    }
}