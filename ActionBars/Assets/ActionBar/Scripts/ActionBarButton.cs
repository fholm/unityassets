using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ActionBarButton : MonoBehaviour
{
    Mesh mesh;
    Color[] colors = new Color[4] { Color.black, Color.black, Color.black, Color.black };
    Vector3[] normals = new Vector3[4] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
    Vector2[] uv1 = new Vector2[4] { new Vector2(5, 5), new Vector2(5, 5), new Vector2(5, 5), new Vector2(5, 5) };
    ActionBarDescriptor descriptor;
    BattleTextRenderer label;
    BattleTextRenderer stack;
    bool onCooldown = false;
    ActionBarRow row;

    public string Label
    {
        get { return label ? "" : label.Sentance; }
        set
        {
            InitLabel();
            label.SetText(value ?? "");
        }
    }

    public int Stack
    {
        get { return stack ? 0 : System.Int32.Parse(stack.Sentance); }
        set
        {
            InitStack();

            if (value == 1 && !ActionBarSettings.Instance.DisplayOneStacks)
            {
                stack.SetText("");
            }
            else
            {
                stack.SetText(value.ToString());
            }
        }
    }

    public bool Pressed
    {
        get;
        set;
    }

    public bool Overlay
    {
        get { return colors[0].r == 1f; }
        set
        {
            float v = value ? 1f : 0f;

            if (colors[0].r != v)
            {
                for (int i = 0; i < 4; ++i)
                {
                    colors[i].r = v;
                }

                UpdateShaderData();
            }
        }
    }

    public Color[] Colors
    {
        get { return colors; }
    }

    public ActionBarDescriptor Descriptor
    {
        get { return descriptor; }
    }

    public ActionBarRow Row
    {
        get { return row; }
    }

    public bool Empty
    {
        get { return descriptor == null; }
    }

    public bool CloneOnPickup
    {
        get
        {
            return row.CloneOnPickup;
        }
    }

    public int ItemGroup
    {
        get
        {
            return row.ItemGroup;
        }
    }

    public bool Locked
    {
        get
        {
            return row.IsLocked;
        }
    }

    void Update()
    {
        if (descriptor != null)
        {
            if (onCooldown != descriptor.OnCooldown)
            {
                if (onCooldown)
                {
                    GameObject doneInstance = GameObject.Instantiate(ActionBarSettings.Instance.ButtonCooldownDonePrefab) as GameObject;

                    doneInstance.layer = gameObject.layer;
                    doneInstance.transform.parent = transform;
                    doneInstance.transform.localPosition = new Vector3(0.5f, -0.5f, -1f);

                    ActionBarSettings.Instance.PlayCooldownDoneSound();
                }

                onCooldown = descriptor.OnCooldown;
            }
        }
    }

    void UpdateShaderData()
    {
        if (Application.isPlaying)
        {
            mesh.colors = colors;
            mesh.normals = normals;
            mesh.uv1 = uv1;
        }
    }

    void SetIcon(int atlas, int icon)
    {
        if (icon < ActionBarSettings.Instance.IconsPerAtlas && atlas < ActionBarSettings.Instance.AtlasMaterials.Length)
        {
            renderer.sharedMaterial = ActionBarSettings.Instance.GetAtlasMaterial(atlas);

            // Only do this if we actually get a material
            if (renderer.sharedMaterial != null)
            {
                int atlasSize = ActionBarSettings.Instance.AtlasSize;

                for (int i = 0; i < 4; ++i)
                {
                    colors[i].r = 0f;
                    colors[i].b = 1f;
                    colors[i].g = (1f / atlasSize) * (icon % atlasSize);
                    colors[i].a = (1f / atlasSize) * (icon / atlasSize);
                }

                UpdateShaderData();
            }
        }
    }

    public void SetGrayscale(float value)
    {
        value = Mathf.Clamp01(value);

        if (normals[0].x != value)
        {
            for (int i = 0; i < 4; ++i)
            {
                normals[i].x = value;
            }

            UpdateShaderData();
        }
    }

    public void SetCooldown(float start, float duration)
    {
        if (uv1[0].x != start || uv1[0].y != duration)
        {
            for (int i = 0; i < 4; ++i)
            {
                uv1[i].x = start;
                uv1[i].y = duration;
            }

            UpdateShaderData();
        }
    }

    public ActionBarDescriptor SetDescriptor(ActionBarDescriptor desc)
    {
        if (desc == null)
        {
            return null;
        }

        ActionBarDescriptor temp = descriptor;

        descriptor = desc;
        descriptor.Buttons.Add(this);

        onCooldown = desc.OnCooldown;
        SetIcon(desc.Atlas, desc.Icon);
        SetCooldown(desc.CooldownStart, desc.Cooldown);
        SetGrayscale(descriptor.Disabled ? 1f : 0f);

        if (desc.Stackable)
        {
            InitStack();
            Stack = desc.Stack;
            stack.gameObject.active = true;
        }
        else
        {
            if (stack != null)
            {
                stack.gameObject.active = false;
            }
        }

        if (temp != null)
        {
            temp.Buttons.Remove(this);
        }

        return temp;
    }

    public ActionBarDescriptor RemoveDescriptor()
    {
        for (int i = 0; i < 4; ++i)
        {
            colors[i].b = 0f;
        }

        UpdateShaderData();

        if (stack != null)
        {
            stack.gameObject.active = false;
        }

        ActionBarDescriptor tmp = descriptor;
        descriptor = null;
        tmp.Buttons.Remove(this);
        return tmp;
    }

    public void Init(ActionBarRow row)
    {
        if (mesh == null)
        {
            this.row = row;
            gameObject.layer = row.Layer;

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(0.5f, -0.5f, 0f);
            collider.size = new Vector3(1f, 1f, 0.1f);

            renderer.castShadows = false;
            renderer.receiveShadows = false;
            renderer.useLightProbes = false;
            renderer.sharedMaterial = ActionBarSettings.Instance.GetAtlasMaterial(0);

            mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = row.Quad.vertices;
            mesh.triangles = row.Quad.triangles;
            mesh.uv = row.Quad.uv;
            mesh.uv1 = uv1;
            mesh.colors = colors;
            mesh.normals = normals;
        }
    }

    public void Press()
    {
        descriptor.Invoke();
    }

    void InitLabel()
    {
        if (!label)
        {
            GameObject labelGo = new GameObject("TextLabel");
            labelGo.layer = row.Layer;
            labelGo.transform.parent = transform;
            labelGo.transform.localPosition = row.LabelFontPosition + new Vector3(0, 0, -0.1f);

            label = labelGo.AddComponent<BattleTextRenderer>();
            label.GlyphSize = row.LabelFontSize;
            label.Init();
        }
    }

    void InitStack()
    {
        if (!stack)
        {
            GameObject stackGo = new GameObject("TextLabel");
            stackGo.layer = row.Layer;
            stackGo.transform.parent = transform;
            stackGo.transform.localPosition = row.StackFontPosition + new Vector3(0, 0, -0.1f);

            stack = stackGo.AddComponent<BattleTextRenderer>();
            stack.GlyphSize = row.StackFontSize;
            stack.Init();
        }
    }

    void InitCooldown()
    {

    }
}