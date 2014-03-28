//#define RENDER_TEXTURE_MAP

using System;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    int zoomIndex = 0;
    float lastPing = 0f;
    float lastAutoDetect = 0f;

    HashSet<MapItem> mapItems = new HashSet<MapItem>();
    HashSet<MapItem> autoItems = new HashSet<MapItem>();
    HashSet<MapItem> manualItems = new HashSet<MapItem>();
    List<MapActivePing> activePings = new List<MapActivePing>();

    [SerializeField]
    Transform target;

#if RENDER_TEXTURE_MAP
    [SerializeField]
    Texture texture;

    [SerializeField]
    Camera textureCamera;
#else
    [SerializeField]
    Texture2D texture;
#endif

    [SerializeField]
    MapOrientation orientation = MapOrientation.TopLeft;

    [SerializeField]
    bool inverseMousePosition = true;

    [SerializeField]
    float screenSize = 256f;

    [SerializeField]
    float iconSize = 16f;

    [SerializeField]
    float marginLeftRight = 10f;

    [SerializeField]
    float marginTopBottom = 10f;

    [SerializeField]
    float worldSize = 128f;

    [SerializeField]
    Vector3 worldOrigin = Vector3.zero;

    [SerializeField]
    MapZoomLevel[] zoomLevels = new MapZoomLevel[0];

    [SerializeField]
    bool autoDetect = true;

    [SerializeField]
    LayerMask autoDetectLayers = 1;

    [SerializeField]
    float autoDetectInterval = 0.1f;

    [SerializeField]
    bool pingEnabled = true;

    [SerializeField]
    bool pingOnClick = true;

    [SerializeField]
    string pingButtonName = "Fire1";

    [SerializeField]
    float pingThreshold = 0.5f;

    [SerializeField]
    float pingDuration = 3f;

    [SerializeField]
    bool pingIgnoreOutsideMap = true;

    [SerializeField]
    Texture pingIcon = null;

    [SerializeField]
    bool pingIconAlphaBlend = true;

    [SerializeField]
    AudioClip pingSound = null;

    [SerializeField]
    AudioSource pingSoundSource = null;

    public MapOrientation Orientation
    {
        get
        {
            return orientation;
        }
    }

    public Texture PingIcon
    {
        get
        {
            return pingIcon;
        }
    }

    public bool PingIconAlphaBlend
    {
        get
        {
            return pingIconAlphaBlend;
        }
    }

    public float IconSize
    {
        get
        {
            return iconSize;
        }
    }

    public float IconSizeScaled
    {
        get
        {
            return iconSize * ZoomLevel.IconScale;
        }
    }

    public float WorldSize
    {
        get
        {
            return worldSize;
        }
    }

    public float WorldMapVisibleSize
    {
        get
        {
            return worldSize * (1f / ZoomLevel.MapScale);
        }
    }

    public float WorldRadius
    {
        get
        {
            Vector3 topRight = worldOrigin;

            topRight.z += worldSize / 2f;
            topRight.x += worldSize / 2f;

            return (topRight - worldOrigin).magnitude;
        }
    }

    public Vector3 WorldTopLeft
    {
        get
        {
            Vector3 p = worldOrigin;

            p.z += worldSize / 2f;
            p.x -= worldSize / 2f;

            return p;
        }
    }

    public Vector3 TargetPosition
    {
        get
        {
            if (target == null)
            {
                return Vector3.zero;
            }

            return new Vector3(target.position.x, 0, target.position.z);
        }
    }

    public MapZoomLevel ZoomLevel
    {
        get
        {
            if (zoomLevels.Length > 0)
            {
                zoomIndex = Mathf.Clamp(zoomIndex, 0, zoomLevels.Length - 1);
                return zoomLevels[zoomIndex];
            }

            return new MapZoomLevel();
        }
    }

    public Vector3 NormalizedMousePosition
    {
        get
        {
            Vector3 mpos = Input.mousePosition;
            mpos.y = Screen.height - mpos.y;

            if (inverseMousePosition)
            {
                return GUI.matrix.inverse * mpos;
            }
            else
            {
                return mpos;
            }
        }
    }

    public Vector3 MapWorldTopLeft
    {
        get
        {
            Vector3 v = TargetPosition;

            v.x -= WorldMapVisibleSize * 0.5f;
            v.z += WorldMapVisibleSize * 0.5f;
            v.x = Mathf.Clamp(v.x, WorldTopLeft.x, WorldTopLeft.x + WorldSize - WorldMapVisibleSize);
            v.z = Mathf.Clamp(v.z, WorldTopLeft.z - WorldSize + WorldMapVisibleSize, WorldTopLeft.z);

            return v;
        }
    }

    public Vector3 MapWorldBottomRight
    {
        get
        {
            Vector3 v = MapWorldTopLeft;

            v.x += WorldMapVisibleSize;
            v.z -= WorldMapVisibleSize;

            return v;
        }
    }

    public Texture Texture
    {
        get
        {
            return texture;
        }
    }

    public float TextureTop
    {
        get
        {
            float zDist = Mathf.Clamp((WorldTopLeft.z - TargetPosition.z) / worldSize, ZoomLevel.MapClamp, 1f - ZoomLevel.MapClamp);
            return -(TextureSize * zDist) + (ScreenSize * 0.5f);
        }
    }

    public float TextureLeft
    {
        get
        {
            float xDist = Mathf.Clamp((TargetPosition.x - WorldTopLeft.x) / worldSize, ZoomLevel.MapClamp, 1f - ZoomLevel.MapClamp);
            return -(TextureSize * xDist) + (ScreenSize * 0.5f);
        }
    }

    public float TextureSize
    {
        get
        {
            return screenSize * ZoomLevel.MapScale;
        }
    }

    public float ScreenSize
    {
        get
        {
            return screenSize;
        }
    }

    public HashSet<MapItem> MapItems
    {
        get { return mapItems; }
    }

    public float ScreenLeft
    {
        get
        {
            switch (orientation)
            {
                case MapOrientation.TopCenter: 
                case MapOrientation.BottomCenter:
                    return (Screen.width / 2f) - (screenSize / 2f);

                case MapOrientation.TopRight: 
                case MapOrientation.BottomRight:
                case MapOrientation.MiddleRight:
                    return Screen.width - screenSize - marginLeftRight;

                default:
                    return marginLeftRight;
            }
        }
    }

    public float ScreenTop
    {
        get
        {
            switch (orientation)
            {
                case MapOrientation.BottomLeft:
                case MapOrientation.BottomRight:
                case MapOrientation.BottomCenter:
                    return Screen.height - screenSize - marginTopBottom;

                case MapOrientation.MiddleLeft:
                case MapOrientation.MiddleRight:
                    return (Screen.height / 2f) - (screenSize / 2f);

                default:
                    return marginTopBottom;
            }
        }
    }

    public IEnumerable<MapActivePing> Pings
    {
        get
        {
            return activePings;
        }
    }

    public MapItem TargetMapItem
    {
        get
        {
            return target.GetComponent<MapItem>();
        }
    }

    public void ZoomIn()
    {
        zoomIndex = Mathf.Clamp(zoomIndex + 1, 0, zoomLevels.Length - 1);
    }

    public void ZoomOut()
    {
        zoomIndex = Mathf.Clamp(zoomIndex - 1, 0, zoomLevels.Length - 1);
    }

    public bool ScreenToMapCoords(float screenX, float screenY, out float mapX, out float mapY)
    {
        mapY = screenY - ScreenTop;
        mapX = screenX - ScreenLeft;
        return mapX > 0 && mapX <= screenSize && mapY > 0 && mapY <= screenSize;
    }

    public void WorldToMapCoords(Vector3 p, out float left, out float top)
    {
        float xDist = Mathf.Clamp01((p.x - MapWorldTopLeft.x) / WorldMapVisibleSize);
        float zDist = Mathf.Clamp01((MapWorldTopLeft.z - p.z) / WorldMapVisibleSize);

        left = ScreenSize * xDist;
        top = ScreenSize * zDist;
    }

    public void AddItem(MonoBehaviour behaviour)
    {
        AddItem(behaviour.GetComponent<MapItem>());
    }

    public void AddItem(GameObject go)
    {
        AddItem(go.GetComponent<MapItem>());
    }

    public void AddItem(MapItem item)
    {
        if (item != null && item.transform != target)
        {
            autoItems.Remove(item);
            manualItems.Add(item);
        }
    }

    public void RemoveItem(MonoBehaviour behaviour)
    {
        RemoveItem(behaviour.GetComponent<MapItem>());
    }

    public void RemoveItem(GameObject go)
    {
        RemoveItem(go.GetComponent<MapItem>());
    }

    public void RemoveItem(MapItem item)
    {
        if (item != null)
        {
            manualItems.Remove(item);
        }
    }

    public void Ping(Vector3 p)
    {
        Ping(p, "");
    }

    public void Ping(Vector3 p, string label)
    {
        Ping(p, label, null);
    }

    public void Ping(Vector3 p, object data)
    {
        Ping(p, "", data);
    }

    public void Ping(Vector3 p, string label, object data)
    {
        if (pingEnabled && Time.time - lastPing > pingThreshold)
        {
            Vector3 l = MapWorldTopLeft;
            Vector3 r = MapWorldBottomRight;

            if (!pingIgnoreOutsideMap || (p.x >= l.x && p.x <= r.x && p.z <= l.z && p.z >= r.z))
            {
                lastPing = Time.time;
                activePings.Add(new MapActivePing { Label = label, Time = lastPing, Position = p, Data = data });

                if (pingSound != null && pingSoundSource != null)
                {
                    pingSoundSource.PlayOneShot(pingSound);
                }
            }
        }
    }

    public Vector3 MapToWorldCoords(float left, float top)
    {
        float zDist = Mathf.Clamp(top, 0f, ScreenSize) / ScreenSize;
        float xDist = Mathf.Clamp(left, 0f, ScreenSize) / ScreenSize;

        Vector3 v = MapWorldTopLeft;

        v.x += xDist * WorldMapVisibleSize;
        v.z -= zDist * WorldMapVisibleSize;

        return v;
    }

    public void RedrawMinimap()
    {
#if RENDER_TEXTURE_MAP
        if (texture != null && texture is RenderTexture && textureCamera != null && textureCamera.enabled == false)
        {
            textureCamera.aspect = 1f;
            textureCamera.enabled = true;

            StartCoroutine(disableCamera());
        }
#endif
    }

