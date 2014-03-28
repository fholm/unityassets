using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BattleTextCamera : MonoBehaviour
{
    public static BattleTextCamera Instance { get; private set; }

    Camera textCamera = null;

    public Camera MainCamera;

    bool verify()
    {
        if (MainCamera == null)
        {
            Debug.LogWarning("[BattleTextCamera] The field 'mainCamera' is not set");
            return false;
        }

        return true;
    }

    void Start()
    {
        Instance = this;
        textCamera = GetComponent<Camera>();

        if (!verify())
        {
            Debug.LogWarning("[BattleTextCamera] Falling back to UnityEngine.Camera.mainCamera");
            MainCamera = Camera.mainCamera;
        }

        // Set text camera settings
        textCamera.isOrthoGraphic = true;
        textCamera.orthographicSize = 10f;
        textCamera.nearClipPlane = 1;
        textCamera.farClipPlane = 3;
        textCamera.clearFlags = CameraClearFlags.Nothing;
        textCamera.depth = 0;
        textCamera.renderingPath = RenderingPath.Forward;

        // Position ourselves at the origin, with no rotation
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    void Awake()
    {
        Start();
    }

    void Update()
    {
        Instance = this;

        // Never allow any movement, ever.
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    public Vector3 WorldToTextPosition(Vector3 worldPosition)
    {
        Vector3 screen = MainCamera.WorldToScreenPoint(worldPosition);
        Vector3 text = textCamera.ScreenToWorldPoint(screen);

        text.z = 1;

        return text;
    }
}