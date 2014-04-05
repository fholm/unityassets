//#define UNITY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimMath;

namespace SlimNet.SceneGraph
{
    internal class Node
    {
        public const int LEFT_TOP_FRONT = 0;
        public const int RIGHT_TOP_FRONT = 1;
        public const int LEFT_TOP_BACK = 2;
        public const int RIGHT_TOP_BACK = 3;
        public const int LEFT_BOTTOM_FRONT = 4;
        public const int RIGHT_BOTTOM_FRONT = 5;
        public const int LEFT_BOTTOM_BACK = 6;
        public const int RIGHT_BOTTOM_BACK = 7;

        internal int Count;
        internal Node[] Children;

        internal readonly Vector3 Center;
        internal readonly Vector3 Extents;
        internal readonly BoundingBox Box;
        internal readonly OCTree Tree;
        internal readonly List<Collidable> Objects;
        internal readonly int Level;
        internal readonly Node Parent;

        internal Node(OCTree tree, Node parent, Vector3 center, Vector3 extents, int level)
        {
            Tree = tree;
            Center = center;
            Extents = extents;
            Objects = new List<Collidable>();
            Box = new BoundingBox(center - extents, center + extents);
            Level = level;
            Parent = parent;
        }

        public override string ToString()
        {
            return "<Node:" + Level + ":" + Count + ">";
        }

        internal void Merge()
        {
            if (Count == 0)
            {
                Children = null;
                if (Parent != null)
                    Parent.Merge();
            }
        }

        internal void Intersect(ref BoundingBox box, List<Actor> result)
        {
            if (Box.Intersects(ref box))
            {
                if (Children != null)
                {
                    Children[LEFT_TOP_FRONT].Intersect(ref box, result);
                    Children[RIGHT_TOP_FRONT].Intersect(ref box, result);
                    Children[LEFT_TOP_BACK].Intersect(ref box, result);
                    Children[RIGHT_TOP_BACK].Intersect(ref box, result);
                    Children[LEFT_BOTTOM_FRONT].Intersect(ref box, result);
                    Children[RIGHT_BOTTOM_FRONT].Intersect(ref box, result);
                    Children[LEFT_BOTTOM_BACK].Intersect(ref box, result);
                    Children[RIGHT_BOTTOM_BACK].Intersect(ref box, result);
                }

                if (Objects.Count > 0)
                {
                    for (var i = 0; i < Objects.Count; ++i)
                    {
                        if (Objects[i].Box.Intersects(ref box))
                        {
                            ActorPool.Verify(Objects[i].Actor);
                            result.Add(Objects[i].Actor);
                        }
                    }
                }
            }
        }

        internal void Intersect(ref BoundingSphere sphere, List<Actor> result)
        {
            if (Box.Intersects(ref sphere))
            {
                if (Children != null)
                {
                    Children[LEFT_TOP_FRONT].Intersect(ref sphere, result);
                    Children[RIGHT_TOP_FRONT].Intersect(ref sphere, result);
                    Children[LEFT_TOP_BACK].Intersect(ref sphere, result);
                    Children[RIGHT_TOP_BACK].Intersect(ref sphere, result);
                    Children[LEFT_BOTTOM_FRONT].Intersect(ref sphere, result);
                    Children[RIGHT_BOTTOM_FRONT].Intersect(ref sphere, result);
                    Children[LEFT_BOTTOM_BACK].Intersect(ref sphere, result);
                    Children[RIGHT_BOTTOM_BACK].Intersect(ref sphere, result);
                }

                if (Objects.Count > 0)
                {
                    for (var i = 0; i < Objects.Count; ++i)
                    {
                        if (Objects[i].Box.Intersects(ref sphere))
                        {
                            ActorPool.Verify(Objects[i].Actor);
                            result.Add(Objects[i].Actor);
                        }
                    }
                }
            }
        }

        internal void Intersect(ref Ray ray, List<Actor> result)
        {
            if (Box.Intersects(ref ray))
            {
                if (Children != null)
                {
                    Children[LEFT_TOP_FRONT].Intersect(ref ray, result);
                    Children[RIGHT_TOP_FRONT].Intersect(ref ray, result);
                    Children[LEFT_TOP_BACK].Intersect(ref ray, result);
                    Children[RIGHT_TOP_BACK].Intersect(ref ray, result);
                    Children[LEFT_BOTTOM_FRONT].Intersect(ref ray, result);
                    Children[RIGHT_BOTTOM_FRONT].Intersect(ref ray, result);
                    Children[LEFT_BOTTOM_BACK].Intersect(ref ray, result);
                    Children[RIGHT_BOTTOM_BACK].Intersect(ref ray, result);
                }

                if (Objects.Count > 0)
                {
                    for (var i = 0; i < Objects.Count; ++i)
                    {
                        if (Objects[i].Box.Intersects(ref ray))
                        {
                            ActorPool.Verify(Objects[i].Actor);
                            result.Add(Objects[i].Actor);
                        }
                    }
                }
            }
        }

