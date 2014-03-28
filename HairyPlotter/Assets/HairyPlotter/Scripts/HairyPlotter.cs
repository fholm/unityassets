/**
 * Copyright (C) 2012 Fredrik Holmstrom
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do 
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 **/

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

public class HairyPlotter : MonoBehaviour
{
    List<HairyPlotterVertex> vertices;
    List<HairyPlotterTriangle> triangles;
    Vector3 selectionPosition = Vector3.zero;
    HairyPlotterTriangle triangleHover = null;
    HashSet<HairyPlotterTriangle> triangleSelection = new HashSet<HairyPlotterTriangle>();
    HashSet<HairyPlotterVertex> verticeSelection = new HashSet<HairyPlotterVertex>();
    HairyPlotterVertex uvEditVertex = null;

    public bool Dirty;
    public Mesh EditMesh;
    public string OriginalMesh;
    public HairyPlotterActions CurrentAction = HairyPlotterActions.None;

    public int VertexCount { get { return vertices == null ? 0 : vertices.Count; } }
    public int TriangleCount { get { return triangles == null ? 0 : triangles.Count; } }
    public int VertexSelectionCount { get { return verticeSelection.Count; } }
    public int TriangleSelectionCount { get { return triangleSelection.Count; } }
    public List<HairyPlotterVertex> SelectedVertices { get { return verticeSelection.ToList(); } }
    public List<HairyPlotterTriangle> SelectedTriangles { get { return triangleSelection.ToList(); } }
    public Vector3 SelectionPosition { get { return selectionPosition; } set { selectionPosition = value; } }
    public HairyPlotterVertex LastVertex { get { return VertexCount > 0 ? vertices[VertexCount - 1] : null; } }
    public HairyPlotterTriangle LastTriangle { get { return TriangleCount > 0 ? triangles[TriangleCount - 1] : null; } }
    public HairyPlotterTriangle HoveredTriangle { get { return triangleHover; } }
    public HairyPlotterVertex UvEditVertex { get { return uvEditVertex; } }
    public IEnumerable<HairyPlotterVertex> UnusedVertices { get { return vertices.Where(x => x.TriangleCount == 0).ToArray(); } }
    public int UnusedVerticesCount { get { return vertices == null ? 0 : vertices.Where(x => x.TriangleCount == 0).Count(); } }

    public void ToggleSelected(HairyPlotterVertex vertex)
    {
        if (vertex == null)
            return;

        if (IsSelected(vertex))
        {
            RemoveSelection(vertex);
        }
        else
        {
            AddSelection(vertex);
        }

        ResetSelectionPosition();
    }

    public bool ToggleSelected(HairyPlotterTriangle triangle)
    {
        if (triangle == null)
            return false;

        if (IsSelected(triangle))
        {
            RemoveSelection(triangle);
        }
        else
        {
            AddSelection(triangle);
        }

        return true;
    }

    public HairyPlotterVertex GetVertex(int index)
    {
        return index < vertices.Count ? vertices[index] : null;
    }

    public bool IsSelected(HairyPlotterVertex vertex)
    {
        return vertex != null && verticeSelection.Contains(vertex);
    }

    public bool IsSelected(int index)
    {
        return index < vertices.Count && IsSelected(vertices[index]);
    }

    public bool IsSelected(HairyPlotterTriangle triangle)
    {
        return triangle != null && triangleSelection.Contains(triangle);
    }

    public void AddSelection(HairyPlotterVertex vertex)
    {
        if (vertex == null)
            return;

        verticeSelection.Add(vertex);
        ResetSelectionPosition();
    }

    public void AddSelection(HairyPlotterTriangle triangle)
    {
        if (triangle == null)
            return;

        if (triangleHover == triangle)
        {
            triangleHover = null;
        }

        triangleSelection.Add(triangle);
    }

    public void AddSelection(int index)
    {
        if (index < vertices.Count)
            AddSelection(vertices[index]);
    }

    public void RemoveSelection(HairyPlotterVertex vertex)
    {
        if (uvEditVertex == vertex)
            uvEditVertex = null;

        verticeSelection.Remove(vertex);
        ResetSelectionPosition();
    }

    public void RemoveSelection(HairyPlotterTriangle triangle)
    {
        if (triangle == null)
            return;

        triangleSelection.Remove(triangle);
    }

    public void RemoveSelection(int index)
    {
        if (index < vertices.Count)
            RemoveSelection(vertices[index]);
    }

    public void ResetSelectionPosition()
    {
        selectionPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }

