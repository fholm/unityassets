using UnityEngine;

public class RPGCamera : MonoBehaviour
{
    public static RPGCamera Instance { get; private set; }

    float targetYaw;
    float targetPitch;
    float targetDistance;

    float currentYaw;
    float currentPitch;
    float currentDistance;
    float currentMinDistance;
    float currentMaxDistance;

    float realDistance = 0f;

    public bool DisplayDebugGizmos = true;

    public Camera Camera = null;
    public Transform Target = null;

    public float MinDistance = 1f;
    public float MaxDistance = 32f;
    public float MinPitch = -80f;
    public float MaxPitch = 80f;
    public float ZoomSpeed = 16f;
    public float RotationMouseSpeed = 4f;

    public bool SmoothZoom = true;
    public float SmoothZoomSpeed = 8f;

    public bool SmoothRotation = true;
    public float SmoothRotationSpeed = 8f;

    public bool SmoothAutoRotation = true;
    public float SmoothAutoRotationSpeed = 4f;

    public LayerMask Obstacles = 0;
    public Vector3 TargetOffset = Vector3.zero;

    public string ZoomAxis = "Mouse ScrollWheel";
    public string YawAxis = "Mouse X";
    public string PitchAxis = "Mouse Y";
    public string MouseRotateButton = "Fire1";
    public string MouseLookButton = "Fire2";

    public bool LockCameraBehindTarget { get; set; }
    public bool RotateCameraBehindTarget { get; set; }

    public bool HasCamera { get { return Camera != null; } }
    public bool HasTarget { get { return Target != null; } }
    public Vector3 TargetPosition { get { return HasTarget ? Target.position + TargetOffset : TargetOffset; } }

    void Start()
    {
        Instance = this;

        if (!HasCamera)
        {
            Camera = GetComponentInChildren<Camera>();
        }

        if (!HasTarget)
        {
            try
            {
                Target = GameObject.FindGameObjectWithTag("CameraTarget").transform;
            }
            catch
            {

            }
        }

        if (HasCamera)
        {
            Camera.transform.localPosition = Vector3.zero;
            Camera.transform.localRotation = Quaternion.identity;
        }

        MinPitch = Mathf.Clamp(MinPitch, -85f, 0f);
        MaxPitch = Mathf.Clamp(MaxPitch, 0f, 85f);
        MinDistance = Mathf.Max(0f, MinDistance);

        currentMinDistance = MinDistance;
        currentMaxDistance = MaxDistance;

        currentYaw = targetYaw = 0f;
        currentPitch = targetPitch = Mathf.Lerp(MinPitch, MaxPitch, 0.6f);
        currentDistance = targetDistance = realDistance = Mathf.Lerp(MinDistance, MaxDistance, 0.5f);
    }

    void LateUpdate()
    {
        Instance = this;

        if (!HasCamera)
        {
            Debug.LogError("No camera found");
            return;
        }

        if (!HasTarget)
        {
            Debug.LogError("No target found");
            return;
        }

        bool rotate = RPGInputUtils.GetButtonSafe(MouseRotateButton, false);
        bool mouseLook = RPGInputUtils.GetButtonSafe(MouseLookButton, false);

        bool smoothRotation = SmoothRotation || SmoothAutoRotation;
        float smoothRotationSpeed = SmoothRotationSpeed;

        // This defines our "real" distance to the player
        realDistance -= RPGInputUtils.GetAxisRawSafe(ZoomAxis, 0f) * ZoomSpeed;
        realDistance = Mathf.Clamp(realDistance, MinDistance, MaxDistance);

        // This is the distance we want to (clamped to what is viewable)
        targetDistance = realDistance;
        targetDistance = Mathf.Clamp(targetDistance, currentMinDistance, currentMaxDistance);

        // This is our current distance
        if (SmoothZoom)
        {
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * SmoothZoomSpeed);
        }
        else
        {
            currentDistance = targetDistance;
        }
        
        // Calculate offset vector
        Vector3 offset = new Vector3(0, 0, -currentDistance);

        // LMB is not down, but we should rotate camera behind target
        if(!rotate && RotateCameraBehindTarget)
        {
            targetYaw = RPGInputUtils.SignedAngle(offset.normalized, -Target.forward, Vector3.up);
            smoothRotation = SmoothAutoRotation;
            smoothRotationSpeed = SmoothAutoRotationSpeed;
        }