        internal void Divide()
        {
            var x2 = Extents.X / 2;
            var y2 = Extents.Y / 2;
            var z2 = Extents.Z / 2;

            var extents = new Vector3(x2, y2, z2);
            var right = Center.X + x2;
            var left = Center.X - x2;
            var top = Center.Y + y2;
            var bottom = Center.Y - y2;
            var front = Center.Z + z2;
            var back = Center.Z - z2;

            var l = Level+1;

            Children = new Node[8];
            Children[LEFT_TOP_FRONT] = new Node(Tree, this, new Vector3(left, top, front), extents, l);
            Children[RIGHT_TOP_FRONT] = new Node(Tree, this, new Vector3(right, top, front), extents, l);
            Children[LEFT_TOP_BACK] = new Node(Tree, this, new Vector3(left, top, back), extents, l);
            Children[RIGHT_TOP_BACK] = new Node(Tree, this, new Vector3(right, top, back), extents, l);
            Children[LEFT_BOTTOM_FRONT] = new Node(Tree, this, new Vector3(left, bottom, front), extents, l);
            Children[RIGHT_BOTTOM_FRONT] = new Node(Tree, this, new Vector3(right, bottom, front), extents, l);
            Children[LEFT_BOTTOM_BACK] = new Node(Tree, this, new Vector3(left, bottom, back), extents, l);
            Children[RIGHT_BOTTOM_BACK] = new Node(Tree, this, new Vector3(right, bottom, back), extents, l);
        }

        internal bool Insert(Collidable obj)
        {
            ++Count;

            if (Children == null)
            {
                if (Objects.Count < Tree.MaxObjectsPerNode || Level > 12)
                {
                    obj.Node = this;
                    Objects.Add(obj);
                    return true;
                }
                else
                {
                    Divide();

                    var oldObjectsCount = Objects.Count;
                    var oldObjects = Objects.ToArray();

                    Count = 0;
                    Objects.Clear();

                    for (var i = 0; i < oldObjectsCount; ++i)
                    {
                        Insert(oldObjects[i]);
                    }
                }
            }

            var center = obj.Center;
            var extents = obj.Extents;

            var right = center.X + extents.X;
            var left = center.X - extents.X;
            var top = center.Y + extents.Y;
            var bottom = center.Y - extents.Y;
            var front = center.Z + extents.Z;
            var back = center.Z - extents.Z;

            // in right 
            if (left > Center.X)
            {
                // in right_top
                if (bottom > Center.Y)
                {
                    // in right_top_back
                    if (front < Center.Z)
                    {
                        Children[RIGHT_TOP_BACK].Insert(obj);
                    }

                    // in right_top_front
                    else if (back > Center.Z)
                    {
                        Children[RIGHT_TOP_FRONT].Insert(obj);
                    }

                    // in both Z quadrants
                    else
                    {
                        obj.Node = this;
                        Objects.Add(obj);
                    }
                }

                // in the right_bottom
                else if (top < Center.Y)
                {

                    // in right_bottom_back
                    if (front < Center.Z)
                    {
                        Children[RIGHT_BOTTOM_BACK].Insert(obj);
                    }

                    // in right_bottom_front
                    else if (back > Center.Z)
                    {
                        Children[RIGHT_BOTTOM_FRONT].Insert(obj);
                    }

                    // in both Z quadrants
                    else
                    {
                        obj.Node = this;
                        Objects.Add(obj);
                    }
                }

                // in both right Y quadrants
                else
                {
                    obj.Node = this;
                    Objects.Add(obj);
                }
            }

            // in the left X quadrant
            else if (right < Center.X)
            {

                // in left_top
                if (bottom > Center.Y)
                {
                    // in left_top_back
                    if (front < Center.Z)
                    {
                        Children[LEFT_TOP_BACK].Insert(obj);
                    }

                    // in left_top_front
                    else if (back > Center.Z)
                    {
                        Children[LEFT_TOP_FRONT].Insert(obj);
                    }

                    // in both Z quadrants
                    else
                    {
                        obj.Node = this;
                        Objects.Add(obj);
                    }
                }

                // in the left_bottom
                else if (top < Center.Y)
                {

                    // in left_bottom_back
                    if (front < Center.Z)
                    {
                        Children[LEFT_BOTTOM_BACK].Insert(obj);
                    }

                    // in left_bottom_front
                    else if (back > Center.Z)
                    {
                        Children[LEFT_BOTTOM_FRONT].Insert(obj);
                    }

                    // in both Z quadrants
                    else
                    {
                        obj.Node = this;
                        Objects.Add(obj);
                    }
                }

                // in both left Y quadrants
                else
                {
                    obj.Node = this;
                    Objects.Add(obj);
                }

            }

            // in both X quadrants
            else
            {
                obj.Node = this;
                Objects.Add(obj);
            }

            return false;
        }

        public void Draw(Action<Vector3, Vector3, Color4> drawCallback)
        {
            drawCallback(Center, Extents, new Color4(1.0f, 1.0f, 1.0f));

            if (Children != null)
            {
                for (var i = 0; i < Children.Length; ++i)
                {
                    if(Children[i] != null)
                        Children[i].Draw(drawCallback);
                }
            }

            for (var i = 0; i < Objects.Count; ++i)
            {
                if (Objects[i] != null)
                    Objects[i].Draw(drawCallback);
            }
        }
    }
}