#if RENDER_TEXTURE_MAP
    System.Collections.IEnumerator disableCamera()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        textureCamera.enabled = false;
    }
#endif

    void AddMapItems(HashSet<MapItem> items)
    {
        foreach (MapItem item in items)
        {
            AddMapItem(item);
        }
    }

    void AddMapItem(MapItem item)
    {
        if (item != null && item.enabled && item.gameObject.active)
        {
            Vector3 p = item.Position;
            Vector3 l = MapWorldTopLeft;
            Vector3 r = MapWorldBottomRight;

            if (p.x >= l.x && p.x <= r.x && p.z <= l.z && p.z >= r.z)
            {
                // Set items map as this map
                item.Map = this;

                // Store it in set of items to be drawn
                mapItems.Add(item);
            }
        }
    }

    void AutoDetect()
    {
        if (autoDetect && Time.time - lastAutoDetect > autoDetectInterval)
        {
            // Store time
            lastAutoDetect = Time.time;

            // Clear current auto items
            autoItems.Clear();

            // Find all items in range of map
            foreach (Collider c in Physics.OverlapSphere(worldOrigin, WorldRadius, autoDetectLayers))
            {
                // Find component
                MapItem item = c.GetComponent<MapItem>();

                // Not null, not a manually added item and not the target item
                if (item != null && !manualItems.Contains(item) && item.transform != target)
                {
                    autoItems.Add(item);
                }
            }
        }
    }

    void CleanUpOldPings()
    {
        // This is O(n) - but that doesn't really matter as it's so few elements
        for (var i = 0; i < activePings.Count; ++i)
        {
            if (Time.time - activePings[i].Time > pingDuration)
            {
                activePings.RemoveAt(i);
            }
        }
    }

    void CheckForPing()
    {
        if (pingEnabled && pingOnClick && Input.GetButtonDown(pingButtonName))
        {
            Vector3 mpos = NormalizedMousePosition;
            
            float top;
            float left;

            if (ScreenToMapCoords(mpos.x, mpos.y, out left, out top))
            {
                Ping(MapToWorldCoords(left, top), "Click!");
            }
        }
    }

    void Start()
    {
        Instance = this;
        RedrawMinimap();

        Debug.LogWarning("MiniMap: If you're using Unity Pro, enable pro-only RenderTexture features by uncommenting #define RENDER_TEXTURE_MAP at the top in Map.cs. If you're not, you can remove this debug message from the Start method in the same file.");
    }

    void Update()
    {
        Instance = this;

        AutoDetect();
        CleanUpOldPings();
        CheckForPing();

        mapItems.Clear();

        AddMapItems(autoItems);
        AddMapItems(manualItems);
        AddMapItem(TargetMapItem);
    }
}

[Serializable]
public class MapZoomLevel
{
    public float MapScale = 1f;
    public float IconScale = 1f;
    public float MapScaleMultiplier { get { return 1f / MapScale; } }
    public float MapClamp { get { return (1f / MapScale) * 0.5f; } }

    public static MapZoomLevel Init(MapZoomLevel level)
    {
        level.MapScale = Mathf.Clamp(level.MapScale, 1f, float.MaxValue);
        level.IconScale = Mathf.Clamp(level.IconScale, 1f, float.MaxValue);

        return level;
    }
}

public struct MapActivePing
{
    public float Time;
    public string Label;
    public Vector3 Position;
    public object Data;
}

public enum MapOrientation
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}