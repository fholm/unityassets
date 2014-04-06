using UnityEngine;
using System.Collections;

public class FpsHudRadar : MonoBehaviour
{
    float scanStart = -10f;

    [SerializeField]
    Transform compass;

    [SerializeField]
    Transform compassArrow;

    [SerializeField]
    Transform player;

    [SerializeField]
    Material blipMaterial;

    [SerializeField]
    GameObject blipContainer;

    [SerializeField]
    GameObject blipPrefab;

    [SerializeField]
    int blipMaxCount = 30;

    [SerializeField]
    Transform scan;

    [SerializeField]
    bool scanVisible = true;

    [SerializeField]
    float scanDuration = 2f;

    [SerializeField]
    float scanFrequency = 4f;

    [SerializeField]
    float scanRange = 100f;

    [SerializeField]
    Transform scanOrigin = null;

    [SerializeField]
    LayerMask scanLayers;

    void Start()
    {
        compass = transform.Find("Compass");
        compassArrow = transform.Find("Arrow");
        scan = transform.Find("Scan");
        player = transform.Find("Player");

        // Create blip container
        blipContainer = new GameObject("BlipContainer");
        blipContainer.transform.parent = transform;
        blipContainer.transform.localScale = new Vector3(1, 1, 1);
        blipContainer.transform.localPosition = new Vector3(0, -0.5f, 0);
        blipContainer.transform.localRotation = Quaternion.identity;

        // Set colors
        compass.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.TextColor);
        compassArrow.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.IconColor);
        scan.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.IconColor);
        blipMaterial.SetColor("_Color", FpsHud.Instance.BlipColor);
        renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.TextColor);
        player.renderer.sharedMaterial.SetColor("_Color", FpsHud.Instance.IconColor);
        scan.renderer.sharedMaterial.SetFloat("_Speed", scanDuration);

        // Turn on/off scan
        if (!scanVisible)
        {
            scan.gameObject.active = false;
        }

        // Create blips
        for (int i = 0; i < blipMaxCount; ++i)
        {
            GameObject blip = GameObject.Instantiate(blipPrefab) as GameObject;
            blip.transform.parent = blipContainer.transform;
            blip.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            blip.transform.localPosition = new Vector3(0, 0, 0);
            blip.transform.localRotation = Quaternion.identity;
            blip.renderer.enabled = false;
        }
    }

    void Update()
    {
        float t = Time.time - scanStart;

        if (t >= scanFrequency)
        {
            t = 0f;
            scanStart = Time.time;

            if (scanVisible)
            {
                scan.renderer.material.SetFloat("_Start", scanStart);
            }

            Collider[] hits = Physics.OverlapSphere(scanOrigin.position, scanRange, scanLayers);

            for (int i = 0; i < blipMaxCount; ++i)
            {
                FpsHudRadarBlip blip = blipContainer.transform.GetChild(i).GetComponent<FpsHudRadarBlip>();
                blip.renderer.enabled = false;

                if (i < hits.Length)
                {
                    blip.ScanVisible = hits[i].GetComponent<FpsHudRadarVisible>();

                    if (blip.ScanVisible)
                    {
                        blip.ScanPosition = blip.ScanVisible.transform.position;
                        blip.renderer.enabled = true;
                    }
                }
            }
        }

        if (scanVisible)
        {
            scan.renderer.enabled = t <= scanDuration;

            for (int i = 0; i < blipMaxCount; ++i)
            {
                FpsHudRadarBlip blip = blipContainer.transform.GetChild(i).GetComponent<FpsHudRadarBlip>();

                if (blip.ScanVisible)
                {
                    Vector3 v = blip.ScanPosition - scanOrigin.transform.position;
                    float a = FpsHudUtils.SignedAngle(scanOrigin.transform.forward, v.normalized, Vector3.up);

                    blip.transform.localPosition = new Vector3(0, v.magnitude / scanRange, 0);
                    blip.transform.localRotation = Quaternion.identity;
                    blip.transform.RotateAround(blip.transform.parent.position, blip.transform.parent.forward, -a);
                }
            }
        }

        blipMaterial.SetMatrix("_MaskMatrix", blipContainer.transform.worldToLocalMatrix);
    }
}