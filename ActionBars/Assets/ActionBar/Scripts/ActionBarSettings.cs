using UnityEngine;
using System.Collections;

public class ActionBarSettings : MonoBehaviour
{
    static ActionBarSettings instance = null;

    public static ActionBarSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(ActionBarSettings)) as ActionBarSettings;
            }

            return instance;
        }
    }

    [SerializeField]
    public Texture2D ButtonTexture;

    [SerializeField]
    public Color ButtonOverlayColor = new Color(118f/255f, 186f/255f, 255f/255f);

    [SerializeField]
    public float ButtonCooldownDarkness = 0.75f;

    [SerializeField]
    public float ButtonPickupTime = 0.25f;

    [SerializeField]
    public float ButtonPickupDistance = 30f;

    [SerializeField]
    public AudioSource ButtonAudioSource;

    [SerializeField]
    public AudioClip ButtonSuccessfullPressSound;

    [SerializeField]
    public AudioClip ButtonDisabledPressSound;

    [SerializeField]
    public AudioClip ButtonCooldownPressSound;

    [SerializeField]
    public AudioClip ButtonCooldownDoneSound;

    [SerializeField]
    public GameObject ButtonCooldownDonePrefab;

    [SerializeField]
    public TextAsset FontDefinition;

    [SerializeField]
    public Material FontMaterial;

    [SerializeField]
    public float LabelFontSize = 0.25f;

    [SerializeField]
    public Vector2 LabelFontPosition = new Vector2(0.1f, -0.7f);

    [SerializeField]
    public float StackFontSize = 0.25f;

    [SerializeField]
    public Vector2 StackFontPosition = new Vector2(0.1f, -0.7f);

    [SerializeField]
    public bool DisplayKeybindings = true;

    [SerializeField]
    public bool DisplayCooldownSeconds = true;

    [SerializeField]
    public bool DisplayStacks = false;

    [SerializeField]
    public bool DisplayOneStacks = false;

    [SerializeField]
    public Texture2D[] AtlasTextures;

    [SerializeField]
    public int AtlasSize = 4;

    [SerializeField]
    public int MaxLabelCharacters = 6;

    [HideInInspector]
    public Material[] AtlasMaterials;

    public int IconsPerAtlas
    {
        get { return AtlasSize * AtlasSize; }
    }

    void Start()
    {
        Init();
    }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (AtlasMaterials.Length != AtlasTextures.Length)
        {
            ButtonPickupDistance = Mathf.RoundToInt(ButtonPickupDistance);
            AtlasMaterials = new Material[AtlasTextures.Length];
            Shader shader = Resources.Load("ActionBarShader", typeof(Shader)) as Shader;

            for (int i = 0; i < AtlasMaterials.Length; ++i)
            {
                AtlasMaterials[i] = new Material(shader);
                AtlasMaterials[i].SetTexture("_Button", ButtonTexture);
                AtlasMaterials[i].SetTexture("_Atlas", AtlasTextures[i]);
                AtlasMaterials[i].SetColor("_OverlayColor", ButtonOverlayColor);
                AtlasMaterials[i].SetFloat("_CooldownColor", Mathf.Clamp01(ButtonCooldownDarkness));
            }
        }
    }

    public Material GetAtlasMaterial(int index)
    {
        Init();

        if (index < AtlasMaterials.Length)
        {
            return AtlasMaterials[index];
        }

        return default(Material);
    }

    public void PlayPressSound()
    {
        if (ButtonAudioSource != null && ButtonSuccessfullPressSound != null)
        {
            ButtonAudioSource.PlayOneShot(ButtonSuccessfullPressSound);
        }
    }

    public void PlayDisabledSound()
    {
        if (ButtonAudioSource != null && ButtonDisabledPressSound != null)
        {
            ButtonAudioSource.PlayOneShot(ButtonDisabledPressSound);
        }
    }

    public void PlayCooldownSound()
    {
        if (ButtonAudioSource != null && ButtonCooldownPressSound != null)
        {
            ButtonAudioSource.PlayOneShot(ButtonCooldownPressSound);
        }
    }

    public void PlayCooldownDoneSound()
    {
        if (ButtonAudioSource != null && ButtonCooldownDoneSound != null)
        {
            ButtonAudioSource.PlayOneShot(ButtonCooldownDoneSound);
        }
    }
}