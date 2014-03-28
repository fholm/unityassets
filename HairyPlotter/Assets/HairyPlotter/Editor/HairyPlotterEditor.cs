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

[CustomEditor(typeof(HairyPlotter))]
public class HairyPlotterEditor : Editor
{
    public static readonly Color VertexToolbarColor = new Color(135f / 255f, 203f / 255f, 255f / 255f, 255f / 255f);
    public static readonly Color TriangleToolbarColor = new Color(198f / 255f, 160f / 255f, 255f / 255f, 255f / 255f);
    public static readonly Color VertexSelectionToolbarColor = new Color(216f / 255f, 255f / 255f, 160f / 255f, 255f / 255f);
    public static readonly Color TriangleSelectionToolbarColor = new Color(255f / 255f, 200f / 255f, 160f / 255f, 255f / 255f);
    public static readonly Color ActiveUvEditorColor = new Color(129f / 255f, 179f / 255f, 255f / 255f, 255f / 255f);

    public static readonly Color SelectedVertexColor = new Color(1, 0, 0, 1);
    public static readonly Color UvEditedVertexColor = new Color(0, 0, 1, 1);

    public static readonly Color TriangleHoverColor = new Color(135f / 255f, 203f / 255f, 255f / 255f, 127f / 255f);
    public static readonly Color TriangleSelectedColor = new Color(255f / 255f, 0f, 0f, 127f / 255f);

    HairyPlotter plotter = null;
    SceneView.OnSceneFunc sceneViewCallback = null;
    Texture2D markerTexture = null;
    Material material = null;

    Material highlightMaterial
    {
        get
        {
            if (!material)
            {
                material = new Material("Shader \"Lines/Colored Blended\" {" +
                    "SubShader { Pass { " +
                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                    "    BindChannels {" +
                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                    "} } }");
            }

            return material;
        }
    }

    Texture2D uvMarkerTexture
    {
        get
        {
            if (!markerTexture)
            {
                markerTexture = (Texture2D)Resources.Load("HairyPlotter_UvMarkerTexture", typeof(Texture2D));
            }

            return markerTexture;
        }
    }

    void DrawVertices()
    {
        if (!plotter)
            return;

        if (plotter.VertexCount == 0)
            return;

        for (int i = 0; i < plotter.VertexCount; ++i)
        {
            Handles.color = Color.white;
            float size = 0.04f;

            if (plotter.IsSelected(i))
            {
                continue;
            }

            if (Handles.Button(plotter.GetVertex(i).Position, Quaternion.identity, size, size, Handles.CubeCap))
            {
                plotter.ToggleSelected(plotter.GetVertex(i));
                Repaint();
            }
        }

        for (int i = 0; i < plotter.VertexCount; ++i)
        {
            Handles.color = Color.white;
            float size = 0.04f;

            if (plotter.IsSelected(i))
            {
                if (plotter.UvEditVertex == plotter.GetVertex(i))
                {
                    Handles.color = UvEditedVertexColor;
                    size = 0.02f;
                }
                else
                {
                    Handles.color = SelectedVertexColor;
                    size = 0.02f;
                }
            }
            else
            {
                continue;
            }

            if (Handles.Button(plotter.GetVertex(i).Position, Quaternion.identity, size, size, Handles.CubeCap))
            {
                plotter.ToggleSelected(plotter.GetVertex(i));
                Repaint();
            }
        }
    }

