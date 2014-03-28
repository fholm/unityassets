using UnityEngine;
using System.Collections;

public class RTSCameraPan : MonoBehaviour
{
    FogOfWar fow;

    void Start()
    {
        fow = FindObjectOfType(typeof(FogOfWar)) as FogOfWar;
    }

    void OnGUI()
    {
        if (GUI.RepeatButton(new Rect(10, Screen.height - 70, 100, 60), "Left"))
        {
            fow.ClearAll = true;
            transform.position += Vector3.left * 0.25f;
        }

        if (GUI.RepeatButton(new Rect(Screen.width - 110, Screen.height - 70, 100, 60), "Right"))
        {
            fow.ClearAll = true;
            transform.position += Vector3.right * 0.25f;
        }
    }
}
