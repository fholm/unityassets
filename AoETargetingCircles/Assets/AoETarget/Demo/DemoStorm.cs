using UnityEngine;
using System.Collections;

public class DemoStorm : MonoBehaviour
{
    float lastCast = 0f;

    void CastSpell(Vector3 point)
    {
        if (Time.time - lastCast > 5f || lastCast == 0f)
        {
            point.y += 10f;

            GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Storm"), point, Quaternion.identity);
            GameObject.Destroy(go, 10f);

            lastCast = Time.time;
        }
    }
}