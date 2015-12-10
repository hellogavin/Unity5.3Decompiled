namespace UnityEditorInternal
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class RectUtils
    {
        public static bool Contains(Rect a, Rect b)
        {
            if (a.xMin > b.xMin)
            {
                return false;
            }
            if (a.xMax < b.xMax)
            {
                return false;
            }
            if (a.yMin > b.yMin)
            {
                return false;
            }
            if (a.yMax < b.yMax)
            {
                return false;
            }
            return true;
        }

        public static Rect Encompass(Rect a, Rect b)
        {
            Rect rect = a;
            rect.xMin = Math.Min(a.xMin, b.xMin);
            rect.yMin = Math.Min(a.yMin, b.yMin);
            rect.xMax = Math.Max(a.xMax, b.xMax);
            rect.yMax = Math.Max(a.yMax, b.yMax);
            return rect;
        }

        public static Rect Inflate(Rect a, float factor)
        {
            return Inflate(a, factor, factor);
        }

        public static Rect Inflate(Rect a, float factorX, float factorY)
        {
            float num = a.width * factorX;
            float num2 = a.height * factorY;
            float num3 = (num - a.width) / 2f;
            float num4 = (num2 - a.height) / 2f;
            Rect rect = a;
            rect.xMin -= num3;
            rect.yMin -= num4;
            rect.xMax += num3;
            rect.yMax += num4;
            return rect;
        }

        public static bool Intersection(Rect r1, Rect r2, out Rect intersection)
        {
            if (!r1.Overlaps(r2) && !r2.Overlaps(r1))
            {
                intersection = new Rect(0f, 0f, 0f, 0f);
                return false;
            }
            float x = Mathf.Max(r1.xMin, r2.xMin);
            float y = Mathf.Max(r1.yMin, r2.yMin);
            float num3 = Mathf.Min(r1.xMax, r2.xMax);
            float num4 = Mathf.Min(r1.yMax, r2.yMax);
            intersection = new Rect(x, y, num3 - x, num4 - y);
            return true;
        }

        public static bool Intersects(Rect r1, Rect r2)
        {
            if (!r1.Overlaps(r2) && !r2.Overlaps(r1))
            {
                return false;
            }
            return true;
        }

        public static bool IntersectsSegment(Rect rect, Vector2 p1, Vector2 p2)
        {
            float xMin = Mathf.Min(p1.x, p2.x);
            float xMax = Mathf.Max(p1.x, p2.x);
            if (xMax > rect.xMax)
            {
                xMax = rect.xMax;
            }
            if (xMin < rect.xMin)
            {
                xMin = rect.xMin;
            }
            if (xMin > xMax)
            {
                return false;
            }
            float yMin = Mathf.Min(p1.y, p2.y);
            float yMax = Mathf.Max(p1.y, p2.y);
            float f = p2.x - p1.x;
            if (Mathf.Abs(f) > 1E-07f)
            {
                float num6 = (p2.y - p1.y) / f;
                float num7 = p1.y - (num6 * p1.x);
                yMin = (num6 * xMin) + num7;
                yMax = (num6 * xMax) + num7;
            }
            if (yMin > yMax)
            {
                float num8 = yMax;
                yMax = yMin;
                yMin = num8;
            }
            if (yMax > rect.yMax)
            {
                yMax = rect.yMax;
            }
            if (yMin < rect.yMin)
            {
                yMin = rect.yMin;
            }
            if (yMin > yMax)
            {
                return false;
            }
            return true;
        }

        public static Rect Move(Rect r, Vector2 delta)
        {
            Rect rect = r;
            rect.xMin += delta.x;
            rect.yMin += delta.y;
            rect.xMax += delta.x;
            rect.yMax += delta.y;
            return rect;
        }

        public static Rect Offset(Rect a, Rect b)
        {
            Rect rect = a;
            rect.xMin += b.xMin;
            rect.yMin += b.yMin;
            return rect;
        }

        public static Rect Offset(Rect r, float offsetX, float offsetY)
        {
            Rect rect = r;
            rect.xMin += offsetX;
            rect.yMin += offsetY;
            return rect;
        }

        public static Rect OffsetX(Rect r, float offsetX)
        {
            return Offset(r, offsetX, 0f);
        }
    }
}

