namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SnapEdge
    {
        public EdgeDir dir;
        public float end;
        private const float kSnapDist = 0f;
        public Object m_Object;
        public float pos;
        public float start;
        public float startDragPos;
        public float startDragStart;

        internal SnapEdge(Object win, EdgeDir _d, float _p, float _s, float _e)
        {
            this.dir = _d;
            this.m_Object = win;
            this.pos = _p;
            this.start = _s;
            this.end = _e;
        }

        internal void ApplyOffset(Vector2 offset, bool windowMove)
        {
            offset = ((this.dir != EdgeDir.Left) && (this.dir != EdgeDir.Right)) ? new Vector2(offset.y, offset.x) : offset;
            if (windowMove)
            {
                this.pos += offset.x;
            }
            else
            {
                this.pos = offset.x + this.startDragPos;
            }
            this.start += offset.y;
            this.end += offset.y;
        }

        private int EdgeCoordinateIndex()
        {
            if (((this.dir != EdgeDir.Left) && (this.dir != EdgeDir.Right)) && (this.dir != EdgeDir.CenterX))
            {
                return 1;
            }
            return 0;
        }

        private static bool EdgeInside(SnapEdge edge, List<SnapEdge> frustum)
        {
            foreach (SnapEdge edge2 in frustum)
            {
                if (!ShouldEdgesSnap(edge, edge2))
                {
                    return false;
                }
            }
            return true;
        }

        internal static EdgeDir OppositeEdge(EdgeDir dir)
        {
            switch (dir)
            {
                case EdgeDir.Left:
                    return EdgeDir.Right;

                case EdgeDir.Right:
                    return EdgeDir.Left;

                case EdgeDir.CenterX:
                    return EdgeDir.CenterX;

                case EdgeDir.Up:
                    return EdgeDir.Down;

                case EdgeDir.Down:
                    return EdgeDir.Up;
            }
            return EdgeDir.CenterY;
        }

        private static bool ShouldEdgesSnap(SnapEdge a, SnapEdge b)
        {
            return ((((a.dir == EdgeDir.CenterX) || (a.dir == EdgeDir.CenterY)) && (a.dir == b.dir)) || ((a.dir == OppositeEdge(b.dir)) && ((a.start <= b.end) && (a.end >= b.start))));
        }

        internal static Vector2 Snap(List<SnapEdge> sourceEdges, List<SnapEdge> edgesToSnapAgainst, List<KeyValuePair<SnapEdge, SnapEdge>>[] activeEdges)
        {
            Vector2 zero = Vector2.zero;
            float num = 10f;
            activeEdges[0].Clear();
            activeEdges[1].Clear();
            float[] numArray = new float[] { num, num };
            float[] numArray2 = new float[2];
            foreach (SnapEdge edge in sourceEdges)
            {
                int index = edge.EdgeCoordinateIndex();
                Snap(edge, edgesToSnapAgainst, ref numArray[index], ref numArray2[index], activeEdges[index]);
            }
            zero.x = numArray2[0];
            zero.y = numArray2[1];
            return zero;
        }

        internal static void Snap(SnapEdge edge, List<SnapEdge> otherEdges, ref float maxDist, ref float snapVal, List<KeyValuePair<SnapEdge, SnapEdge>> activeEdges)
        {
            foreach (SnapEdge edge2 in otherEdges)
            {
                if (ShouldEdgesSnap(edge, edge2))
                {
                    float num = Mathf.Abs((float) (edge2.pos - edge.pos));
                    if (num < maxDist)
                    {
                        maxDist = num;
                        snapVal = edge2.pos - edge.pos;
                        activeEdges.Clear();
                        activeEdges.Add(new KeyValuePair<SnapEdge, SnapEdge>(edge, edge2));
                    }
                    else if (num == maxDist)
                    {
                        activeEdges.Add(new KeyValuePair<SnapEdge, SnapEdge>(edge, edge2));
                    }
                }
            }
        }

        public override string ToString()
        {
            if (this.m_Object != null)
            {
                object[] objArray1 = new object[] { "Edge: ", this.dir, " of ", this.m_Object.name, "    pos: ", this.pos, " (", this.start, " - ", this.end, ")" };
                return string.Concat(objArray1);
            }
            return ("Edge: " + this.dir + " of NULL - something is wrong!");
        }

        public enum EdgeDir
        {
            Left,
            Right,
            CenterX,
            Up,
            Down,
            CenterY,
            None
        }
    }
}

