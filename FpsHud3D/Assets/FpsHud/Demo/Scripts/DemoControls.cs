using UnityEngine;
using System.Collections;

public class DemoControls : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FpsHud.Instance.SplatterRandom();
        }

        if (!FpsHudScopeCamera.Instance.Toggled)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                FpsHud.Instance.ReticuleStyle = FpsHud.ReticuleStyles.Crosshair;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FpsHud.Instance.ReticuleStyle = FpsHud.ReticuleStyles.Circle;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                FpsHud.Instance.ReticuleStyle = FpsHud.ReticuleStyles.Dot;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (FpsHudScopeCamera.Instance.Toggled)
            {
                FpsHud.Instance.ActiveReticule.gameObject.SetActiveRecursively(true);
                FpsHudScopeCamera.Instance.Exit();
            }
            else
            {
                FpsHud.Instance.ActiveReticule.gameObject.SetActiveRecursively(false);
                FpsHudScopeCamera.Instance.Enter(20f);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            FpsHudPerspectiveCamera cam = FindObjectOfType(typeof(FpsHudPerspectiveCamera)) as FpsHudPerspectiveCamera;
            BloomAndLensFlares bloom = cam.GetComponent<BloomAndLensFlares>();
            bloom.enabled = !bloom.enabled;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (FpsHud.Instance.TotalAmmo > 0)
            {
                int newAmmo = Mathf.Min(FpsHud.Instance.TotalAmmo, FpsHud.Instance.ClipAmmoMax);

                FpsHud.Instance.ClipAmmo = newAmmo;
                FpsHud.Instance.TotalAmmo -= newAmmo;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (FpsHud.Instance.ClipAmmo > 0)
            {
                FpsHud.Instance.ClipAmmo -= 1;
            }
        }

        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyUp(KeyCode.Return))
        {
            if (!Screen.fullScreen)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
            else
            {
                Screen.SetResolution(960, 600, false);
            }
        }

        FpsHud.Instance.ReticuleSpread += Input.GetAxis("Mouse ScrollWheel");
    }
}