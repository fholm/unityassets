using UnityEngine;
using System.Collections;

public class DistanceFieldGenerator : MonoBehaviour
{
    [SerializeField]
    Texture2D source;

    void Start()
    {
        int searchDistance = 16;
        Texture2D target = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);

        for (int x = 0; x < source.width; ++x)
        {
            for(int y = 0; y < source.height; ++y) 
            {
                float a = source.GetPixel(x, y).r;
                float distance = float.MaxValue;

                int fxMin = Mathf.Max(x - searchDistance, 0);
                int fxMax = Mathf.Min(x + searchDistance, source.width);
                int fyMin = Mathf.Max(y - searchDistance, 0);
                int fyMax = Mathf.Min(y + searchDistance, source.height);

                for (int fx = fxMin; fx < fxMax; ++fx)
                {
                    for (int fy = fyMin; fy < fyMax; ++fy)
                    {
                        float p = source.GetPixel(fx, fy).r;

                        if (a != p)
                        {
                            float xd = x - fx;
                            float yd = y - fy;
                            float d = Mathf.Sqrt((xd * xd) + (yd * yd));

                            if (d < 0)
                            {
                                Debug.Log(d);
                            }

                            if (Mathf.Abs(d) < Mathf.Abs(distance))
                            {
                                distance = d;
                            }
                        }
                    }
                }

                if (distance != float.MaxValue)
                {
                    distance = Mathf.Clamp(distance, -searchDistance, +searchDistance);

                    if (a == 1)
                    {
                        a = Mathf.Clamp01((distance + searchDistance) / (searchDistance + searchDistance));
                    }
                    else
                    {
                        a = 1f - Mathf.Clamp01((distance + searchDistance) / (searchDistance + searchDistance));
                    }
                }

                target.SetPixel(x, y, new Color(a, a, a, 1));
            }
        }

        System.IO.File.WriteAllBytes("distancefield.png", target.EncodeToPNG());
    }

    // Update is called once per frame
    void Update()
    {

    }
}