    public void ClearVertexSelection()
    {
        uvEditVertex = null;
        verticeSelection.Clear();
        ResetSelectionPosition();
    }

    public void ClearTriangleSelection()
    {
        triangleSelection.Clear();
    }

    public void SetTriangleHover(HairyPlotterTriangle triangle)
    {
        triangleHover = triangle;
    }

    public void SetUvEditVertex(HairyPlotterVertex vertex)
    {
        uvEditVertex = vertex;
    }

    public void ToEachSelection(Action<HairyPlotterVertex> action)
    {
        foreach (HairyPlotterVertex vertex in verticeSelection)
        {
            action(vertex);
        }

        Dirty = true;
    }

    public HairyPlotterVertex CreateVertex()
    {
        return CreateVertex(Vector3.zero, Vector2.zero);
    }

    public HairyPlotterVertex CreateVertex(Vector3 pos, Vector2 uv)
    {
        // Create vertex
        HairyPlotterVertex vertex = new HairyPlotterVertex(pos, uv, this);

        // Set index
        vertex.Index = vertices.Count;

        // Add to list
        vertices.Add(vertex);

        // Return
        return vertex;
    }

    public HairyPlotterTriangle CreateTriangle(HairyPlotterVertex v0, HairyPlotterVertex v1, HairyPlotterVertex v2)
    {
        // Create tri
        HairyPlotterTriangle triangle = new HairyPlotterTriangle(this);

        // Set vertices for triangle
        triangle.SetVertices(v0, v1, v2);

        // Add to triangle list
        triangles.Add(triangle);

        // Return
        return triangle;
    }

    public HairyPlotterTriangle CreateTriangle(int i0, int i1, int i2)
    {
        return CreateTriangle(vertices[i0], vertices[i1], vertices[i2]);
    }

    public HairyPlotterTriangle GetTriangle(int index)
    {
        return triangles[index];
    }

    public void DestroyVertex(HairyPlotterVertex vertex)
    {
        if (vertex != null)
        {
            if (uvEditVertex == vertex)
                uvEditVertex = null;

            // Mark dirty
            Dirty = true;

            // Remove from vertices and selection
            vertices.Remove(vertex);
            verticeSelection.Remove(vertex);

            for (int i = 0; i < vertex.TriangleCount; ++i)
            {
                DestroyTriangle(vertex.GetTriangle(i));
            }

            // Update
            UpdateVertexIndexes();
        }
    }

    public void DestroyTriangle(HairyPlotterTriangle triangle)
    {
        if (triangle != null)
        {
            // Mark dirty
            Dirty = true;

            // Clear hover state
            if (triangleHover == triangle)
                triangleHover = null;

            // Remove selection
            triangleSelection.Remove(triangle);

            // Remove triangle from mesh
            triangles.Remove(triangle);

            // Remove triangle from it's three vertices
            triangle.GetVertex(0).RemoveTriangle(triangle);
            triangle.GetVertex(1).RemoveTriangle(triangle);
            triangle.GetVertex(2).RemoveTriangle(triangle);

            // Clear link between triangle and vertices
            triangle.ClearVertices();

            // Update
            UpdateVertexIndexes();
        }
    }

    public void ClearVertices()
    {
        vertices.Clear();
        verticeSelection.Clear();
        ClearTriangles();
        uvEditVertex = null;
        Dirty = true;
    }

    public void ClearTriangles()
    {
        triangles.Clear();
        triangleHover = null;
        Dirty = true;
    }

    public void InitEditing()
    {
        if (!EditMesh)
        {
            // Grab filter
            MeshFilter filter = GetComponent<MeshFilter>();

            // Copy original mesh
            EditMesh = CloneMesh(filter.sharedMesh);

            // If it's an asset
            if (AssetDatabase.IsMainAsset(filter.sharedMesh))
            {
                // Store original mesh
                OriginalMesh = AssetDatabase.GetAssetPath(filter.sharedMesh);
            }

            // Overwrite shared mesh
            filter.sharedMesh = EditMesh;

            // Create temp asset
            AssetDatabase.CreateAsset(EditMesh, "Assets/HairyPlotterMesh.asset");
            AssetDatabase.SaveAssets();

            vertices = null;
            triangles = null;
            verticeSelection = null;
        }

        if (EditMesh)
        {
            if (vertices == null || triangles == null)
            {
                Vector2[] meshUv = EditMesh.uv;
                Vector3[] meshVertices = EditMesh.vertices;
                int[] meshTriangles = EditMesh.triangles;

                triangles = new List<HairyPlotterTriangle>();
                vertices = new List<HairyPlotterVertex>();
                verticeSelection = new HashSet<HairyPlotterVertex>();

                // Add verts
                for (int i = 0; i < meshVertices.Length; i += 1)
                {
                    CreateVertex(meshVertices[i], i < meshUv.Length ? meshUv[i] : Vector2.zero);
                }

                // Add tris
                for (int i = 0; i < meshTriangles.Length; i += 3)
                {
                    CreateTriangle(meshTriangles[i + 0], meshTriangles[i + 1], meshTriangles[i + 2]);
                }
            }
        }
    }

