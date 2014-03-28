using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FpsHudScopeCamera : MonoBehaviour
{
    static FpsHudScopeCamera instance = null;

    public static FpsHudScopeCamera Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FpsHudScopeCamera)) as FpsHudScopeCamera;
            }

            return instance;
        }
    }

    bool display = false;
    int playerCameraCullingMask;
    Color playerCameraBackgroundColor;
    CameraClearFlags playerCameraClearFlags;

    [SerializeField]
    GameObject scopeOverlay;

    [SerializeField]
    float scopeScale = 1f;

    [SerializeField]
    int maxScopePixelSize = 1024;

    void Start()
    {
        instance = this;

        if (!scopeOverlay)
        {
            Debug.LogError("Scope overlay not set");
            enabled = false;
            return;
        }

        camera.enabled = false;
        camera.depth = 50;
        camera.orthographic = false;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 1000f;
        scopeOverlay.renderer.enabled = false;
    }

    void LateUpdate()
    {
        FpsHud hud = FpsHud.Instance;

        if (hud.PlayerCamera)
        {
            camera.transform.position = hud.PlayerCamera.transform.position;
            camera.transform.rotation = hud.PlayerCamera.transform.rotation;

            if (display)
            {
                // Clamp scope size
                scopeScale = Mathf.Clamp01(scopeScale);

                int h = Screen.height;
                int w = Screen.width;
                int s = Mathf.Clamp((int)((h < w ? h : w) * scopeScale), 0, maxScopePixelSize - 2);
                camera.pixelRect = new Rect((Screen.width / 2) - (s / 2), (Screen.height / 2) - (s / 2), s, s);

                scopeOverlay.transform.position = (Vector3.forward * 5).ToScreenPosition();
                scopeOverlay.transform.localScale = new Vector3(s + 2, s + 2, 1);
            }
        }
    }

    public void Enter(float fieldOfView)
    {
        FpsHud hud = FpsHud.Instance;

        if (!display && hud.PlayerCamera)
        {
            display = true;
            camera.enabled = true;
            camera.fieldOfView = fieldOfView;
            scopeOverlay.renderer.enabled = true;

            playerCameraCullingMask = hud.PlayerCamera.cullingMask;
            playerCameraClearFlags = hud.PlayerCamera.clearFlags;
            playerCameraBackgroundColor = hud.PlayerCamera.backgroundColor;

            hud.PlayerCamera.cullingMask = 0;
            hud.PlayerCamera.clearFlags = CameraClearFlags.SolidColor;
            hud.PlayerCamera.backgroundColor = Color.black;

            hud.ActiveCamera = camera;
        }
    }

    public void Exit()
    {
        FpsHud hud = FpsHud.Instance;

        if (display && hud.PlayerCamera)
        {
            display = false;
            camera.enabled = false;
            scopeOverlay.renderer.enabled = false;

            hud.PlayerCamera.cullingMask = playerCameraCullingMask;
            hud.PlayerCamera.clearFlags = playerCameraClearFlags;
            hud.PlayerCamera.backgroundColor = playerCameraBackgroundColor;
            hud.ActiveCamera = hud.PlayerCamera;
        }
    }
}