    void DrawTriangles()
    {
        if (plotter.HoveredTriangle != null)
        {
            HairyPlotterTriangle tri = plotter.HoveredTriangle;

            highlightMaterial.SetPass(0);

            GL.Begin(GL.TRIANGLES);
            GL.Color(TriangleHoverColor);
            GL.Vertex(tri.GetVertex(0).Position);
            GL.Vertex(tri.GetVertex(1).Position);
            GL.Vertex(tri.GetVertex(2).Position);
            GL.End();
        }

        List<HairyPlotterTriangle> selectedTriangles = plotter.SelectedTriangles;

        if (selectedTriangles.Count > 0)
        {
            GL.Begin(GL.TRIANGLES);
            GL.Color(TriangleSelectedColor);

            for (int i = 0; i < selectedTriangles.Count; ++i)
            {
                HairyPlotterTriangle tri = selectedTriangles[i];
                GL.Vertex(tri.GetVertex(0).Position);
                GL.Vertex(tri.GetVertex(1).Position);
                GL.Vertex(tri.GetVertex(2).Position);
            }

            GL.End();
        }
    }

    void OnSceneViewRender(SceneView sceneView)
    {
        if (!plotter) 
            return;

        plotter.InitEditing();
        DrawVertices();
        DrawTriangles();
    }

    void OnSceneGUI()
    {
        if (!plotter) 
            return;

        plotter.InitEditing();

        if (plotter.CurrentAction == HairyPlotterActions.None)
        {
            plotter.SetTriangleHover(HoverTriangle());

            if (plotter.ToggleSelected(PickTriangle()))
            {
                Repaint();
            }

            if (plotter.HoveredTriangle != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }
        
        if (plotter.CurrentAction == HairyPlotterActions.TriangleSwitch)
        {
            HairyPlotterTriangle triangle = PickTriangle();

            if (triangle != null)
            {
                List<HairyPlotterVertex> vertices = plotter.SelectedVertices;

                if (vertices.Count == 2)
                {
                    triangle.SwitchVertices(vertices[0], vertices[1]);
                    plotter.ClearVertexSelection();
                }
                else
                {
                    Debug.LogWarning("Switch In Triangle requires exactly two selected vertices");
                }
            }
        }

        DrawTriangles();
    }

    HairyPlotterTriangle HoverTriangle()
    {
        return RaycastTriangle();
    }

    HairyPlotterTriangle PickTriangle()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            return RaycastTriangle();
        }

