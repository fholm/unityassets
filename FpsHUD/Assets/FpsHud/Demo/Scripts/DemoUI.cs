using UnityEngine;
using System.Collections;

public class DemoUI : MonoBehaviour
{
    GameObject reticule;

    public GameObject Reticule_Crosshair;
    public GameObject Reticule_Dot;
    public GameObject Reticule_Circle;
    public GameObject FPSController;

    class Foo
    {
    }

    class Bar : Foo
    {
    }

    void Start()
    {
        Reticule_Crosshair.SetActiveRecursively(true);
        Reticule_Dot.SetActiveRecursively(false);
        Reticule_Circle.SetActiveRecursively(false);

        reticule = Reticule_Crosshair;
    }

    void Update()
    {
    }

    void OnGUI()
    {
        GUILayout.Label("Demo Controls (not part of UI)");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Crosshair: ");

        if (GUILayout.Button("Standard"))
        {
            Reticule_Crosshair.SetActiveRecursively(true);
            Reticule_Dot.SetActiveRecursively(false);
            Reticule_Circle.SetActiveRecursively(false);
            reticule = Reticule_Crosshair;
        }

        if (GUILayout.Button("Circle"))
        {
            Reticule_Crosshair.SetActiveRecursively(false);
            Reticule_Dot.SetActiveRecursively(false);
            Reticule_Circle.SetActiveRecursively(true);
            reticule = Reticule_Circle;
        }

        if (GUILayout.Button("Dot"))
        {
            Reticule_Crosshair.SetActiveRecursively(false);
            Reticule_Dot.SetActiveRecursively(true);
            Reticule_Circle.SetActiveRecursively(false);
            reticule = Reticule_Dot;
        }
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Spread: ");

        reticule.GetComponent<FpsHudReticule>().Spread = GUILayout.HorizontalSlider(reticule.GetComponent<FpsHudReticule>().Spread, 0, 1);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Splatter: ");

        if (GUILayout.Button("Test"))
        {
            FpsHudSplatter.Instance.Display();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sniper Zoom: ");

        if (GUILayout.Button("Enable"))
        {
            Reticule_Crosshair.SetActiveRecursively(false);
            Reticule_Dot.SetActiveRecursively(false);
            Reticule_Circle.SetActiveRecursively(false);

            FpsHudScopeCamera.Instance.Enter(15f);
        }

        if (GUILayout.Button("Disable"))
        {
            Reticule_Crosshair.SetActiveRecursively(true);
            Reticule_Dot.SetActiveRecursively(false);
            Reticule_Circle.SetActiveRecursively(false);

            FpsHudScopeCamera.Instance.Exit();
        }

        GUILayout.EndHorizontal();
    }
}