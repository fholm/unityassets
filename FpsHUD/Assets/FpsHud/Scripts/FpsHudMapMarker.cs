using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FpsHudMapMarker : MonoBehaviour
{
    Mesh quad;
    int icon = -1;
    Color[] colors = new Color[4];
    Vector2[] uv = new Vector2[4];

    [SerializeField]
    Transform target = null;

    [SerializeField]
    int atlasSize = 4;

    [SerializeField]
    int iconIndex = 0;

    [SerializeField]
    Color iconColor = Color.cyan;

    [SerializeField]
    bool rotate = true;

    [SerializeField]
    bool clamp = false;

    [SerializeField]
    int clampIcon = -1;

    [SerializeField]
    bool clampRotate = false;

    [SerializeField]
    string tooltip = "";

    public bool MapRotate
    {
        get { return rotate; }
    }

    public bool MapHasTooltip
    {
        get { return !string.IsNullOrEmpty(tooltip); }
    }

    public string MapTooltip
    {
        get { return tooltip; }
    }

    public bool MapClamp
    {
        get { return clamp; }
    }

    public bool MapClampRotate
    {
        get { return clampRotate; }
    }

    public Transform MapTarget
    {
        get { return target; }
    }

    void Start()
    {
        // Copy our quad mesh
        quad = GetComponent<MeshFilter>().mesh;

        MapSetIcon(iconIndex);
        MapSetColor(iconColor);

        // Register icon with map
        FpsHudMapCamera.RegisterIcon(this);
    }

    void Awake()
    {
        FpsHudMapCamera.RegisterIcon(this);
    }

    void OnEnable()
    {
        FpsHudMapCamera.RegisterIcon(this);
    }

    void OnDestroy()
    {
        FpsHudMapCamera.UnregisterIcon(this);
    }

    void OnDisable()
    {
        FpsHudMapCamera.UnregisterIcon(this);
    }

    public void MapSetIcon(int index)
    {
        icon = Mathf.Clamp(index, 0, atlasSize * atlasSize);

        float r = 1f / atlasSize;
        float x = r * (icon % atlasSize);
        float y = r * (icon / atlasSize);

        uv[0].x = x;
        uv[0].y = y;
        uv[1].x = x + r;
        uv[1].y = y;
        uv[2].x = x;
        uv[2].y = y + r;
        uv[3].x = x + r;
        uv[3].y = y + r;

        quad.uv = uv;
    }

    public void MapSetColor(Color color)
    {
        for (int i = 0; i < uv.Length; ++i)
        {
            colors[i] = color;
        }

        quad.colors = colors;
    }

    public void MapSetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void MapSetSize(int size)
    {
        transform.localScale = new Vector3(size, size, 1);
    }

    public void MapSetRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void MapSetClampState(bool clamping)
    {
        if (clamp)
        {
            if (clamping)
            {
                if (icon != clampIcon && clampIcon != -1)
                {
                    iconIndex = icon;
                    MapSetIcon(clampIcon);
                }
            }
            else if(icon != iconIndex)
            {
                MapSetIcon(iconIndex);
            }
        }
    }
}