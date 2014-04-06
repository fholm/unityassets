using UnityEngine;
using System.Collections;

public class FpsHud : MonoBehaviour
{
    public enum ReticuleStyles
    {
        Crosshair,
        Circle,
        Dot,
        None
    }

    static FpsHud instance = null;

    public static FpsHud Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(FpsHud)) as FpsHud;
            }

            return instance;
        }
    }

    FpsHudAmmo ammo;
    FpsHudSplatter splatter;
    FpsHudScopeCamera scope;

    [SerializeField]
    Color textColor;

    [SerializeField]
    Color iconColor;

    [SerializeField]
    Color blipColor;

    [SerializeField]
    Color crosshairColor = Color.white;

    [SerializeField]
    Camera scopeCamera;

    [SerializeField]
    Camera playerCamera;

    [SerializeField]
    Camera perspectiveCamera;

    [SerializeField]
    Camera orthographicCamera;

    [SerializeField]
    Camera screenCamera;

    [SerializeField]
    FpsHudReticule reticuleCrosshair;

    [SerializeField]
    FpsHudReticule reticuleCircle;

    [SerializeField]
    FpsHudReticule reticuleDot;

    [SerializeField]
    ReticuleStyles reticuleStyle;

    FpsHudAmmo fpsAmmo
    {
        get
        {
            if (ammo == null)
            {
                ammo = FindObjectOfType(typeof(FpsHudAmmo)) as FpsHudAmmo;
            }

            return ammo;
        }
    }

    FpsHudScopeCamera fpsScope
    {
        get
        {
            if (scope == null)
            {
                scope = FindObjectOfType(typeof(FpsHudScopeCamera)) as FpsHudScopeCamera;
            }

            return scope;
        }
    }

    FpsHudSplatter fpsSplatter
    {
        get
        {
            if (splatter == null)
            {
                splatter = FindObjectOfType(typeof(FpsHudSplatter)) as FpsHudSplatter;
            }

            return splatter;
        }
    }


    public Camera ActiveCamera { get; private set; }
    public FpsHudReticule ActiveReticule { get; private set; }
    public Camera ScopeCamera { get { return scopeCamera; } }
    public Camera ScreenCamera { get { return screenCamera; } }
    public Camera PerspectiveCamera { get { return perspectiveCamera; } }
    public Camera OrthographicCamera { get { return orthographicCamera; } }
    public Camera PlayerCamera { get { return playerCamera; } set { playerCamera = value; } }

    public bool HasActiveCamera { get { return ActiveCamera; } }
    public bool HasPlayerCamera { get { return PlayerCamera; } }

    public Color TextColor { get { return textColor; } }
    public Color IconColor { get { return iconColor; } }
    public Color BlipColor { get { return blipColor; } }
    public Color CrosshairColor { get { return crosshairColor; } }

    public ReticuleStyles ReticuleStyle
    {
        get { return reticuleStyle; }
        set
        {
            switch (reticuleStyle = value)
            {
                case FpsHud.ReticuleStyles.Crosshair:
                    reticuleDot.gameObject.SetActiveRecursively(false);
                    reticuleCircle.gameObject.SetActiveRecursively(false);
                    reticuleCrosshair.gameObject.SetActiveRecursively(true);

                    ActiveReticule = reticuleCrosshair;
                    break;

                case FpsHud.ReticuleStyles.Circle:
                    reticuleDot.gameObject.SetActiveRecursively(false);
                    reticuleCircle.gameObject.SetActiveRecursively(true);
                    reticuleCrosshair.gameObject.SetActiveRecursively(false);

                    ActiveReticule = reticuleCircle;
                    break;

                case FpsHud.ReticuleStyles.Dot:
                    reticuleDot.gameObject.SetActiveRecursively(true);
                    reticuleCircle.gameObject.SetActiveRecursively(false);
                    reticuleCrosshair.gameObject.SetActiveRecursively(false);

                    ActiveReticule = reticuleDot;
                    break;

                case FpsHud.ReticuleStyles.None:
                    reticuleDot.gameObject.SetActiveRecursively(false);
                    reticuleCircle.gameObject.SetActiveRecursively(false);
                    reticuleCrosshair.gameObject.SetActiveRecursively(false);

                    ActiveReticule = null;
                    break;
            }
        }
    }

    public float ReticuleSpread
    {
        get
        {
            if (ActiveReticule)
            {
                return ActiveReticule.Spread;
            }

            return 0;
        }
        set
        {
            if (ActiveReticule)
            {
                ActiveReticule.Spread = Mathf.Clamp01(value);
            }
        }
    }

    public int TotalAmmo
    {
        get
        {
            if (fpsAmmo)
            {
                return fpsAmmo.TotalAmmo;
            }

            return 0;
        }
        set
        {
            if (fpsAmmo)
            {
                fpsAmmo.TotalAmmo = value;
            }
        }
    }

    public int ClipAmmo
    {
        get
        {
            if (fpsAmmo)
            {
                return fpsAmmo.ClipAmmo;
            }

            return 0;
        }
        set
        {
            if (fpsAmmo)
            {
                fpsAmmo.ClipAmmo = value;
            }
        }
    }


    public int ClipAmmoMax
    {
        get
        {
            if (fpsAmmo)
            {
                return fpsAmmo.ClipAmmoMax;
            }

            return 0;
        }
        set
        {
            if (fpsAmmo)
            {
                fpsAmmo.ClipAmmoMax = value;
            }
        }
    }

    void Start()
    {
        ActiveCamera = PlayerCamera;
        ReticuleStyle = reticuleStyle;
    }

    public void EnterScope(float fieldOfView)
    {
        if (fpsScope)
        {
            fpsScope.Enter(fieldOfView);
            ActiveCamera = ScopeCamera;
        }
    }

    public void ExitScope()
    {
        if (fpsScope)
        {
            fpsScope.Exit();
            ActiveCamera = PlayerCamera;
        }
    }

    public void SplatterRandom()
    {
        if (fpsSplatter)
        {
            fpsSplatter.Display();
        }
    }

    public void SplatterSide(FpsHudSplatterSide side)
    {
        if (fpsSplatter)
        {
            fpsSplatter.Display(side);
        }
    }

    public void SplatterDirectional(Vector3 origin)
    {
        if (fpsSplatter)
        {
            fpsSplatter.Display(origin);
        }
    }
}