using UnityEngine;
using System.Collections;

public class FpsHudAmmo : MonoBehaviour
{
    static FpsHudAmmo instance = null;

    public static FpsHudAmmo Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FpsHudAmmo)) as FpsHudAmmo;
            }

            return instance;
        }
    }

    public int ClipAmmo
    {
        get { return clipAmmo; }
        set
        {
            value = Mathf.Clamp(value, 0, clipAmmoMax);

            if (clipAmmo != value)
            {
                clipAmmo = value;
                UpdateClip();
            }
        }
    }

    public int ClipAmmoMax
    {
        get { return clipAmmoMax; }
        set
        {
            value = Mathf.Clamp(value, 0, 999);

            if (clipAmmoMax != value)
            {
                clipAmmoMax = value;
                UpdateClip();
            }
        }
    }

    public int TotalAmmo
    {
        get { return totalAmmo; }
        set
        {
            value = Mathf.Clamp(value, 0, 999);

            if (totalAmmo != value)
            {
                totalAmmo = value;
                UpdateTotal();
            }
        }
    }

    [SerializeField]
    TextMesh clipBars;

    [SerializeField]
    TextMesh clipCounter;

    [SerializeField]
    TextMesh totalCounter;

    [SerializeField]
    int totalAmmo;

    [SerializeField]
    int clipAmmo;

    [SerializeField]
    int clipAmmoMax;

    [SerializeField]
    string clipBarChar = "I";

    [SerializeField]
    int clipBarCount = 30;

    [SerializeField]
    float clipBarWidth = 1f;

    void Start()
    {
        clipBars = transform.Find("ClipBars").GetComponent<TextMesh>();
        clipCounter = transform.Find("ClipCounter").GetComponent<TextMesh>();
        totalCounter = transform.Find("TotalCounter").GetComponent<TextMesh>();

        // Set colors
        clipBars.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.IconColor);
        clipCounter.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.TextColor);
        totalCounter.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.TextColor);

        // Set values over themselves for init
        ClipAmmo = clipAmmo;
        ClipAmmoMax = clipAmmoMax;
        TotalAmmo = totalAmmo;

        // Run update to set initial values
        UpdateClip();
        UpdateTotal();
    }

    void UpdateTotal()
    {
        totalCounter.text = totalAmmo.ToString();
    }

    void UpdateClip()
    {
        int bars = clipAmmo;

        if (clipAmmoMax > clipBarCount)
        {
            bars = Mathf.RoundToInt(clipBarCount * Mathf.Clamp01((float)clipAmmo / (float)clipAmmoMax));
        }

        float offset = clipBars.transform.localPosition.x;
        float width = bars * clipBarWidth * clipBars.transform.localScale.x;

        clipCounter.text = clipAmmo.ToString();
        clipCounter.transform.localPosition = new Vector3(-width + offset, clipCounter.transform.localPosition.y, 0);

        clipBars.text = new string(clipBarChar[0], bars);
    }
}