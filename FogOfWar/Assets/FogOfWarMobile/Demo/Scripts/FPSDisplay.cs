using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    int frameCount = 0;

    float fps = 0;
    float timeLeft = 0f;
    float timePassed = 0f;
    float updateInterval = 0.5f;

    public GameObject fow;

    void Start()
    {
        timeLeft = updateInterval;
    }

    void Update()
    {
        frameCount += 1;
        timeLeft -= Time.deltaTime;
        timePassed += Time.timeScale / Time.deltaTime;

        if (timeLeft <= 0f)
        {
            fps = timePassed / frameCount;
            timeLeft = updateInterval;
            timePassed = 0f;
            frameCount = 0;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 25), fps.ToString());
        
        if (fow != null && GUI.Button(new Rect(Screen.width - 110, 10, 100, 60), "Toggle"))
        {
            fow.SetActiveRecursively(!fow.active);
        }
    }
}