    public void UpdateVertexIndexes()
    {
        // Re-index all vertices
        for (int i = 0; i < vertices.Count; ++i)
        {
            vertices[i].Index = i;
        }
    }

    public void UpdateMesh()
    {
        if (Dirty)
        {
            // Re-index all vertices
            UpdateVertexIndexes();

            // Update edit mesh
            EditMesh.Clear();
            EditMesh.vertices = vertices.Select(x => x.Position).ToArray();
            EditMesh.uv = vertices.Select(x => x.Uv).ToArray();
            EditMesh.triangles = triangles.SelectMany(x => x.VertexIndexes).ToArray();
            EditMesh.RecalculateBounds();
            EditMesh.RecalculateNormals();
        }
    }

    public static Mesh CloneMesh(Mesh mesh)
    {
        if (!mesh)
        {
            mesh = new Mesh();
            mesh.name = "Empty Mesh";
        }

        Mesh clone = new Mesh();

        clone.vertices = mesh.vertices;
        clone.uv = mesh.uv;
        clone.triangles = mesh.triangles;
        clone.normals = mesh.normals;
        clone.name = mesh.name;
        clone.RecalculateBounds();
        clone.RecalculateNormals();

        return clone;
    }

    public static bool RayIntersectsTriangle(Ray ray, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        //Compute vectors along two edges of the triangle.
        Vector3 edge1, edge2;

        //Edge 1
        edge1.x = vertex2.x - vertex1.x;
        edge1.y = vertex2.y - vertex1.y;
        edge1.z = vertex2.z - vertex1.z;

        //Edge2
        edge2.x = vertex3.x - vertex1.x;
        edge2.y = vertex3.y - vertex1.y;
        edge2.z = vertex3.z - vertex1.z;

        //Cross product of ray direction and edge2 - first part of determinant.
        Vector3 directioncrossedge2;
        directioncrossedge2.x = (ray.direction.y * edge2.z) - (ray.direction.z * edge2.y);
        directioncrossedge2.y = (ray.direction.z * edge2.x) - (ray.direction.x * edge2.z);
        directioncrossedge2.z = (ray.direction.x * edge2.y) - (ray.direction.y * edge2.x);

        //Compute the determinant.
        float determinant;
        //Dot product of edge1 and the first part of determinant.
        determinant = (edge1.x * directioncrossedge2.x) + (edge1.y * directioncrossedge2.y) + (edge1.z * directioncrossedge2.z);

        //If the ray is parallel to the triangle plane, there is no collision.
        //This also means that we are not culling, the ray may hit both the
        //back and the front of the triangle.
        if (determinant > -1e-6f && determinant < 1e-6f)
        {
            return false;
        }

        float inversedeterminant = 1.0f / determinant;

        //Calculate the U parameter of the intersection point.
        Vector3 distanceVector;
        distanceVector.x = ray.origin.x - vertex1.x;
        distanceVector.y = ray.origin.y - vertex1.y;
        distanceVector.z = ray.origin.z - vertex1.z;

        float triangleU;
        triangleU = (distanceVector.x * directioncrossedge2.x) + (distanceVector.y * directioncrossedge2.y) + (distanceVector.z * directioncrossedge2.z);
        triangleU *= inversedeterminant;

        //Make sure it is inside the triangle.
        if (triangleU < 0f || triangleU > 1f)
        {
            return false;
        }

        //Calculate the V parameter of the intersection point.
        Vector3 distancecrossedge1;
        distancecrossedge1.x = (distanceVector.y * edge1.z) - (distanceVector.z * edge1.y);
        distancecrossedge1.y = (distanceVector.z * edge1.x) - (distanceVector.x * edge1.z);
        distancecrossedge1.z = (distanceVector.x * edge1.y) - (distanceVector.y * edge1.x);

        float triangleV;
        triangleV = ((ray.direction.x * distancecrossedge1.x) + (ray.direction.y * distancecrossedge1.y)) + (ray.direction.z * distancecrossedge1.z);
        triangleV *= inversedeterminant;

        //Make sure it is inside the triangle.
        if (triangleV < 0f || triangleU + triangleV > 1f)
        {
            return false;
        }

        //Compute the distance along the ray to the triangle.
        float raydistance;
        raydistance = (edge2.x * distancecrossedge1.x) + (edge2.y * distancecrossedge1.y) + (edge2.z * distancecrossedge1.z);
        raydistance *= inversedeterminant;

        //Is the triangle behind the ray origin?
        if (raydistance < 0f)
        {
            return false;
        }

        return true;
    }
}