        // Only LMB down and no lock
        if (rotate && !mouseLook && !LockCameraBehindTarget)
        {
            targetYaw += (RPGInputUtils.GetAxisRawSafe(YawAxis, 0f) * RotationMouseSpeed);
            targetPitch -= (RPGInputUtils.GetAxisRawSafe(PitchAxis, 0f) * RotationMouseSpeed);
            targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
            smoothRotation = SmoothRotation;
            smoothRotationSpeed = SmoothRotationSpeed;
        }

        // RMB 
        if (mouseLook && LockCameraBehindTarget)
        {
            targetPitch -= (RPGInputUtils.GetAxisRawSafe(PitchAxis, 0f) * RotationMouseSpeed);
            targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
        }

        // Lock camera behind target, this overrides everything
        if (LockCameraBehindTarget)
        {
            targetYaw = RPGInputUtils.SignedAngle(offset.normalized, -Target.transform.forward, Vector3.up);
            smoothRotation = false;
        }

        // Clamp targetYaw to -180, 180
        targetYaw = Mathf.Repeat(targetYaw + 180f, 360f) - 180f;

        if (!smoothRotation)
        {
            currentYaw = targetYaw;
            currentPitch = targetPitch;
        }
        else
        {
            // Clamp smooth currentYaw to targetYaw and clamp it to -180, 180
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.deltaTime * smoothRotationSpeed);
            currentYaw = Mathf.Repeat(currentYaw + 180f, 360f) - 180f;

            // Smooth pitch
            currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, Time.deltaTime * smoothRotationSpeed);
        }

        // Rotate offset vector
        offset = Quaternion.Euler(currentPitch, currentYaw, 0f) * offset;

        // Position camera holder correctly
        transform.position = TargetPosition + offset;

        // And then have the camera look at our target
        Camera.transform.LookAt(TargetPosition);

        // Make sure we don't collide with anything
        float closest = float.MaxValue;
        bool mid = AvoidCollision(transform.position, ref closest);
        bool bottomLeft = AvoidCollision(Camera.ScreenToWorldPoint(new Vector3(0, 0, Camera.nearClipPlane)), ref closest);
        bool bottomRight = AvoidCollision(Camera.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.nearClipPlane)), ref closest);
        bool topLeft = AvoidCollision(Camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.nearClipPlane)), ref closest);
        bool topRight = AvoidCollision(Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.nearClipPlane)), ref closest);

        if (mid && bottomLeft && bottomRight && topLeft && topRight)
        {
            currentMinDistance = MinDistance;
            currentMaxDistance = MaxDistance;
        }
        else
        {
            currentMinDistance = Mathf.Min(currentMinDistance, 1f);
            currentMaxDistance = Mathf.Max(currentMinDistance + 0.05f, closest * 0.9f);
        }

        // Clear this flag
        LockCameraBehindTarget = false;
        RotateCameraBehindTarget = false;
    }

    bool AvoidCollision(Vector3 point, ref float closest)
    {
        RaycastHit hit;
        Vector3 direction = (point - TargetPosition).normalized;

        if (Physics.Raycast(TargetPosition, direction, out hit, MaxDistance, Obstacles))
        {
            float calculatedDistance = (hit.point - Target.position).magnitude;

            if (calculatedDistance < closest)
            {
                closest = calculatedDistance;
            }

            return false;
        }

        return true;
    }

    void OnDrawGizmos()
    {
        if (DisplayDebugGizmos && HasTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(TargetPosition, currentMinDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(TargetPosition, currentMaxDistance);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(TargetPosition, Camera.ScreenToWorldPoint(new Vector3(0, 0, Camera.nearClipPlane)));
            Gizmos.DrawLine(TargetPosition, Camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.nearClipPlane)));
            Gizmos.DrawLine(TargetPosition, Camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, Camera.nearClipPlane)));
            Gizmos.DrawLine(TargetPosition, Camera.ScreenToWorldPoint(new Vector3(0, Screen.height, Camera.nearClipPlane)));
            Gizmos.DrawLine(TargetPosition, transform.position);
        }
    }
}
