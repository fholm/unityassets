//#define UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimMath;

namespace SlimNet.SceneGraph
{
    public class OCTree
    {
        public readonly int MaxObjectsPerNode;

        internal Node Root;
        internal readonly List<Collidable> DirtyObjects = new List<Collidable>();

        public int Count { get { return Root.Count; } }

        public OCTree(int maxObjectPerNode, Vector3 center, Vector3 extents)
        {
            Root = new Node(this, null, center, extents, 0);
            Root.Divide();
            
            MaxObjectsPerNode = maxObjectPerNode;
        }

        public void RegisterDirty(Collidable col)
        {
            DirtyObjects.Add(col);
        }

        internal void UpdateDirty()
        {
            var count = DirtyObjects.Count;
            for (var i = 0; i < count; ++i)
            {
                DirtyObjects[i].Update(this);
            }
        }

        public void Insert(Collidable col)
        {
            Root.Insert(col);
        }

        public void Remove(Collidable col)
        {
            if (col.Node != null)
                col.Remove();
        }

        public List<Actor> Intersect(Collidable col)
        {
            UpdateDirty();
            return Intersect(col.Box);
        }

        public List<Actor> Intersect(Ray ray)
        {
            UpdateDirty();
            var result = new List<Actor>();
            Root.Intersect(ref ray, result);
            return result;
        }

        public List<Actor> Intersect(BoundingBox box)
        {
            UpdateDirty();
            var result = new List<Actor>();
            Root.Intersect(ref box, result);
            return result;
        }

        public List<Actor> Intersect(BoundingSphere sphere)
        {
            UpdateDirty();
            var result = new List<Actor>();
            Root.Intersect(ref sphere, result);
            return result;
        }

        /*
        // DebugMethod
        internal void Insert(float cX, float cY, float cZ, float eX, float eY, float eZ)
        {
            var center = new Vector3(cX, cY, cZ);
            var extents = new Vector3(eX, eY, eZ);

            Insert(new Collidable
            {
                Center = center,
                Extents = extents,
                Box = new BoundingBox { Minimum = (center - extents), Maximum = (center + extents) }
            }); 
        }
        */

        public void Draw(Action<Vector3, Vector3, Color4> drawCallback)
        {
            Root.Draw(drawCallback);
        }

        public void Clear()
        {
            Root = new Node(this, null, Root.Center, Root.Extents, 0);
            Root.Divide();
        }
    }
}
