namespace UnityEditorInternal
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    internal class QuadTreeNode<T> where T: IBounds
    {
        private const float kSmallestAreaForQuadTreeNode = 10f;
        private Rect m_BoundingRect;
        private List<QuadTreeNode<T>> m_ChildrenNodes;
        private static Color m_DebugBoxFillColor;
        private static Color m_DebugFillColor;
        private static Color m_DebugWireColor;
        private List<T> m_Elements;

        static QuadTreeNode()
        {
            QuadTreeNode<T>.m_DebugFillColor = new Color(1f, 1f, 1f, 0.01f);
            QuadTreeNode<T>.m_DebugWireColor = new Color(1f, 0f, 0f, 0.5f);
            QuadTreeNode<T>.m_DebugBoxFillColor = new Color(1f, 0f, 0f, 0.01f);
        }

        public QuadTreeNode(Rect r)
        {
            this.m_Elements = new List<T>();
            this.m_ChildrenNodes = new List<QuadTreeNode<T>>(4);
            this.m_BoundingRect = r;
        }

        public List<T> ContainedBy(Rect queryArea)
        {
            List<T> list = new List<T>();
            foreach (T local in this.m_Elements)
            {
                if (RectUtils.Contains(local.boundingRect, queryArea))
                {
                    list.Add(local);
                }
                else if (queryArea.Overlaps(local.boundingRect))
                {
                    list.Add(local);
                }
            }
            foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
            {
                if (!node.IsEmpty)
                {
                    if (RectUtils.Contains(node.BoundingRect, queryArea))
                    {
                        list.AddRange(node.ContainedBy(queryArea));
                        return list;
                    }
                    if (RectUtils.Contains(queryArea, node.BoundingRect))
                    {
                        list.AddRange(node.Elements(true));
                    }
                    else if (node.BoundingRect.Overlaps(queryArea))
                    {
                        list.AddRange(node.ContainedBy(queryArea));
                    }
                }
            }
            return list;
        }

        private int Count(bool recursive)
        {
            int count = this.m_Elements.Count;
            if (recursive)
            {
                foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
                {
                    count += node.Count(recursive);
                }
            }
            return count;
        }

        public int CountItemsIncludingChildren()
        {
            return this.Count(true);
        }

        public int CountLocalItems()
        {
            return this.Count(false);
        }

        public void DebugDraw(Vector2 offset)
        {
            HandleUtility.ApplyWireMaterial();
            Rect boundingRect = this.m_BoundingRect;
            boundingRect.x += offset.x;
            boundingRect.y += offset.y;
            Handles.DrawSolidRectangleWithOutline(boundingRect, QuadTreeNode<T>.m_DebugFillColor, QuadTreeNode<T>.m_DebugWireColor);
            foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
            {
                node.DebugDraw(offset);
            }
            foreach (IBounds bounds in this.Elements(false))
            {
                Rect rectangle = bounds.boundingRect;
                rectangle.x += offset.x;
                rectangle.y += offset.y;
                Handles.DrawSolidRectangleWithOutline(rectangle, QuadTreeNode<T>.m_DebugBoxFillColor, Color.yellow);
            }
        }

        private List<T> Elements(bool recursive)
        {
            List<T> list = new List<T>();
            if (recursive)
            {
                foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
                {
                    list.AddRange(node.Elements(recursive));
                }
            }
            list.AddRange(this.m_Elements);
            return list;
        }

        public List<T> GetElements()
        {
            return this.Elements(false);
        }

        public List<T> GetElementsIncludingChildren()
        {
            return this.Elements(true);
        }

        public void Insert(T item)
        {
            if (!RectUtils.Contains(this.m_BoundingRect, item.boundingRect))
            {
                Rect intersection = new Rect();
                if (!RectUtils.Intersection(item.boundingRect, this.m_BoundingRect, out intersection))
                {
                    return;
                }
            }
            if (this.m_ChildrenNodes.Count == 0)
            {
                this.Subdivide();
            }
            foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
            {
                if (RectUtils.Contains(node.BoundingRect, item.boundingRect))
                {
                    node.Insert(item);
                    return;
                }
            }
            this.m_Elements.Add(item);
        }

        public List<T> IntersectsWith(Rect queryArea)
        {
            List<T> list = new List<T>();
            foreach (T local in this.m_Elements)
            {
                if (RectUtils.Intersects(local.boundingRect, queryArea))
                {
                    list.Add(local);
                }
            }
            foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
            {
                if (!node.IsEmpty && RectUtils.Intersects(node.BoundingRect, queryArea))
                {
                    list.AddRange(node.IntersectsWith(queryArea));
                    return list;
                }
            }
            return list;
        }

        public void Remove(T item)
        {
            this.m_Elements.Remove(item);
            foreach (QuadTreeNode<T> node in this.m_ChildrenNodes)
            {
                node.Remove(item);
            }
        }

        private void Subdivide()
        {
            if ((this.m_BoundingRect.height * this.m_BoundingRect.width) > 10f)
            {
                float width = this.m_BoundingRect.width / 2f;
                float height = this.m_BoundingRect.height / 2f;
                this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.position.x, this.m_BoundingRect.position.y, width, height)));
                this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.xMin, this.m_BoundingRect.yMin + height, width, height)));
                this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.xMin + width, this.m_BoundingRect.yMin, width, height)));
                this.m_ChildrenNodes.Add(new QuadTreeNode<T>(new Rect(this.m_BoundingRect.xMin + width, this.m_BoundingRect.yMin + height, width, height)));
            }
        }

        public Rect BoundingRect
        {
            get
            {
                return this.m_BoundingRect;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (((this.m_BoundingRect.width == 0f) && (this.m_BoundingRect.height == 0f)) || (this.m_ChildrenNodes.Count == 0));
            }
        }
    }
}

