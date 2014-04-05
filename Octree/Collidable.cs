//#define UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimMath;

namespace SlimNet.SceneGraph
{
    public class Collidable
    {
        internal Node Node;
        internal Vector3 Center;
        internal Vector3 Extents;
        internal BoundingBox Box;
        internal Actor Actor;

        //TODO: Re-enable static/dynamic support
        //internal bool IsDynamic;

        public Vector3 CenterPosition { get { return Center; } }
        public Vector3 BoxExtents { get { return Extents; } }
        public BoundingBox BoundingVolume { get { return Box; } }

        internal Collidable(Vector3 center, Vector3 extents)
        {
            Center = center;
            Extents = extents;
            Box = new BoundingBox(center - extents, center + extents);
        }

        internal void Remove()
        {
            if (Node == null)
                return;

            Node.Objects.Remove(this);

            while (Node != null)
            {
                Node.Count -= 1;
                Node = Node.Parent;
            }
        }

        internal void Update(OCTree tree)
        {
            var oldNode = Node;

            Remove();

            tree.Insert(this);

            if (oldNode != null)
                oldNode.Merge();
        }

        public void Draw(Action<Vector3, Vector3, Color4> drawCallback)
        {
            drawCallback(Center, Extents, new Color4(0.0f, 1.0f, 0.0f));
        }
    }
}
