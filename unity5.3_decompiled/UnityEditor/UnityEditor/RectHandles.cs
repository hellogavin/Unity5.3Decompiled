namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class RectHandles
    {
        private static Vector2 s_CurrentMousePosition;
        private static int s_LastCursorId = 0;
        private static float s_RotationDist;
        private static Vector2 s_StartMousePosition;
        private static Vector3 s_StartPosition;
        private static float s_StartRotation;
        private static Styles s_Styles;
        private static Vector3[] s_TempVectors = new Vector3[0];

        public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
        {
            dirA = Vector3.ProjectOnPlane(dirA, axis);
            dirB = Vector3.ProjectOnPlane(dirB, axis);
            return (Vector3.Angle(dirA, dirB) * ((Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) >= 0f) ? ((float) 1) : ((float) (-1))));
        }

        internal static Vector3 CornerSlider(int id, Vector3 cornerPos, Vector3 handleDir, Vector3 outwardsDir1, Vector3 outwardsDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap)
        {
            Event current = Event.current;
            Vector3 vector = Handles.Slider2D(id, cornerPos, handleDir, outwardsDir1, outwardsDir2, handleSize, drawFunc, snap);
            EventType type = current.type;
            if (type != EventType.MouseMove)
            {
                if (type != EventType.Repaint)
                {
                    return vector;
                }
            }
            else
            {
                DetectCursorChange(id);
                return vector;
            }
            if (((HandleUtility.nearestControl == id) && (GUIUtility.hotControl == 0)) || (GUIUtility.hotControl == id))
            {
                HandleDirectionalCursor(cornerPos, handleDir, outwardsDir1 + outwardsDir2);
            }
            return vector;
        }

        internal static void DetectCursorChange(int id)
        {
            if (HandleUtility.nearestControl == id)
            {
                s_LastCursorId = id;
                Event.current.Use();
            }
            else if (s_LastCursorId == id)
            {
                s_LastCursorId = 0;
                Event.current.Use();
            }
        }

        public static void DrawDottedLineWithShadow(Color shadowColor, Vector2 screenOffset, Vector3 p1, Vector3 p2, float screenSpaceSize)
        {
            Camera current = Camera.current;
            if ((current != null) && (Event.current.type == EventType.Repaint))
            {
                Color color = Handles.color;
                shadowColor.a *= color.a;
                Handles.color = shadowColor;
                Handles.DrawDottedLine(current.ScreenToWorldPoint(current.WorldToScreenPoint(p1) + screenOffset), current.ScreenToWorldPoint(current.WorldToScreenPoint(p2) + screenOffset), screenSpaceSize);
                Handles.color = color;
                Handles.DrawDottedLine(p1, p2, screenSpaceSize);
            }
        }

        private static void DrawImageBasedCap(int controlID, Vector3 position, Quaternion rotation, float size, GUIStyle normal, GUIStyle active)
        {
            if ((Camera.current == null) || (Vector3.Dot(position - Camera.current.transform.position, Camera.current.transform.forward) >= 0f))
            {
                Vector3 vector = (Vector3) HandleUtility.WorldToGUIPoint(position);
                Handles.BeginGUI();
                float fixedWidth = normal.fixedWidth;
                float fixedHeight = normal.fixedHeight;
                Rect rect = new Rect(vector.x - (fixedWidth / 2f), vector.y - (fixedHeight / 2f), fixedWidth, fixedHeight);
                if (GUIUtility.hotControl == controlID)
                {
                    active.Draw(rect, GUIContent.none, controlID);
                }
                else
                {
                    normal.Draw(rect, GUIContent.none, controlID);
                }
                Handles.EndGUI();
            }
        }

        public static void DrawPolyLineWithShadow(Color shadowColor, Vector2 screenOffset, params Vector3[] points)
        {
            Camera current = Camera.current;
            if ((current != null) && (Event.current.type == EventType.Repaint))
            {
                if (s_TempVectors.Length != points.Length)
                {
                    s_TempVectors = new Vector3[points.Length];
                }
                for (int i = 0; i < points.Length; i++)
                {
                    s_TempVectors[i] = current.ScreenToWorldPoint(current.WorldToScreenPoint(points[i]) + screenOffset);
                }
                Color color = Handles.color;
                shadowColor.a *= color.a;
                Handles.color = shadowColor;
                Handles.DrawPolyLine(s_TempVectors);
                Handles.color = color;
                Handles.DrawPolyLine(points);
            }
        }

        private static MouseCursor GetScaleCursor(Vector2 direction)
        {
            float num = Mathf.Atan2(direction.x, direction.y) * 57.29578f;
            if (num < 0f)
            {
                num = 360f + num;
            }
            if (num >= 27.5f)
            {
                if (num < 72.5f)
                {
                    return MouseCursor.ResizeUpRight;
                }
                if (num < 117.5f)
                {
                    return MouseCursor.ResizeHorizontal;
                }
                if (num < 162.5f)
                {
                    return MouseCursor.ResizeUpLeft;
                }
                if (num < 207.5f)
                {
                    return MouseCursor.ResizeVertical;
                }
                if (num < 252.5f)
                {
                    return MouseCursor.ResizeUpRight;
                }
                if (num < 297.5f)
                {
                    return MouseCursor.ResizeHorizontal;
                }
                if (num < 342.5f)
                {
                    return MouseCursor.ResizeUpLeft;
                }
            }
            return MouseCursor.ResizeVertical;
        }

        private static void HandleDirectionalCursor(Vector3 handlePosition, Vector3 handlePlaneNormal, Vector3 direction)
        {
            Vector3 vector2;
            Vector2 mousePosition = Event.current.mousePosition;
            Plane plane = new Plane(handlePlaneNormal, handlePosition);
            if (RaycastGUIPointToWorldHit(mousePosition, plane, out vector2))
            {
                Vector2 vector3 = WorldToScreenSpaceDir(vector2, direction);
                Rect position = new Rect(mousePosition.x - 100f, mousePosition.y - 100f, 200f, 200f);
                EditorGUIUtility.AddCursorRect(position, GetScaleCursor(vector3));
            }
        }

        public static void PivotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            DrawImageBasedCap(controlID, position, rotation, size, s_Styles.pivotdot, s_Styles.pivotdotactive);
        }

        internal static bool RaycastGUIPointToWorldHit(Vector2 guiPoint, Plane plane, out Vector3 hit)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiPoint);
            float enter = 0f;
            bool flag = plane.Raycast(ray, out enter);
            hit = !flag ? Vector3.zero : ray.GetPoint(enter);
            return flag;
        }

        public static void RectScalingCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            DrawImageBasedCap(controlID, position, rotation, size, s_Styles.dragdot, s_Styles.dragdotactive);
        }

        public static void RenderRectWithShadow(bool active, params Vector3[] corners)
        {
            Vector3[] points = new Vector3[] { corners[0], corners[1], corners[2], corners[3], corners[0] };
            Color color = Handles.color;
            Handles.color = new Color(1f, 1f, 1f, !active ? 0.5f : 1f);
            DrawPolyLineWithShadow(new Color(0f, 0f, 0f, !active ? 0.5f : 1f), new Vector2(1f, -1f), points);
            Handles.color = color;
        }

        public static float RotationSlider(int id, Vector3 cornerPos, float rotation, Vector3 pivot, Vector3 handleDir, Vector3 outwardsDir1, Vector3 outwardsDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap)
        {
            Vector3 vector = outwardsDir1 + outwardsDir2;
            Vector2 vector2 = HandleUtility.WorldToGUIPoint(cornerPos);
            Vector2 vector3 = HandleUtility.WorldToGUIPoint(cornerPos + vector) - vector2;
            vector3 = (Vector2) (vector3.normalized * 15f);
            RaycastGUIPointToWorldHit(vector2 + vector3, new Plane(handleDir, cornerPos), out cornerPos);
            Event current = Event.current;
            Vector3 vector4 = Handles.Slider2D(id, cornerPos, handleDir, outwardsDir1, outwardsDir2, handleSize, drawFunc, Vector2.zero);
            if (current.type == EventType.MouseMove)
            {
                DetectCursorChange(id);
            }
            if ((current.type == EventType.Repaint) && (((HandleUtility.nearestControl == id) && (GUIUtility.hotControl == 0)) || (GUIUtility.hotControl == id)))
            {
                Rect position = new Rect(current.mousePosition.x - 100f, current.mousePosition.y - 100f, 200f, 200f);
                EditorGUIUtility.AddCursorRect(position, MouseCursor.RotateArrow);
            }
            return (rotation - AngleAroundAxis(vector4 - pivot, cornerPos - pivot, handleDir));
        }

        internal static Vector3 SideSlider(int id, Vector3 position, Vector3 sideVector, Vector3 direction, float size, Handles.DrawCapFunction drawFunc, float snap)
        {
            return SideSlider(id, position, sideVector, direction, size, drawFunc, snap, 0f);
        }

        internal static Vector3 SideSlider(int id, Vector3 position, Vector3 sideVector, Vector3 direction, float size, Handles.DrawCapFunction drawFunc, float snap, float bias)
        {
            Event current = Event.current;
            Vector3 normalized = Vector3.Cross(sideVector, direction).normalized;
            Vector3 vector2 = Handles.Slider2D(id, position, normalized, direction, sideVector, 0f, drawFunc, (Vector2) (Vector2.one * snap));
            vector2 = position + Vector3.Project(vector2 - position, direction);
            EventType type = current.type;
            if (type != EventType.Repaint)
            {
                if (type != EventType.Layout)
                {
                    if (type != EventType.MouseMove)
                    {
                        return vector2;
                    }
                }
                else
                {
                    Vector3 vector3 = sideVector.normalized;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToLine((Vector3) ((position + (sideVector * 0.5f)) - ((vector3 * size) * 2f)), (Vector3) ((position - (sideVector * 0.5f)) + ((vector3 * size) * 2f))) - bias);
                    return vector2;
                }
                DetectCursorChange(id);
                return vector2;
            }
            if (((HandleUtility.nearestControl == id) && (GUIUtility.hotControl == 0)) || (GUIUtility.hotControl == id))
            {
                HandleDirectionalCursor(position, normalized, direction);
            }
            return vector2;
        }

        private static Vector2 WorldToScreenSpaceDir(Vector3 worldPos, Vector3 worldDir)
        {
            Vector3 vector = (Vector3) HandleUtility.WorldToGUIPoint(worldPos);
            Vector2 vector3 = HandleUtility.WorldToGUIPoint(worldPos + worldDir) - vector;
            vector3.y *= -1f;
            return vector3;
        }

        private class Styles
        {
            public readonly GUIStyle dragdot = "U2D.dragDot";
            public readonly GUIStyle dragdotactive = "U2D.dragDotActive";
            public readonly GUIStyle pivotdot = "U2D.pivotDot";
            public readonly GUIStyle pivotdotactive = "U2D.pivotDotActive";
        }
    }
}

