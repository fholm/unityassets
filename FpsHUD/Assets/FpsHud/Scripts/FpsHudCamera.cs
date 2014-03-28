using UnityEngine;

[ExecuteInEditMode]
public class FpsHudCamera : MonoBehaviour
{
    static FpsHudCamera instance = null;

    public static FpsHudCamera Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FpsHudCamera)) as FpsHudCamera;
            }

            return instance;
        }
    }

    void Start()
    {
        if (!camera)
        {
            gameObject.AddComponent<Camera>();
        }


        instance = this;
        camera.orthographic = true;
        camera.nearClipPlane = 0;
        camera.farClipPlane = 500f;
        camera.depth = 100;
        camera.clearFlags = CameraClearFlags.Depth;
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