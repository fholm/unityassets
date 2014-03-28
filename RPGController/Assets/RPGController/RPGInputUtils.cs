using UnityEngine;

public static class RPGInputUtils
{
    public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
    }

    public static bool GetButtonSafe(string name, bool @default)
    {
        try
        {
            return Input.GetButton(name);
        }
        catch
        {
            Debug.LogError("The button '" + name + "' isn't defined in the input manager");
            return @default;
        }
    }

    public static bool GetButtonDownSafe(string name, bool @default)
    {
        try
        {
            return Input.GetButtonDown(name);
        }
        catch
        {
            Debug.LogError("The button '" + name + "' isn't defined in the input manager");
            return @default;
        }
    }

    public static float GetAxisRawSafe(string name, float @default)
    {
        try
        {
            return Input.GetAxisRaw(name);
        }
        catch
        {
            Debug.LogError("The axis '" + name + "' isn't defined in the input manager");
            return @default;
        }
    }
}