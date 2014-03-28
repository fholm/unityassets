using UnityEngine;
using System.Collections;

public class FpsHudClip : MonoBehaviour
{
    float previousAvailable;
    GameObject[] markers;

    public int Markers = 30;
    public GameObject MarkerPrefab;
    public int MaxBullets = 30;
    public int CurrentBullets = 30;

    void Start()
    {
        markers = new GameObject[Markers];

        for (int i = 0; i < Markers; ++i)
        {
            markers[i] = GameObject.Instantiate(MarkerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            markers[i].transform.parent = transform;
            markers[i].transform.localPosition = new Vector3(-(i * 4) + 0.5f, 0.5f, 0);
        }
    }

    void Update()
    {
        float available = Mathf.Clamp01(((float)CurrentBullets) / ((float)MaxBullets));

        if (available != previousAvailable)
        {
            int m = (int)(Markers * available);

            for (int i = 0; i < Markers; ++i)
            {
                if (i + 1 > m)
                {
                    markers[i].renderer.enabled = false;
                }
                else
                {
                    markers[i].renderer.enabled = true;
                }
            }

            previousAvailable = available;
        }
    }
}