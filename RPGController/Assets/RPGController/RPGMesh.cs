using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
public class RPGMesh : MonoBehaviour
{
    static Dictionary<int, RPGTriangleTree> triangleTrees = new Dictionary<int, RPGTriangleTree>();

    int meshId;
    Mesh sharedMesh;
    Color color = new Color(153f/256f, 217f/256f, 245f/255f);

    void Start()
    {
        sharedMesh = GetComponent<MeshFilter>().sharedMesh;

        if (sharedMesh == null)
        {
            Debug.Log("[RPGMesh] No MeshFilter.sharedMesh object found");
            enabled = false;
            return;
        }

        meshId = sharedMesh.GetInstanceID();

        if (!triangleTrees.ContainsKey(meshId))
        {
            triangleTrees[meshId] = new RPGTriangleTree(GetComponent<MeshCollider>());
        }
    }

    public Vector3 ClosestPointOn(Vector3 to, float bodyRadius, bool displayDebugInfo, bool displayExtendedDebugInfo)
    {
        RPGTriangleTree tt;

        if (triangleTrees.TryGetValue(meshId, out tt))
        {
            Vector3
                p = Vector3.zero,
                p0 = Vector3.zero,
                p1 = Vector3.zero,
                p2 = Vector3.zero,
                tp0 = Vector3.zero,
                tp1 = Vector3.zero,
                tp2 = Vector3.zero,
                closestPoint = Vector3.zero;

            float magnitude = float.MaxValue;
            Vector3 localOrigin = transform.InverseTransformPoint(to);
            HashSet<int> triangles = tt.FindClosestTriangles(localOrigin, bodyRadius);

            foreach (int index in triangles)
            {
                tt.GetTrianglePoints(index, out p0, out p1, out p2);

                if (displayDebugInfo)
                {
                    Debug.DrawLine(transform.TransformPoint(p0), transform.TransformPoint(p1), color);
                    Debug.DrawLine(transform.TransformPoint(p1), transform.TransformPoint(p2), color);
                    Debug.DrawLine(transform.TransformPoint(p2), transform.TransformPoint(p0), color);
                }

                ClosestPointOnTriangleToPoint(ref p0, ref p1, ref p2, ref localOrigin, out closestPoint);
                closestPoint = transform.TransformPoint(closestPoint);

                if ((closestPoint - to).magnitude < magnitude)
                {
                    p = closestPoint;
                    tp0 = transform.TransformPoint(p0);
                    tp1 = transform.TransformPoint(p1);
                    tp2 = transform.TransformPoint(p2);
                    magnitude = (closestPoint - to).magnitude;
                }
            }

            if (triangles.Count > 0)
            {
                Plane trianglePlane = new Plane(tp0, tp1, tp2);
                Vector3 projectedPoint = to - trianglePlane.GetDistanceToPoint(to) * trianglePlane.normal;

                if (PointLiesInTriangle(projectedPoint, p0, p1, p2))
                {
                    p = projectedPoint;
                }

                if (displayDebugInfo)
                {
                    Debug.DrawLine(tp0, tp1);
                    Debug.DrawLine(tp1, tp2);
                    Debug.DrawLine(tp2, tp0);
                }

                return p;
            }
        }

        return Vector3.zero;
    }

    bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
    {
        var cr1 = Vector3.Cross(b - a, p1 - a);
        var cr2 = Vector3.Cross(b - a, p2 - a);
        return Vector3.Dot(cr1, cr2) >= 0f;
    }

    bool PointLiesInTriangle(Vector3 pt, Vector3 a, Vector3 b, Vector3 c)
    {
        return
            SameSide(pt, a, b, c) &&
            SameSide(pt, b, a, c) &&
            SameSide(pt, c, a, b);
    }

    /// <summary>
    /// Determines the closest point between a point and a triangle.
    /// 
    /// The code in this method is copyrighted by the SlimDX Group under the MIT license:
    /// 
    /// Copyright (c) 2007-2010 SlimDX Group
    /// 
    /// Permission is hereby granted, free of charge, to any person obtaining a copy
    /// of this software and associated documentation files (the "Software"), to deal
    /// in the Software without restriction, including without limitation the rights
    /// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    /// copies of the Software, and to permit persons to whom the Software is
    /// furnished to do so, subject to the following conditions:
    /// 
    /// The above copyright notice and this permission notice shall be included in
    /// all copies or substantial portions of the Software.
    /// 
    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    /// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    /// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    /// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    /// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    /// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    /// THE SOFTWARE.
    /// 
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <param name="vertex1">The first vertex to test.</param>
    /// <param name="vertex2">The second vertex to test.</param>
    /// <param name="vertex3">The third vertex to test.</param>
    /// <param name="result">When the method completes, contains the closest point between the two objects.</param>
    void ClosestPointOnTriangleToPoint(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, ref Vector3 point, out Vector3 result)
    {
        //Source: Real-Time Collision Detection by Christer Ericson
        //Reference: Page 136

        //Check if P in vertex region outside A
        Vector3 ab = vertex2 - vertex1;
        Vector3 ac = vertex3 - vertex1;
        Vector3 ap = point - vertex1;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0.0f && d2 <= 0.0f)
        {
            result = vertex1; //Barycentric coordinates (1,0,0)
            return;
        }

        //Check if P in vertex region outside B
        Vector3 bp = point - vertex2;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0.0f && d4 <= d3)
        {
            result = vertex2; // barycentric coordinates (0,1,0)
            return;
        }

        //Check if P in edge region of AB, if so return projection of P onto AB
        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
        {
            float v = d1 / (d1 - d3);
            result = vertex1 + v * ab; //Barycentric coordinates (1-v,v,0)
            return;
        }

        //Check if P in vertex region outside C
        Vector3 cp = point - vertex3;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0.0f && d5 <= d6)
        {
            result = vertex3; //Barycentric coordinates (0,0,1)
            return;
        }

        //Check if P in edge region of AC, if so return projection of P onto AC
        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
        {
            float w = d2 / (d2 - d6);
            result = vertex1 + w * ac; //Barycentric coordinates (1-w,0,w)
            return;
        }

        //Check if P in edge region of BC, if so return projection of P onto BC
        float va = d3 * d6 - d5 * d4;
        if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            result = vertex2 + w * (vertex3 - vertex2); //Barycentric coordinates (0,1-w,w)
            return;
        }

        //P inside face region. Compute Q through its barycentric coordinates (u,v,w)
        float denom = 1.0f / (va + vb + vc);
        float v2 = vb * denom;
        float w2 = vc * denom;
        result = vertex1 + ab * v2 + ac * w2; //= u*vertex1 + v*vertex2 + w*vertex3, u = va * denom = 1.0f - v - w
    }
}