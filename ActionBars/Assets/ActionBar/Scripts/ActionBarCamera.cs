using UnityEngine;

[ExecuteInEditMode]
public class ActionBarCamera : MonoBehaviour
{
    static ActionBarCamera instance = null;

    public static ActionBarCamera Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ActionBarCamera)) as ActionBarCamera;
            }

            return instance;
        }
    }

    [SerializeField]
    float viewArea = 500f;

    void Start()
    {
        if (!camera)
        {
            gameObject.AddComponent<Camera>();
        }
        
        instance = this;
        camera.orthographic = true;
        camera.nearClipPlane = 0;
        camera.farClipPlane = viewArea;
        camera.depth = 1;
        camera.clearFlags = CameraClearFlags.Depth;
        camera.cullingMask = 1 << gameObject.layer;
        transform.position = new Vector3(0, 0, -(viewArea/2f));
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