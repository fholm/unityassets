using UnityEngine;

public static class RPGCollisions
{
    public static Vector3 ClosestPointOn(BoxCollider collider, Vector3 to)
    {
        if (collider.transform.rotation == Quaternion.identity)
        {
            return collider.ClosestPointOnBounds(to);
        }

        return closestPointOnOBB(collider, to);
    }

    public static Vector3 ClosestPointOn(SphereCollider collider, Vector3 to)
    {
        Vector3 p;

        p = to - collider.transform.position;
        p.Normalize();

        p *= collider.radius * collider.transform.localScale.x;
        p += collider.transform.position;

        return p;
    }

    static Vector3 closestPointOnOBB(BoxCollider collider, Vector3 to)
    {
        // Cache the collider transform
        var ct = collider.transform;

        // Firstly, transform the point into the space of the collider
        var local = ct.worldToLocalMatrix.MultiplyPoint3x4(to);

        // Now, shift it to be in the center of the box
        local -= collider.center;

        // Inverse scale it by the colliders scale
        var localNorm =
            new Vector3(
                Mathf.Clamp(local.x, -collider.extents.x, collider.extents.x),
                Mathf.Clamp(local.y, -collider.extents.y, collider.extents.y),
                Mathf.Clamp(local.z, -collider.extents.z, collider.extents.z)
            );

        // Now we undo our transformations
        localNorm += collider.center;

        // Return resulting point
        return ct.localToWorldMatrix.MultiplyPoint3x4(localNorm);
    }
}