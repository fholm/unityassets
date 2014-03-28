using System;
using System.Collections.Generic;
using UnityEngine;

public class RPGTriangleTree
{
    public struct Triangle
    {
        public int Index0;
        public int Index1;
        public int Index2;

        public Vector3 Center;
        public Vector3 Extents;

        public Vector3 Min { get { return Center - Extents; } }
        public Vector3 Max { get { return Center + Extents; } }
    }

    public struct Node
    {
        public const int LEFT_TOP_FRONT = 0;
        public const int RIGHT_TOP_FRONT = 1;
        public const int LEFT_TOP_BACK = 2;
        public const int RIGHT_TOP_BACK = 3;
        public const int LEFT_BOTTOM_FRONT = 4;
        public const int RIGHT_BOTTOM_FRONT = 5;
        public const int LEFT_BOTTOM_BACK = 6;
        public const int RIGHT_BOTTOM_BACK = 7;

        public Vector3 Center;
        public Vector3 Extents;
        public Node[] Children;
        public int[] Triangles;

        public Vector3 Min { get { return Center - Extents; } }
        public Vector3 Max { get { return Center + Extents; } }

        public void Init(Vector3 center, Vector3 extents)
        {
            Center = center;
            Extents = extents;
            Triangles = new int[0];
        }

        public static void Split(ref Node node)
        {
            var x2 = node.Extents.x / 2;
            var y2 = node.Extents.y / 2;
            var z2 = node.Extents.z / 2;

            var extents = new Vector3(x2, y2, z2);
            var right = node.Center.x + x2;
            var left = node.Center.x - x2;
            var top = node.Center.y + y2;
            var bottom = node.Center.y - y2;
            var front = node.Center.z + z2;
            var back = node.Center.z - z2;

            node.Children = new Node[8];
            node.Children[LEFT_TOP_FRONT].Init(new Vector3(left, top, front), extents);
            node.Children[RIGHT_TOP_FRONT].Init(new Vector3(right, top, front), extents);
            node.Children[LEFT_TOP_BACK].Init(new Vector3(left, top, back), extents);
            node.Children[RIGHT_TOP_BACK].Init(new Vector3(right, top, back), extents);
            node.Children[LEFT_BOTTOM_FRONT].Init(new Vector3(left, bottom, front), extents);
            node.Children[RIGHT_BOTTOM_FRONT].Init(new Vector3(right, bottom, front), extents);
            node.Children[LEFT_BOTTOM_BACK].Init(new Vector3(left, bottom, back), extents);
            node.Children[RIGHT_BOTTOM_BACK].Init(new Vector3(right, bottom, back), extents);
        }

