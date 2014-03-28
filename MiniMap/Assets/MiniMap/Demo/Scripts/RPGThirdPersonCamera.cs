using UnityEngine;

public class RPGThirdPersonCamera : MonoBehaviour
{
    public static RPGThirdPersonCamera Instance { get; private set; }

    float targetYaw;
    float targetPitch;
    float targetDistance;
    float yClamp = float.MinValue;

    float currentYaw;
    float currentPitch;
    float currentDistance;
    float currentMinDistance;
    float currentMaxDistance;

    float realDistance = 0f;

    public Camera Camera = null;
    public Transform Target = null;

    public float MinDistance = 1f;
    public float MaxDistance = 32f;
    public float MinPitch = -80f;
    public float MaxPitch = 80f;
    public float ZoomSpeed = 16f;

    public Vector3 TargetOffset = Vector3.zero;

    public string ZoomAxis = "Mouse ScrollWheel";
    public string YawAxis = "Mouse X";
    public string PitchAxis = "Mouse Y";
    public string MouseLookButton = "Fire2";

    const bool LockCameraBehindTarget = true;
    const bool RotateCameraBehindTarget = true;

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
            return;
        }

        if (!HasTarget)
        {
            return;
        }

        bool mouseLook = RPGControllerUtils.GetButtonSafe(MouseLookButton, false);

        // This defines our "real" distance to the player
        realDistance -= RPGControllerUtils.GetAxisRawSafe(ZoomAxis, 0f) * ZoomSpeed;
        realDistance = Mathf.Clamp(realDistance, MinDistance, MaxDistance);

        // This is the distance we want to (clamped to what is viewable)
        targetDistance = realDistance;
        targetDistance = Mathf.Clamp(targetDistance, currentMinDistance, currentMaxDistance);
        currentDistance = targetDistance;

        // Calculate offset vector
        Vector3 offset = new Vector3(0, 0, -currentDistance);

        // RMB 
        if (mouseLook && LockCameraBehindTarget)
        {
            targetPitch -= (RPGControllerUtils.GetAxisRawSafe(PitchAxis, 0f) * 4f);
            targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
        }

        // Lock camera behind target, this overrides everything
        if (LockCameraBehindTarget)
        {
            targetYaw = RPGControllerUtils.SignedAngle(offset.normalized, -Target.transform.forward, Vector3.up);
        }

        // Clamp targetYaw to -180, 180
        targetYaw = Mathf.Repeat(targetYaw + 180f, 360f) - 180f;
        currentYaw = targetYaw;
        currentPitch = targetPitch;


        // Rotate offset vector
        offset = Quaternion.Euler(currentPitch, currentYaw, 0f) * offset;

        // Position camera holder correctly
        transform.position = TargetPosition + offset;

        Vector3 p = transform.position;

        transform.position = new Vector3(p.x, Mathf.Clamp(p.y, yClamp, float.MaxValue), p.z);

        // And then have the camera look at our target
        Camera.transform.LookAt(TargetPosition);
    }
}
