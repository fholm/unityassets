using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FpsHudMarker : MonoBehaviour
{
    [SerializeField]
    public Transform TrackTransform;

    [SerializeField]
    public Vector3 TrackOffset;

    [SerializeField]
    public bool ClampToSides = false;

    [SerializeField]
    public float MaxDistance = 256;

    [SerializeField]
    public Vector2 ClampInset = Vector2.zero;

    void Start()
    {
        if (TrackTransform == null)
        {
            TrackTransform = transform;
        }
    }

    void LateUpdate()
    {
        FpsHud hud = FpsHud.Instance;
        Vector3 v = ((TrackTransform.position + TrackOffset) - hud.ActiveCamera.transform.position).normalized;

        float xAngle = FpsHudUtils.SignedAngle(hud.ActiveCamera.transform.forward, v, Vector3.up);
        float yAngle = FpsHudUtils.SignedAngle(hud.ActiveCamera.transform.forward, v, Vector3.right);

        float hFov = hud.ActiveCamera.fov;
        float vFov = hFov * hud.ActiveCamera.aspect;

        float hFov2 = hFov / 2;
        float vFov2 = vFov / 2;

        if (xAngle < -vFov2 || xAngle > vFov2 || yAngle < -hFov2 || xAngle > hFov2)
        {
            if (ClampToSides)
            {
                if (xAngle < -90 || xAngle > 90)
                {
                    // Calculate the flipped inverse angle
                    xAngle = -FpsHudUtils.SignedAngle(-hud.ActiveCamera.transform.forward, v, Vector3.up);

                    // Recalculate yAngle
                    Vector3 yForward = Quaternion.Euler(hud.ActiveCamera.transform.rotation.eulerAngles.x, 0, 0) * Vector3.forward;

                    // Y-only angle
                    yAngle = FpsHudUtils.SignedAngle(yForward, v, Vector3.right);
                    yAngle = yAngle < 0 ? -vFov2 : vFov2;
                }

                xAngle = Mathf.Clamp(xAngle, -vFov2, vFov2);
                yAngle = Mathf.Clamp(yAngle, -vFov2, vFov2);
            }
            else
            {
                renderer.enabled = false;
                return;
            }
        }

        float xRatio = (xAngle + vFov2) / vFov;
        float yRatio = 1 - ((yAngle + hFov2) / hFov);

        float xMin = -Screen.width / 2;
        float yMin = -Screen.height / 2;

        float xPosition = xMin + Screen.width * xRatio;
        float yPosition = yMin + Screen.height * yRatio;

        renderer.enabled = true;
        transform.position = new Vector3(xPosition, yPosition, 1).ToScreenPosition();
    }
}