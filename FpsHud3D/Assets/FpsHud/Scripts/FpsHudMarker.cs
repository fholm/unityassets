using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FpsHudMarker : MonoBehaviour
{
    GameObject textObject;

    [SerializeField]
    public Transform TrackTransform;

    [SerializeField]
    public Vector3 TrackOffset;

    [SerializeField]
    public bool ClampToSides = false;

    [SerializeField]
    public float MaxDistance = 256;

    [SerializeField]
    public Vector2 ClampInset = Vector2.zero;

    [SerializeField]
    public float ScaleFrom = 0.25f;

    [SerializeField]
    public float ScaleTo = 2f;

    [SerializeField]
    public bool DisplayDistance;

    [SerializeField]
    public Font DisplayFont;

    [SerializeField]
    public Vector2 DisplayFontInset = Vector2.zero;

    void Start()
    {
        if (TrackTransform == null)
        {
            TrackTransform = transform.parent;
        }

        if (DisplayDistance)
        {
            textObject = new GameObject("Text");
            textObject.transform.parent = transform;
            textObject.layer = gameObject.layer;
            textObject.transform.localPosition = new Vector3(-0.28f, -0.3f, 0);

            MeshRenderer trenderer =textObject.AddComponent<MeshRenderer>();
            TextMesh tmesh = textObject.AddComponent<TextMesh>();

            tmesh.anchor = TextAnchor.UpperLeft;
            tmesh.font = DisplayFont;
            tmesh.characterSize = 0.05f;

            trenderer.material = DisplayFont.material;
        }
    }

    void LateUpdate()
    {
        FpsHud hud = FpsHud.Instance;
        Camera cam = hud.PlayerCamera;

        if (!cam)
        {
            return;
        }

        Vector3 viewPort = cam.WorldToViewportPoint(TrackTransform.position);
        Vector3 v = TrackTransform.position - cam.transform.position;

        float dn = v.sqrMagnitude / (MaxDistance * MaxDistance);
        float s = Mathf.Lerp(ScaleFrom, ScaleTo, dn);

        if (DisplayDistance)
        {
            textObject.transform.localPosition = new Vector3(-0.28f, -0.3f, 0);
            textObject.GetComponent<TextMesh>().text = System.Math.Round(v.magnitude, 1) + "m";
        }

        if (ClampToSides)
        {
            if (viewPort.z >= 0)
            {
                viewPort.z = 1;
            }
            else
            {
                viewPort.x = 1f - viewPort.x;
                viewPort.y = (TrackTransform.position.y > cam.transform.position.y) ? 1f : 0f;
                viewPort.z = 1;
            }

            float xMin = 0f + ClampInset.x;
            float xMax = 1f - ClampInset.x;

            float yMin = 0f + ClampInset.y;
            float yMax = 1f - ClampInset.y;

            viewPort.x = Mathf.Clamp(viewPort.x, xMin, xMax);
            viewPort.y = Mathf.Clamp(viewPort.y, yMin, yMax);

            if (viewPort.y <= yMin)
            {
                textObject.transform.localPosition = new Vector3(-0.28f, -0.3f + DisplayFontInset.y);
            }
        }

        transform.position = FpsHud.Instance.PerspectiveCamera.ViewportToWorldPoint(viewPort);
        transform.localScale = new Vector3(s, s, s);

        renderer.enabled = true;
    }
}