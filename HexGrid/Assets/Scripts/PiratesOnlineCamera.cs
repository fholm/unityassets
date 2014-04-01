using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PiratesOnlineCamera : MonoBehaviour
{
    float fov = PiratesOnlineConstants.MaxFov;

    [SerializeField]
    float scrollSpeed = 100f;

    void Update()
    {
        fov = Mathf.Clamp(
            fov + (-Input.GetAxis("Mouse ScrollWheel") * scrollSpeed), 
            PiratesOnlineConstants.MinFov, 
            PiratesOnlineConstants.MaxFov
        );

        camera.fov = fov;
    }
}