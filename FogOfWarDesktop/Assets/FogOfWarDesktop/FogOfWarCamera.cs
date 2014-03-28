using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FogOfWarCamera : MonoBehaviour
{
    Transform parent;

    public static RenderTexture Texture { get; private set; }
    public static FogOfWarCamera Instance { get; private set; }

    void Start()
    {
        Instance = this;
        camera.enabled = true;
        camera.targetTexture = Texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

        parent = transform.parent;
    }

    void Update()
    {
        transform.position = parent.position;
        transform.rotation = parent.rotation;
    }
}