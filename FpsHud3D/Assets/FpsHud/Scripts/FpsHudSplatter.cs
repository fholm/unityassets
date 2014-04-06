using UnityEngine;

public class FpsHudSplatter : MonoBehaviour
{
    int n = 0;
    FpsHudSplatterQuad[] splatters;

    [SerializeField]
    int quadMaxSize = 256;

    [SerializeField]
    int quadMinSize = 128;

    [SerializeField]
    float displayTime = 2f;

    [SerializeField]
    Material material;

    [SerializeField]
    GameObject quadPrefab;

    [SerializeField]
    Transform target;

    [SerializeField]
    Texture2D[] Textures;

    public Transform Target
    {
        get { return target; }
        set
        {
            target = value;
        }
    }


    void Start()
    {
        splatters = new FpsHudSplatterQuad[Textures.Length];

        for (int i = 0; i < splatters.Length; ++i)
        {
            GameObject go = GameObject.Instantiate(quadPrefab) as GameObject;

            // Setup object
            go.layer = gameObject.layer;
            go.transform.parent = transform;
            go.transform.localScale = new Vector3(quadMaxSize, quadMaxSize, 1);
            go.renderer.material = new Material(material);
            go.renderer.material.mainTexture = Textures[i];
            go.renderer.enabled = false;

            // 
            splatters[i] = go.GetComponent<FpsHudSplatterQuad>();
        }

        if (!Target && FpsHud.Instance.PlayerCamera)
        {
            Target = FpsHud.Instance.PlayerCamera.transform;
        }
    }

    public void Display()
    {
        Display((FpsHudSplatterSide)Random.Range(0, 4));
    }

    public void Display(Vector3 source)
    {
        if(target) 
        {
            Vector3 forward = target.forward;

            // Ignore y rotation
            source.y = 0;
            forward.y = 0;
            source.Normalize();
            forward.Normalize();

            Vector3 direction = (source - target.position).normalized;
            float angle = FpsHudUtils.SignedAngle(target.forward, direction, Vector3.up);

            // Intfront of us
            if (angle >= -45f && angle <= 45f)
            {
                Display(FpsHudSplatterSide.Top);
            }

            // Left side
            else if (angle <= -45f && angle >= -135f)
            {
                Display(FpsHudSplatterSide.Left);
            }

            // Right side
            else if (angle >= 45f && angle <= 135f)
            {
                Display(FpsHudSplatterSide.Right);
            }

            // Behind
            else
            {
                Display(FpsHudSplatterSide.Bottom);
            }
        }
    }

    public void Display(FpsHudSplatterSide side)
    {
        if (FpsHud.Instance.PlayerCamera)
        {
            n = ++n % splatters.Length;

            int w = Screen.width / 2;
            int h = Screen.height / 2;
            int s = Random.Range(quadMinSize, quadMaxSize);

            splatters[n].renderer.enabled = true;
            splatters[n].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            splatters[n].transform.localScale = new Vector3(s, s, 1);
            splatters[n].StartTime = Time.time;
            splatters[n].Duration = displayTime;

            switch (side)
            {
                case FpsHudSplatterSide.Top:
                    splatters[n].transform.position = new Vector3(Random.Range(-w, w), h, 1);
                    break;

                case FpsHudSplatterSide.Bottom:
                    splatters[n].transform.position = new Vector3(Random.Range(-w, w), -h, 1);
                    break;

                case FpsHudSplatterSide.Left:
                    splatters[n].transform.position = new Vector3(-w, Random.Range(-h, h), 1);
                    break;

                case FpsHudSplatterSide.Right:
                    splatters[n].transform.position = new Vector3(w, Random.Range(-h, h), 1);
                    break;
            }
        }
    }
}

public enum FpsHudSplatterSide
{
    Top,
    Bottom,
    Left,
    Right
}