using UnityEngine;
using System.Collections;

public class FpsHudAnchor : MonoBehaviour
{
    public int Width = 0;
    public int Height = 0;
    public Vector3 Offset = Vector2.zero;
    public FpsHudAnchorMode Mode = FpsHudAnchorMode.BottomLeft;

    void Update()
    {
        int w = Screen.width / 2;
        int h = Screen.height / 2;

        switch (Mode)
        {
            case FpsHudAnchorMode.BottomLeft:
                transform.position =
                    Offset + (new Vector3(
                        -w,
                        -h + Height,
                        1
                    ));
                break;

            case FpsHudAnchorMode.BottomCenter:
                break;

            case FpsHudAnchorMode.BottomRight:
                transform.position =
                    Offset + (new Vector3(
                        w - Width,
                        -h + Height,
                        1
                    ));
                break;
        }
    }

    void LateUpdate()
    {
        Update();
    }
}


public enum FpsHudAnchorMode
{
    BottomLeft,
    BottomCenter,
    BottomRight
}