#pragma warning disable 0414
public class HairyPlotterVertex
{
    static int hashCodeCounter = 0;

    int hashCode = ++hashCodeCounter;
    HairyPlotter plotter;
    List<HairyPlotterTriangle> triangles = new List<HairyPlotterTriangle>();

    public int Index = -1;
    public Vector2 Uv;
    public Vector3 Position;
    public int TriangleCount { get { return triangles.Count; } }
    public HashSet<HairyPlotterTriangle> TriangleSet { get { return new HashSet<HairyPlotterTriangle>(triangles); }}

    public HairyPlotterVertex(Vector3 pos, Vector2 uv, HairyPlotter hairyPlotter)
    {
        Uv = uv;
        Position = pos;
        plotter = hairyPlotter;
    }

    public void AddTriangle(HairyPlotterTriangle triangle)
    {
        triangles.Add(triangle);
    }

    public HairyPlotterTriangle GetTriangle(int index)
    {
        return triangles[index];
    }

    public void RemoveTriangle(HairyPlotterTriangle triangle)
    {
        triangles.Remove(triangle);
    }
    
    public override bool Equals(object obj)
    {
        return ReferenceEquals(obj, this);
    }

    public override int GetHashCode()
    {
        return hashCode;
    }
}
#pragma warning restore 0414

public class HairyPlotterTriangle
{
    static int hashCodeCounter = 0;

    int hashCode = ++hashCodeCounter;
    HairyPlotter plotter;
    HairyPlotterVertex[] vertices;

    public int[] VertexIndexes
    {
        get { return new int[3] { vertices[0].Index, vertices[1].Index, vertices[2].Index }; }
    }

    public HairyPlotterVertex[] VertexObjects
    {
        get { return vertices.Select(v => v).ToArray(); }
    }

    public HairyPlotterTriangle(HairyPlotter hairyPlotter)
    {
        plotter = hairyPlotter;
        vertices = new HairyPlotterVertex[3];
    }

    public void ClearVertices()
    {
        vertices = new HairyPlotterVertex[3];
    }

    public HairyPlotterVertex GetVertex(int index)
    {
        return vertices[index];
    }

    public void SetVertices(HairyPlotterVertex v0, HairyPlotterVertex v1, HairyPlotterVertex v2)
    {
        vertices = new HairyPlotterVertex[3] { v0, v1, v2 };

        v0.AddTriangle(this);
        v1.AddTriangle(this);
        v2.AddTriangle(this);

        plotter.Dirty = true;
    }

    public void ReplaceVertex(HairyPlotterVertex vertex, int index)
    {
        vertices[index].RemoveTriangle(this);
        vertices[index] = vertex;
        vertices[index].AddTriangle(this);
        plotter.Dirty = true;
    }

    public void SwitchVertices(int a, int b)
    {
        SwitchVertices(vertices[a], vertices[b]);
    }

    public void SwitchVertices(HairyPlotterVertex a, HairyPlotterVertex b)
    {
        int aIndex = Array.IndexOf(vertices, a);
        int bIndex = Array.IndexOf(vertices, b);

        if (aIndex == -1 && bIndex == -1)
        {
            return;
        }

        // Switch both
        if (aIndex > -1 && bIndex > -1)
        {
            vertices[bIndex] = a;
            vertices[aIndex] = b;
        }

        // Switch a for b
        if (aIndex > -1)
        {
            vertices[aIndex] = b;

            a.RemoveTriangle(this);
            b.AddTriangle(this);
        }

        // Switch b for a
        if (bIndex > -1)
        {
            vertices[bIndex] = a;

            b.RemoveTriangle(this);
            a.AddTriangle(this);
        }

        plotter.Dirty = true;
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(obj, this);
    }

    public override int GetHashCode()
    {
        return hashCode;
    }
}

public enum HairyPlotterActions
{
    None,
    VertexDelete,
    VertexMove,
    VertexClear,
    VertexClearUnused,
    VertexUvEdit,
    TriangleAdd,
    TriangleDelete,
    TriangleClear,
    TriangleSwitch
}