        return null;
    }

    HairyPlotterTriangle RaycastTriangle()
    {
        // This transform from GUI space to scene camera ray
        Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        // Iterate over each triangle in plotter
        for (int i = 0; i < plotter.TriangleCount; ++i)
        {
            // Grab triangle
            HairyPlotterTriangle triangle = plotter.GetTriangle(i);

            // Intersect ray
            if (HairyPlotter.RayIntersectsTriangle(
                r,
                triangle.GetVertex(0).Position,
                triangle.GetVertex(1).Position,
                triangle.GetVertex(2).Position
            ))
            {
                // Found!
                plotter.CurrentAction = HairyPlotterActions.None;

                // Destroy triangle
                return triangle;
            }
        }

        return null;
    }

    public override void OnInspectorGUI()
    {
        plotter = (HairyPlotter)target;

        if (!plotter) 
            return;

        if (sceneViewCallback == null)
            sceneViewCallback = new SceneView.OnSceneFunc(OnSceneViewRender);

        if (!ReferenceEquals(SceneView.onSceneGUIDelegate, sceneViewCallback))
            SceneView.onSceneGUIDelegate = sceneViewCallback;

        plotter.InitEditing();

        EditorGUILayout.LabelField("Edit Mesh", plotter.EditMesh.name);
        EditorGUILayout.LabelField("Original Mesh", plotter.OriginalMesh ?? "");
        EditorGUILayout.LabelField("Vertex Count", plotter.VertexCount.ToString());
        EditorGUILayout.LabelField("Triangle Count", plotter.TriangleCount.ToString());
        EditorGUILayout.LabelField("Unused Vertices", plotter.UnusedVerticesCount.ToString());

        VertexToolbox();
        TriangleToolbox();
        VertexSelectionToolbox();
        TriangleSelectionToolbox();
        UvToolbox();

        plotter.UpdateMesh();

        if (Event.current.type == EventType.Repaint)
        {
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.Repaint();
            }
        }
    }

    void VertexToolbox()
    {
        GUI.color = VertexToolbarColor;

        EditorGUILayout.LabelField("Vertex Toolbox", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add", EditorStyles.miniButton))
        {
            plotter.AddSelection(plotter.CreateVertex());
        }

        if (GUILayout.Button("Pop", EditorStyles.miniButton))
        {
            plotter.DestroyVertex(plotter.LastVertex);
        }

        GUI.color = plotter.CurrentAction == HairyPlotterActions.VertexUvEdit ? ActiveUvEditorColor : VertexToolbarColor;

        if (GUILayout.Button("UV Editor", EditorStyles.miniButton))
        {
            if (plotter.CurrentAction == HairyPlotterActions.VertexUvEdit)
            {
                plotter.CurrentAction = HairyPlotterActions.None;
            }
            else
            {
                plotter.CurrentAction = HairyPlotterActions.VertexUvEdit;

                if (plotter.SelectedVertices.Count > 0)
                {
                    plotter.SetUvEditVertex(plotter.SelectedVertices.FirstOrDefault());
                }
            }
        }

        GUI.color = VertexToolbarColor;

        if (GUILayout.Button("Select All", EditorStyles.miniButton))
        {
            plotter.ClearVertexSelection();

            for (int i = 0; i < plotter.VertexCount; ++i)
            {
                plotter.AddSelection(i);
            }
        }

        GUI.color = VertexToolbarColor;

        if (plotter.CurrentAction == HairyPlotterActions.VertexClear)
        {
            if (GUILayout.Button("Yes!", EditorStyles.miniButton))
            {
                plotter.ClearVertices();
                plotter.CurrentAction = HairyPlotterActions.None;
            }

            if (GUILayout.Button("No!", EditorStyles.miniButton))
            {
                plotter.CurrentAction = HairyPlotterActions.None;
            }
        }
        else
        {
            if (GUILayout.Button("Delete All", EditorStyles.miniButton))
            {
                plotter.CurrentAction = HairyPlotterActions.VertexClear;
            }
        }

        if (plotter.CurrentAction == HairyPlotterActions.VertexClearUnused)
        {
            if (GUILayout.Button("Yes!", EditorStyles.miniButton))
            {
                foreach (HairyPlotterVertex vertex in plotter.UnusedVertices)
                {
                    plotter.DestroyVertex(vertex);
                }

                plotter.CurrentAction = HairyPlotterActions.None;
            }

            if (GUILayout.Button("No!", EditorStyles.miniButton))
            {
                plotter.CurrentAction = HairyPlotterActions.None;
            }
        }
        else
        {
            if (GUILayout.Button("Delete Unused", EditorStyles.miniButton))
            {
                plotter.CurrentAction = HairyPlotterActions.VertexClearUnused;
            }
        }

        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;
    }

    void TriangleToolbox()
    {
        if (plotter.CurrentAction != HairyPlotterActions.VertexUvEdit)
        {
            EditorGUILayout.LabelField("Triangle Toolbox", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (plotter.CurrentAction == HairyPlotterActions.TriangleAdd)
            {
                GUI.color = ActiveUvEditorColor;

                if (plotter.VertexSelectionCount == 3)
                {
                    CreateTriangleFromSelection();
                }
            }
            else
            {
                GUI.color = TriangleToolbarColor;
            }

            if (GUILayout.Button("Create", EditorStyles.miniButton))
            {
                if (plotter.VertexSelectionCount == 3)
                {
                    CreateTriangleFromSelection();
                }
                else
                {
                    if (plotter.CurrentAction == HairyPlotterActions.TriangleAdd)
                    {
                        plotter.CurrentAction = HairyPlotterActions.None;
                        plotter.ClearVertexSelection();
                    }
                    else
                    {
                        plotter.CurrentAction = HairyPlotterActions.TriangleAdd;
                    }
                }
            }

            if (GUILayout.Button("Pop", EditorStyles.miniButton))
            {
                plotter.DestroyTriangle(plotter.LastTriangle);
            }

            if (plotter.CurrentAction == HairyPlotterActions.TriangleClear)
            {
                if (GUILayout.Button("Yes!", EditorStyles.miniButton))
                {
                    plotter.ClearTriangles();
                    plotter.CurrentAction = HairyPlotterActions.None;
                }

                if (GUILayout.Button("No!", EditorStyles.miniButton))
                {
                    plotter.CurrentAction = HairyPlotterActions.None;
                }
            }
            else
            {
                if (GUILayout.Button("Delete All", EditorStyles.miniButton))
                {
                    plotter.CurrentAction = HairyPlotterActions.TriangleClear;
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    void TriangleSelectionToolbox()
    {
        if (plotter.CurrentAction != HairyPlotterActions.VertexUvEdit)
        {
            if (plotter.TriangleSelectionCount > 0)
            {
                EditorGUILayout.LabelField("Triangle Selection: " + plotter.TriangleSelectionCount, EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();

                GUI.color = TriangleSelectionToolbarColor;

                if (GUILayout.Button("Create Unique Vertices", EditorStyles.miniButton))
                {
                    HashSet<HairyPlotterTriangle> selectedTriangles = new HashSet<HairyPlotterTriangle>(plotter.SelectedTriangles);
                    HashSet<HairyPlotterVertex> uniqueVertices = new HashSet<HairyPlotterVertex>(selectedTriangles.SelectMany(x => x.VertexObjects).Distinct());

                    // Iterate over each vertex that the selectedTriangles use
                    foreach (HairyPlotterVertex vertex in uniqueVertices)
                    {
                        // Iterate over each triangle the vertex blongs to
                        foreach (HairyPlotterTriangle vertexTriangle in vertex.TriangleSet)
                        {
                            // If the triangle is NOT in the selected triangles
                            if (!selectedTriangles.Contains(vertexTriangle))
                            {
                                // Create a clone of the vertex
                                HairyPlotterVertex vertexClone = plotter.CreateVertex(vertex.Position, vertex.Uv);

                                // And step through each selected triangle, and switch out the shared vertex for the clone
                                foreach (HairyPlotterTriangle selectedTriangle in selectedTriangles)
                                {
                                    selectedTriangle.SwitchVertices(vertex, vertexClone);
                                }

                                // We're done
                                break;
                            }
                        }
                    }

                    plotter.Dirty = true;
                    plotter.UpdateVertexIndexes();
                }

                if (GUILayout.Button("Select Vertices", EditorStyles.miniButton))
                {
                    HashSet<HairyPlotterTriangle> selectedTriangles = new HashSet<HairyPlotterTriangle>(plotter.SelectedTriangles);
                    HashSet<HairyPlotterVertex> selectedTrianglesVertices = new HashSet<HairyPlotterVertex>(selectedTriangles.SelectMany(x => x.VertexObjects));

                    plotter.ClearVertexSelection();

                    foreach (HairyPlotterVertex vertex in selectedTrianglesVertices)
                    {
                        plotter.AddSelection(vertex);
                    }

                    // Clear triangle selection
                    plotter.ClearTriangleSelection();
                }

                if (GUILayout.Button("Clear Selection", EditorStyles.miniButton))
                {
                    plotter.ClearTriangleSelection();
                }

                if (plotter.CurrentAction == HairyPlotterActions.TriangleDelete)
                {
                    if (GUILayout.Button("Yes!", EditorStyles.miniButton))
                    {
                        foreach (HairyPlotterTriangle triangle in plotter.SelectedTriangles)
                        {
                            plotter.DestroyTriangle(triangle);
                        }

                        plotter.CurrentAction = HairyPlotterActions.None;
                    }

                    if (GUILayout.Button("No!", EditorStyles.miniButton))
                    {
                        plotter.CurrentAction = HairyPlotterActions.None;
                    }
                }
                else
                {
                    if (GUILayout.Button("Delete Selected", EditorStyles.miniButton))
                    {
                        plotter.CurrentAction = HairyPlotterActions.TriangleDelete;
                    }
                }

                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void VertexSelectionToolbox()
    {
        if (plotter.VertexSelectionCount > 0)
        {
            GUI.color = VertexSelectionToolbarColor;

            EditorGUILayout.LabelField("Vertex Selection", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear", EditorStyles.miniButton))
            {
                plotter.ClearVertexSelection();
                SceneView.lastActiveSceneView.Repaint();
            }

            if (GUILayout.Button("Duplicate And Select", EditorStyles.miniButton))
            {
                // Copy current selection
                HashSet<HairyPlotterVertex> prevSelection = new HashSet<HairyPlotterVertex>(plotter.SelectedVertices);

                // Clear current selection
                plotter.ClearVertexSelection();

                // Duplicate each
                foreach (HairyPlotterVertex v in prevSelection)
                {
                    plotter.AddSelection(plotter.CreateVertex(v.Position, v.Uv));
                }
            }

            GUI.color = plotter.CurrentAction == HairyPlotterActions.TriangleSwitch ? ActiveUvEditorColor : VertexSelectionToolbarColor;

            if (GUILayout.Button("Switch In Triangle", EditorStyles.miniButton))
            {
                if (plotter.CurrentAction == HairyPlotterActions.TriangleSwitch)
                {
                    plotter.CurrentAction = HairyPlotterActions.None;
                }
                else
                {
                    if (plotter.SelectedVertices.Count == 2)
                    {
                        plotter.CurrentAction = HairyPlotterActions.TriangleSwitch;
                    }
                    else
                    {
                        Debug.LogWarning("Switch In Triangle requires exactly two selected vertices");
                    }
                }
            }

            GUI.color = VertexSelectionToolbarColor;

            if (plotter.CurrentAction == HairyPlotterActions.VertexDelete)
            {
                if (GUILayout.Button("Yes!", EditorStyles.miniButton))
                {
                    plotter.CurrentAction = HairyPlotterActions.None;

                    foreach (HairyPlotterVertex vertex in plotter.SelectedVertices)
                    {
                        plotter.DestroyVertex(vertex);
                    }
                }

                if (GUILayout.Button("No!", EditorStyles.miniButton))
                {
                    plotter.CurrentAction = HairyPlotterActions.None;
                }
            }
            else
            {
                if (GUILayout.Button("Delete Selected", EditorStyles.miniButton))
                {
                    plotter.CurrentAction = HairyPlotterActions.VertexDelete;
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("All");
            plotter.SelectionPosition = EditVector3(plotter.SelectionPosition);
            EditorGUILayout.EndHorizontal();

            if (plotter.SelectionPosition.x != float.MaxValue)
                plotter.ToEachSelection(v => v.Position.x = plotter.SelectionPosition.x);

            if (plotter.SelectionPosition.y != float.MaxValue)
                plotter.ToEachSelection(v => v.Position.y = plotter.SelectionPosition.y);

            if (plotter.SelectionPosition.z != float.MaxValue)
                plotter.ToEachSelection(v => v.Position.z = plotter.SelectionPosition.z);
        }

        List<HairyPlotterVertex> selectedVertices = plotter.SelectedVertices;

        foreach (HairyPlotterVertex vertex in selectedVertices)
        {
            EditorGUILayout.BeginHorizontal();

            // Vertex index
            GUILayout.Label("#" + vertex.Index);

            // Lets us deselect a specific vertex
            if (GUILayout.Button("Deselect", EditorStyles.miniButton))
                plotter.RemoveSelection(vertex);

            // Edit XYZ of vertex
            Vector3 pos = EditVector3(vertex.Position);

            // When editing UVs allow switching
            if (selectedVertices.Count > 1 && plotter.CurrentAction == HairyPlotterActions.VertexUvEdit)
            {
                if (GUILayout.Button("Edit UVs"))
                {
                    plotter.SetUvEditVertex(vertex);
                }
            }

            EditorGUILayout.EndHorizontal();

            // If position was updated
            if (pos != vertex.Position)
            {
                vertex.Position = pos;

                plotter.Dirty = true;
                plotter.ResetSelectionPosition();
            }
        }

        GUI.color = Color.white;
    }

    void UvToolbox()
    {
        if (plotter.CurrentAction == HairyPlotterActions.VertexUvEdit)
        {
            GUI.color = VertexToolbarColor;

            if (plotter.UvEditVertex == null)
            {
                GUILayout.Label("UV Editor", EditorStyles.boldLabel);
                GUILayout.Label("Select one or more vertices to edit UV coords", EditorStyles.miniLabel);
                GUI.color = Color.white;
                return;
            }

            float x = 0;
            float y = 0;

            HairyPlotterVertex vertex = plotter.UvEditVertex;
            GUILayout.Label("UV Editor for #" + vertex.Index, EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            Vector2 value = EditVector2(vertex.Uv);
            EditorGUILayout.EndHorizontal();

            if (vertex.Uv != value)
            {
                vertex.Uv = value;
                plotter.Dirty = true;
            }

            x = value.x;
            y = value.y;

            GUI.color = Color.white;

            if (plotter.renderer && plotter.renderer.sharedMaterial && plotter.renderer.sharedMaterial.mainTexture)
            {
                float ratio = (float)uvMarkerTexture.width / (float)uvMarkerTexture.height;
                float w = 500f;
                float h = w * (1f / ratio);
                Rect r = GUILayoutUtility.GetRect(w, h);

                GUI.DrawTexture(r, plotter.renderer.sharedMaterial.mainTexture);

                Rect marker = new Rect(r);

                marker.xMin = r.xMin + (r.width * x);
                marker.yMin = r.yMin + (r.height * (1f - y));
                marker.width = 8;
                marker.height = 8;

                GUI.DrawTexture(marker, uvMarkerTexture);

                if (Event.current.type == EventType.MouseUp)
                {
                    Vector2 mpos = Event.current.mousePosition;

                    // Make sure we're within bounds
                    if (mpos.y - r.yMin >= 0 & mpos.y - r.yMin <= r.height && mpos.x >= 0 && mpos.x <= r.width)
                    {
                        x = (mpos.x / r.width);
                        y = 1f - (Mathf.Clamp(mpos.y - r.yMin, 0, float.MaxValue) / r.height);

                        if (x != vertex.Uv.x || y != vertex.Uv.y)
                        {
                            vertex.Uv = new Vector2(x, y);
                            plotter.Dirty = true;
                        }
                    }

                }
            }
        }
    }

    void CreateTriangleFromSelection()
    {
        if (plotter.VertexSelectionCount == 3)
        {
            List<HairyPlotterVertex> vertices = plotter.SelectedVertices;

            plotter.CurrentAction = HairyPlotterActions.None;
            plotter.CreateTriangle(vertices[0], vertices[1], vertices[2]);
            plotter.ClearVertexSelection();
        }
    }

    Vector2 EditVector2(Vector2 value)
    {
        Vector3 v = new Vector3();

        v.x = EditorGUILayout.FloatField(value.x);
        v.y = EditorGUILayout.FloatField(value.y);

        return v;
    }

    Vector3 EditVector3(Vector3 value)
    {
        Vector3 v = new Vector3();

        v.x = EditorGUILayout.FloatField(value.x);
        v.y = EditorGUILayout.FloatField(value.y);
        v.z = EditorGUILayout.FloatField(value.z);

        return v;
    }
}
