using UnityEngine;
using System.Collections;

public static class FpsHudExtensions
{
    public static void GetViewAngles(this Camera cam, out float horizontal, out float vertical)
    {
        Vector3
            leftDir, rightDir, topDir, bottomDir,
            leftOrg, rightOrg, topOrg, bottomOrg;

        rightOrg = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0.0f));
        rightDir = (cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 1.0f)) - rightOrg).normalized;

        leftOrg = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0.0f));
        leftDir = (cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 1.0f)) - leftOrg).normalized;

        topOrg = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0.0f));
        topDir = (cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, 1.0f)) - topOrg).normalized;

        bottomOrg = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 0.0f));
        bottomDir = (cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 1.0f)) - bottomOrg).normalized;

        horizontal = Vector3.Angle(leftDir, rightDir);
        vertical = Vector3.Angle(bottomDir, topDir);
    }
}