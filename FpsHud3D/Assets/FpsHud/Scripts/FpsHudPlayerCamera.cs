using UnityEngine;
using System.Collections;

public class FpsHudPlayerCamera : MonoBehaviour
{
    public static RenderTexture Texture { get; private set; }
    public static FpsHudPlayerCamera Instance { get; private set; }

    int screenWidth = -1;
    int screenHeight = -1;

    void Start()
    {
        Instance = this;
        camera.enabled = true;
        Init();
    }

    void Update()
    {
        Init();
    }

    void Init()
    {
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            camera.targetTexture = Texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        }
    }
}