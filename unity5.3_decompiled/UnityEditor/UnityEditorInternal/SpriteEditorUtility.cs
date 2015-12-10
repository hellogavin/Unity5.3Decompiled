namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal static class SpriteEditorUtility
    {
        public static void BeginLines(Color color)
        {
            HandleUtility.ApplyWireMaterial();
            GL.PushMatrix();
            GL.MultMatrix(Handles.matrix);
            GL.Begin(1);
            GL.Color(color);
        }

        public static Rect ClampedRect(Rect rect, Rect clamp, bool maintainSize)
        {
            Rect rect2 = new Rect(rect);
            if (maintainSize)
            {
                Vector2 center = rect.center;
                if ((center.x + (Mathf.Abs(rect.width) * 0.5f)) > clamp.xMax)
                {
                    center.x = clamp.xMax - (rect.width * 0.5f);
                }
                if ((center.x - (Mathf.Abs(rect.width) * 0.5f)) < clamp.xMin)
                {
                    center.x = clamp.xMin + (rect.width * 0.5f);
                }
                if ((center.y + (Mathf.Abs(rect.height) * 0.5f)) > clamp.yMax)
                {
                    center.y = clamp.yMax - (rect.height * 0.5f);
                }
                if ((center.y - (Mathf.Abs(rect.height) * 0.5f)) < clamp.yMin)
                {
                    center.y = clamp.yMin + (rect.height * 0.5f);
                }
                rect2.center = center;
            }
            else
            {
                if (rect2.width > 0f)
                {
                    rect2.xMin = Mathf.Max(rect.xMin, clamp.xMin);
                    rect2.xMax = Mathf.Min(rect.xMax, clamp.xMax);
                }
                else
                {
                    rect2.xMin = Mathf.Min(rect.xMin, clamp.xMax);
                    rect2.xMax = Mathf.Max(rect.xMax, clamp.xMin);
                }
                if (rect2.height > 0f)
                {
                    rect2.yMin = Mathf.Max(rect.yMin, clamp.yMin);
                    rect2.yMax = Mathf.Min(rect.yMax, clamp.yMax);
                }
                else
                {
                    rect2.yMin = Mathf.Min(rect.yMin, clamp.yMax);
                    rect2.yMax = Mathf.Max(rect.yMax, clamp.yMin);
                }
            }
            rect2.width = Mathf.Abs(rect2.width);
            rect2.height = Mathf.Abs(rect2.height);
            return rect2;
        }

        public static void DrawBox(Rect position)
        {
            Vector3[] vectorArray = new Vector3[5];
            int num = 0;
            vectorArray[num++] = new Vector3(position.xMin, position.yMin, 0f);
            vectorArray[num++] = new Vector3(position.xMax, position.yMin, 0f);
            vectorArray[num++] = new Vector3(position.xMax, position.yMax, 0f);
            vectorArray[num++] = new Vector3(position.xMin, position.yMax, 0f);
            DrawLine(vectorArray[0], vectorArray[1]);
            DrawLine(vectorArray[1], vectorArray[2]);
            DrawLine(vectorArray[2], vectorArray[3]);
            DrawLine(vectorArray[3], vectorArray[0]);
        }

        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            GL.Vertex(p1);
            GL.Vertex(p2);
        }

        public static void EndLines()
        {
            GL.End();
            GL.PopMatrix();
        }

        public static Vector2 GetPivotValue(SpriteAlignment alignment, Vector2 customOffset)
        {
            switch (alignment)
            {
                case SpriteAlignment.Center:
                    return new Vector2(0.5f, 0.5f);

                case SpriteAlignment.TopLeft:
                    return new Vector2(0f, 1f);

                case SpriteAlignment.TopCenter:
                    return new Vector2(0.5f, 1f);

                case SpriteAlignment.TopRight:
                    return new Vector2(1f, 1f);

                case SpriteAlignment.LeftCenter:
                    return new Vector2(0f, 0.5f);

                case SpriteAlignment.RightCenter:
                    return new Vector2(1f, 0.5f);

                case SpriteAlignment.BottomLeft:
                    return new Vector2(0f, 0f);

                case SpriteAlignment.BottomCenter:
                    return new Vector2(0.5f, 0f);

                case SpriteAlignment.BottomRight:
                    return new Vector2(1f, 0f);

                case SpriteAlignment.Custom:
                    return customOffset;
            }
            return Vector2.zero;
        }

        public static Rect RoundedRect(Rect rect)
        {
            return new Rect((float) Mathf.RoundToInt(rect.xMin), (float) Mathf.RoundToInt(rect.yMin), (float) Mathf.RoundToInt(rect.width), (float) Mathf.RoundToInt(rect.height));
        }

        public static Rect RoundToInt(Rect r)
        {
            r.xMin = Mathf.RoundToInt(r.xMin);
            r.yMin = Mathf.RoundToInt(r.yMin);
            r.xMax = Mathf.RoundToInt(r.xMax);
            r.yMax = Mathf.RoundToInt(r.yMax);
            return r;
        }
    }
}

