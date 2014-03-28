using System;
using UnityEngine;

public class MapItem : MonoBehaviour
{
    Vector3 oldForward = Vector3.forward;

    protected Texture2D realTexture = null;

    [SerializeField]
    protected Texture2D iconTexture = null;

    [SerializeField]
    protected bool iconAlphaBlend = true;

    [SerializeField]
    public string Label = "";

    [SerializeField]
    protected bool labelFallbackOnName = true;

    [SerializeField]
    public float ZOrder = 0;

    [SerializeField]
    protected bool rotateAlongForwardAxis = false;

    [SerializeField]
    protected float rotateAngleAccuracy = 2f;

    [HideInInspector]
    public Map Map = null;

    public virtual Vector3 Position
    {
        get { return transform.position; }
    }

    void Start()
    {
        realTexture = iconTexture;

        if (rotateAlongForwardAxis)
        {
            oldForward = transform.forward;
            realTexture = rotateTexture(iconTexture, signedAngle(transform.forward, Vector3.forward, Vector3.up));
        }
    }

    void Update()
    {
        if (rotateAlongForwardAxis)
        {
            if (Vector3.Angle(oldForward, transform.forward) >= rotateAngleAccuracy)
            {
                oldForward = transform.forward;
                realTexture = rotateTexture(iconTexture, signedAngle(transform.forward, Vector3.forward, Vector3.up));
            }
        }
    }

    public virtual bool Draw(bool labelDrawn)
    {
        if (realTexture != null)
        {
            float top;
            float left;
            float size = Map.IconSizeScaled;

            Map.WorldToMapCoords(transform.position, out left, out top);

            top += Map.ScreenTop - size / 2f;
            left += Map.ScreenLeft - size / 2f;

            drawIcon(left, top, size, realTexture, iconAlphaBlend);

            string label =
                String.IsNullOrEmpty(this.Label)
                    ? (labelFallbackOnName ? name : "")
                    : this.Label;

            if (!labelDrawn && !String.IsNullOrEmpty(label))
            {
                Vector3 mpos = Map.NormalizedMousePosition;

                if (mpos.x >= left && mpos.x <= left + size && mpos.y >= top && mpos.y <= top + size)
                {
                    drawLabel(left, top, size, label);

                    if (Input.GetMouseButtonDown(0))
                    {
                        SendMessage("MapLeftClick", SendMessageOptions.DontRequireReceiver);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        SendMessage("MapRightClick", SendMessageOptions.DontRequireReceiver);
                    }

                    return true;
                }
            }
        }

        return false;
    }

    protected virtual void drawIcon(float left, float top, float size, Texture texture, bool alphaBlend)
    {
        GUI.DrawTexture(new Rect(left, top, size, size), texture, ScaleMode.ScaleAndCrop, alphaBlend);
    }

    protected virtual void drawLabel(float left, float top, float size, string label)
    {
        GUI.Box(new Rect(left + size, top + (size / 4), 150, 20), label);
    }

    float signedAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
    }

    protected Texture2D rotateTexture(Texture2D tex, float angle)
    {
        Texture2D rotImage = new Texture2D(tex.width, tex.height);

        int x, y;
        float x1, y1, x2, y2;

        int w = tex.width;
        int h = tex.height;

        float x0 = rotX(angle, -w / 2.0f, -h / 2.0f) + w / 2.0f;
        float y0 = rotY(angle, -w / 2.0f, -h / 2.0f) + h / 2.0f;

        float dx_x = rotX(angle, 1.0f, 0.0f);
        float dx_y = rotY(angle, 1.0f, 0.0f);
        float dy_x = rotX(angle, 0.0f, 1.0f);
        float dy_y = rotY(angle, 0.0f, 1.0f);

        x1 = x0;
        y1 = y0;

        for (x = 0; x < tex.width; x++)
        {
            x2 = x1;
            y2 = y1;

            for (y = 0; y < tex.height; y++)
            {
                x2 += dx_x;
                y2 += dx_y;
                rotImage.SetPixel((int)Mathf.Floor(x), (int)Mathf.Floor(y), getPixel(tex, x2, y2));
            }

            x1 += dy_x;
            y1 += dy_y;
        }

        rotImage.Apply();
        return rotImage;
    }

    Color getPixel(Texture2D tex, float x, float y)
    {
        Color pix;

        int x1 = (int)Mathf.Floor(x);
        int y1 = (int)Mathf.Floor(y);

        if (x1 > tex.width || x1 < 0 || y1 > tex.height || y1 < 0)
        {
            pix = Color.clear;
        }
        else
        {
            pix = tex.GetPixel(x1, y1);

        }

        return pix;
    }

    float rotX(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return x * cos + y * (-sin);
    }

    float rotY(float angle, float x, float y)
    {
        float cos = Mathf.Cos(angle / 180.0f * Mathf.PI);
        float sin = Mathf.Sin(angle / 180.0f * Mathf.PI);
        return x * sin + y * cos;
    }
}