        public static void Insert(ref Node n, Triangle[] ts, int t)
        {
            if (n.Extents.x / 2f > 0.5f && n.Children == null)
            {
                Node.Split(ref n);
            }

            if (Node.IntersectsTriangle(ref n, ref ts[t]))
            {
                if (n.Children == null)
                {
                    Array.Resize(ref n.Triangles, n.Triangles.Length + 1);
                    n.Triangles[n.Triangles.Length - 1] = t;
                }
                else
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        Insert(ref n.Children[i], ts, t);
                    }
                }
            }
        }

        public static void FindClosestNodes(ref Node n, ref Vector3 p, float r, List<Node> result)
        {
            if (Node.IntersectsSphere(ref n, ref p, r))
            {
                if (n.Triangles.Length > 0)
                {
                    result.Add(n);
                }

                if (n.Children != null)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        FindClosestNodes(ref n.Children[i], ref p, r, result);
                    }
                }
            }
        }

        public static void FindClosestTriangles(ref Node n, ref Vector3 p, float r, HashSet<int> result)
        {
            if (Node.IntersectsSphere(ref n, ref p, r))
            {
                if (n.Triangles.Length > 0)
                {
                    result.UnionWith(n.Triangles);
                }

                if (n.Children != null)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        FindClosestTriangles(ref n.Children[i], ref p, r, result);
                    }
                }
            }
        }

        public static bool IntersectsTriangle(ref Node n, ref Triangle t)
        {
            Vector3 nMin = n.Min;
            Vector3 nMax = n.Max;

            Vector3 tMin = t.Min;
            Vector3 tMax = t.Max;

            if (nMin.x > tMax.x || tMin.x > nMax.x)
                return false;

            if (nMin.y > tMax.y || tMin.y > nMax.y)
                return false;

            if (nMin.z > tMax.z || tMin.z > nMax.z)
                return false;

            return true;
        }

        public static bool IntersectsSphere(ref Node node, ref Vector3 p, float radius)
        {
            Vector3 v = clampVector(p, node.Min, node.Max);
            return (p - v).sqrMagnitude <= radius * radius;
        }

        static Vector3 clampVector(Vector3 value, Vector3 min, Vector3 max)
        {
            float x = value.x;
            x = (x > max.x) ? max.x : x;
            x = (x < min.x) ? min.x : x;

            float y = value.y;
            y = (y > max.y) ? max.y : y;
            y = (y < min.y) ? min.y : y;

            float z = value.z;
            z = (z > max.z) ? max.z : z;
            z = (z < min.z) ? min.z : z;

            return new Vector3(x, y, z);
        }
    }

    public Node Root = new Node();
    public readonly int TriangleCount = 0;
    public readonly int VertexCount = 0;
    public readonly Vector3[] Vertices;
    public readonly Triangle[] Triangles;
    public readonly float Size = float.MinValue;

    public RPGTriangleTree(MeshCollider mc)
    {
        Mesh mesh = mc.sharedMesh;
        int[] tris = mesh.triangles;
        Vector3[] verts = mesh.vertices;

        Vertices = verts;
        VertexCount = verts.Length;
        TriangleCount = tris.Length / 3;
        Triangles = new Triangle[TriangleCount];

        Vector3 size = mc.bounds.extents * 2f;
        Size = Mathf.Max(Size, Mathf.Ceil(size.x));
        Size = Mathf.Max(Size, Mathf.Ceil(size.y));
        Size = Mathf.Max(Size, Mathf.Ceil(size.z));

        Root.Init(Vector3.zero, new Vector3(Size, Size, Size));

        Triangle t = new Triangle();
        Vector3[] pts = new Vector3[3];

        int n = 0;

        for (int i = 0; i < tris.Length; ++i)
        {
            t = new Triangle();
            t.Index0 = tris[i];
            t.Index1 = tris[++i];
            t.Index2 = tris[++i];

            pts[0] = verts[t.Index0];
            pts[1] = verts[t.Index1];
            pts[2] = verts[t.Index2];

            FromPoints(pts, out t.Center, out t.Extents);

            Triangles[n++] = t;

            Node.Insert(ref Root, Triangles, n - 1);
        }
    }

    public void GetTrianglePoints(int n, out Vector3 p0, out Vector3 p1, out Vector3 p2)
    {
        Triangle t = Triangles[n];
        p0 = Vertices[t.Index0];
        p1 = Vertices[t.Index1];
        p2 = Vertices[t.Index2];
    }

    public Vector3[] GetTrianglePoints(int n)
    {
        Vector3[] ps = new Vector3[3];
        GetTrianglePoints(n, out ps[0], out ps[1], out ps[2]);
        return ps;
    }

    public static void FromPoints(Vector3[] points, out Vector3 center, out Vector3 extents)
    {
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        for (int i = 0; i < points.Length; ++i)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        center = 0.5f * (min + max);
        extents = 0.5f * (max - min);
    }

    public void DrawGizmos()
    {
        DrawGizmos(ref Root);
    }

    public List<Node> FindClosestNodes(Vector3 p, float r)
    {
        List<Node> result = new List<Node>();
        Node.FindClosestNodes(ref Root, ref p, r, result);
        return result;
    }

    public HashSet<int> FindClosestTriangles(Vector3 p, float r)
    {
        HashSet<int> result = new HashSet<int>();
        Node.FindClosestTriangles(ref Root, ref p, r, result);
        return result;
    }

    void DrawGizmos(ref Node node)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(node.Center, node.Extents * 2f);

        for (int i = 0; i < node.Triangles.Length; ++i)
        {
            Triangle t = Triangles[node.Triangles[i]];

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vertices[t.Index0], Vertices[t.Index1]);
            Gizmos.DrawLine(Vertices[t.Index1], Vertices[t.Index2]);
            Gizmos.DrawLine(Vertices[t.Index2], Vertices[t.Index0]);
        }

        if (node.Children != null)
        {
            for (int i = 0; i < 8; ++i)
            {
                DrawGizmos(ref node.Children[i]);
            }
        }
    }
}
