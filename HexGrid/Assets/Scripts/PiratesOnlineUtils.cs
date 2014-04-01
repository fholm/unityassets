using UnityEngine;
using System.Collections;

public static class PiratesOnlineUtils
{
    public static void CalculateScreenCornerRay(Camera camera, float x, float y, out Vector3 origin, out Vector3 direction)
    {
        origin = camera.ViewportToWorldPoint(new Vector3(x, y, 0));
        direction = camera.ViewportToWorldPoint(new Vector3(x, y, 1));
        direction = (direction - origin).normalized;
    }

    public static void DrawScreenCornerRay(Camera camera, float x, float y, Color color)
    {
        Vector3 origin, direction;

        CalculateScreenCornerRay(camera, x, y, out origin, out direction);

        Vector3 v = origin + (direction * 50f);

        Debug.DrawLine(origin, v, color);
    }

    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
