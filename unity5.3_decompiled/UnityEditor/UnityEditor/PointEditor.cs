namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class PointEditor
    {
        [CompilerGenerated]
        private static Func<KeyValuePair<int, float>, float> <>f__am$cache5;
        private static bool s_DidDrag;
        private static Quaternion s_EditingRotation = Quaternion.identity;
        private static Vector3 s_EditingScale = Vector3.one;
        private static List<int> s_StartDragSelection;
        private static Vector2 s_StartMouseDragPosition;

        public static void Draw(IEditablePoint points, Transform cloudTransform, List<int> selection, bool twoPassDrawing)
        {
            LightmapVisualization.DrawPointCloud(points.GetUnselectedPositions(), points.GetSelectedPositions(), points.GetDefaultColor(), points.GetSelectedColor(), points.GetPointScale(), cloudTransform);
        }

        public static int FindNearest(Vector2 point, Transform cloudTransform, IEditablePoint points)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(point);
            Dictionary<int, float> source = new Dictionary<int, float>();
            for (int i = 0; i < points.Count; i++)
            {
                float t = 0f;
                Vector3 zero = Vector3.zero;
                if (MathUtils.IntersectRaySphere(ray, cloudTransform.TransformPoint(points.GetPosition(i)), points.GetPointScale() * 0.5f, ref t, ref zero) && (t > 0f))
                {
                    source.Add(i, t);
                }
            }
            if (source.Count <= 0)
            {
                return -1;
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = x => x.Value;
            }
            return source.OrderBy<KeyValuePair<int, float>, float>(<>f__am$cache5).First<KeyValuePair<int, float>>().Key;
        }

        private static Rect FromToRect(Vector2 from, Vector2 to)
        {
            Rect rect = new Rect(from.x, from.y, to.x - from.x, to.y - from.y);
            if (rect.width < 0f)
            {
                rect.x += rect.width;
                rect.width = -rect.width;
            }
            if (rect.height < 0f)
            {
                rect.y += rect.height;
                rect.height = -rect.height;
            }
            return rect;
        }

        public static bool MovePoints(IEditablePoint points, Transform cloudTransform, List<int> selection)
        {
            <MovePoints>c__AnonStorey3D storeyd = new <MovePoints>c__AnonStorey3D {
                points = points
            };
            if (selection.Count != 0)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    s_EditingScale = Vector3.one;
                    s_EditingRotation = Quaternion.identity;
                }
                if (Camera.current != null)
                {
                    Vector3 zero = Vector3.zero;
                    zero = (Tools.pivotMode != PivotMode.Pivot) ? ((Vector3) (selection.Aggregate<int, Vector3>(zero, new Func<Vector3, int, Vector3>(storeyd.<>m__55)) / ((float) selection.Count))) : storeyd.points.GetPosition(selection[0]);
                    zero = cloudTransform.TransformPoint(zero);
                    switch (Tools.current)
                    {
                        case Tool.Move:
                        {
                            Vector3 position = Handles.PositionHandle(zero, (Tools.pivotRotation != PivotRotation.Local) ? Quaternion.identity : cloudTransform.rotation);
                            if (!GUI.changed)
                            {
                                break;
                            }
                            Vector3 vector3 = cloudTransform.InverseTransformPoint(position) - cloudTransform.InverseTransformPoint(zero);
                            foreach (int num in selection)
                            {
                                storeyd.points.SetPosition(num, storeyd.points.GetPosition(num) + vector3);
                            }
                            return true;
                        }
                        case Tool.Rotate:
                        {
                            Quaternion quaternion = Handles.RotationHandle(s_EditingRotation, zero);
                            if (!GUI.changed)
                            {
                                break;
                            }
                            Vector3 vector4 = cloudTransform.InverseTransformPoint(zero);
                            foreach (int num2 in selection)
                            {
                                Vector3 vector5 = storeyd.points.GetPosition(num2) - vector4;
                                vector5 = (Vector3) (Quaternion.Inverse(s_EditingRotation) * vector5);
                                vector5 = (Vector3) (quaternion * vector5);
                                vector5 += vector4;
                                storeyd.points.SetPosition(num2, vector5);
                            }
                            s_EditingRotation = quaternion;
                            return true;
                        }
                        case Tool.Scale:
                        {
                            Vector3 vector6 = Handles.ScaleHandle(s_EditingScale, zero, Quaternion.identity, HandleUtility.GetHandleSize(zero));
                            if (GUI.changed)
                            {
                                Vector3 vector7 = cloudTransform.InverseTransformPoint(zero);
                                foreach (int num3 in selection)
                                {
                                    Vector3 vector8 = storeyd.points.GetPosition(num3) - vector7;
                                    vector8.x /= s_EditingScale.x;
                                    vector8.y /= s_EditingScale.y;
                                    vector8.z /= s_EditingScale.z;
                                    vector8.x *= vector6.x;
                                    vector8.y *= vector6.y;
                                    vector8.z *= vector6.z;
                                    vector8 += vector7;
                                    storeyd.points.SetPosition(num3, vector8);
                                }
                                s_EditingScale = vector6;
                                return true;
                            }
                            break;
                        }
                    }
                }
            }
            return false;
        }

        public static bool SelectPoints(IEditablePoint points, Transform cloudTransform, ref List<int> selection, bool firstSelect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.alt && (Event.current.type != EventType.Repaint))
            {
                return false;
            }
            bool flag = false;
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == controlID) || firstSelect) && (current.button == 0))
                    {
                        if (!current.shift && !EditorGUI.actionKey)
                        {
                            selection.Clear();
                            flag = true;
                        }
                        GUIUtility.hotControl = controlID;
                        s_StartMouseDragPosition = current.mousePosition;
                        s_StartDragSelection = new List<int>(selection);
                        current.Use();
                    }
                    goto Label_0289;

                case EventType.MouseUp:
                {
                    if ((GUIUtility.hotControl != controlID) || (current.button != 0))
                    {
                        goto Label_0289;
                    }
                    if (s_DidDrag)
                    {
                        goto Label_020C;
                    }
                    int item = FindNearest(s_StartMouseDragPosition, cloudTransform, points);
                    if (item != -1)
                    {
                        if (current.shift || EditorGUI.actionKey)
                        {
                            int index = selection.IndexOf(item);
                            if (index != -1)
                            {
                                selection.RemoveAt(index);
                            }
                            else
                            {
                                selection.Add(item);
                            }
                            break;
                        }
                        selection.Add(item);
                    }
                    break;
                }
                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        s_DidDrag = true;
                        selection.Clear();
                        selection.AddRange(s_StartDragSelection);
                        Rect rect = FromToRect(s_StartMouseDragPosition, current.mousePosition);
                        Matrix4x4 matrix = Handles.matrix;
                        Handles.matrix = cloudTransform.localToWorldMatrix;
                        for (int i = 0; i < points.Count; i++)
                        {
                            Vector2 point = HandleUtility.WorldToGUIPoint(points.GetPosition(i));
                            if (rect.Contains(point))
                            {
                                selection.Add(i);
                            }
                        }
                        Handles.matrix = matrix;
                        GUI.changed = true;
                        current.Use();
                    }
                    goto Label_0289;

                case EventType.Repaint:
                    if ((GUIUtility.hotControl == controlID) && (current.mousePosition != s_StartMouseDragPosition))
                    {
                        GUIStyle style = "SelectionRect";
                        Handles.BeginGUI();
                        style.Draw(FromToRect(s_StartMouseDragPosition, current.mousePosition), false, false, false, false);
                        Handles.EndGUI();
                    }
                    goto Label_0289;

                case EventType.Layout:
                    HandleUtility.AddDefaultControl(controlID);
                    goto Label_0289;

                default:
                    goto Label_0289;
            }
            GUI.changed = true;
            flag = true;
        Label_020C:
            s_StartDragSelection = null;
            s_StartMouseDragPosition = Vector2.zero;
            s_DidDrag = false;
            GUIUtility.hotControl = 0;
            current.Use();
        Label_0289:
            selection = selection.Distinct<int>().ToList<int>();
            return flag;
        }

        [CompilerGenerated]
        private sealed class <MovePoints>c__AnonStorey3D
        {
            internal IEditablePoint points;

            internal Vector3 <>m__55(Vector3 current, int index)
            {
                return (current + this.points.GetPosition(index));
            }
        }
    }
}

