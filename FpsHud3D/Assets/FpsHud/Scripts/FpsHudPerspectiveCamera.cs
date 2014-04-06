using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class FpsHudPerspectiveCamera : MonoBehaviour
{
    public static RenderTexture Texture { get; private set; }

    int screenWidth = -1;
    int screenHeight = -1;
    FpsHudFrustrumAnchor[] anchoredObjects;

    void Start()
    {
        camera.enabled = true;
        anchoredObjects = FindObjectsOfType(typeof(FpsHudFrustrumAnchor)).Cast<FpsHudFrustrumAnchor>().ToArray();
        Init();
    }

    void Update()
    {
        Init();
        Anchor();
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

    void Anchor()
    {
        Vector3
            leftDir, rightDir, topDir, bottomDir,
            leftOrg, rightOrg, topOrg, bottomOrg,
            bottomRightDir, bottomRightOrg,
            bottomLeftDir, bottomLeftOrg,
            topLeftDir, topLeftOrg,
            topRightDir, topRightOrg;

        rightOrg = camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0.0f));
        rightDir = (camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 1.0f)) - rightOrg).normalized;

        leftOrg = camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0.0f));
        leftDir = (camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, 1.0f)) - leftOrg).normalized;

        topOrg = camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0.0f));
        topDir = (camera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 1.0f)) - topOrg).normalized;

        topLeftOrg = camera.ViewportToWorldPoint(new Vector3(0f, 1f, 0.0f));
        topLeftDir = (camera.ViewportToWorldPoint(new Vector3(0f, 1f, 1.0f)) - topLeftOrg).normalized;

        topRightOrg = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0.0f));
        topRightDir = (camera.ViewportToWorldPoint(new Vector3(1f, 1f, 1.0f)) - topRightOrg).normalized;

        bottomOrg = camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0.0f));
        bottomDir = (camera.ViewportToWorldPoint(new Vector3(0.5f, 0f, 1.0f)) - bottomOrg).normalized;

        bottomRightOrg = camera.ViewportToWorldPoint(new Vector3(1f, 0f, 0.0f));
        bottomRightDir = (camera.ViewportToWorldPoint(new Vector3(1f, 0f, 1.0f)) - bottomRightOrg).normalized;

        bottomLeftOrg = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0.0f));
        bottomLeftDir = (camera.ViewportToWorldPoint(new Vector3(0f, 0f, 1.0f)) - bottomLeftOrg).normalized;

        for (int i = 0; i < anchoredObjects.Length; ++i)
        {
            FpsHudFrustrumAnchor a = anchoredObjects[i];

            switch (a.Point)
            {
                case FpsHudFrustrumAnchor.AnchorPoint.TopLeft:
                    a.transform.position = (topLeftOrg + (topLeftDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.TopCenter:
                    a.transform.position = (topOrg + (topDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.TopRight:
                    a.transform.position = (topRightOrg + (topRightDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.MiddleLeft:
                    a.transform.position = (leftOrg + (leftDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.MiddleRight:
                    a.transform.position = (rightOrg + (rightDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.BottomLeft:
                    a.transform.position = (bottomLeftOrg + (bottomLeftDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.BottomCenter:
                    a.transform.position = (bottomOrg + (bottomDir * a.Distance)) + a.Offset;
                    break;

                case FpsHudFrustrumAnchor.AnchorPoint.BottomRight:
                    a.transform.position = (bottomRightOrg + (bottomRightDir * a.Distance)) + a.Offset;
                    break;
            }

        }
    }
}