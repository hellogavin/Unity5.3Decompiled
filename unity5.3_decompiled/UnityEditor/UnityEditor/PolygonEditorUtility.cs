namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PolygonEditorUtility
    {
        private const float k_HandlePickDistance = 50f;
        private const float k_HandlePointSnap = 0.2f;
        private Collider2D m_ActiveCollider;
        private bool m_DeleteMode;
        private bool m_FirstOnSceneGUIAfterReset;
        private bool m_LeftIntersect;
        private bool m_LoopingCollider;
        private int m_MinPathPoints = 3;
        private bool m_RightIntersect;
        private float m_SelectedDistance;
        private float m_SelectedEdgeDistance;
        private int m_SelectedEdgePath = -1;
        private int m_SelectedEdgeVertex0 = -1;
        private int m_SelectedEdgeVertex1 = -1;
        private int m_SelectedPath = -1;
        private int m_SelectedVertex = -1;

        private void ApplyEditing(Collider2D collider)
        {
            PolygonCollider2D colliderd = collider as PolygonCollider2D;
            if (colliderd != null)
            {
                PolygonEditor.ApplyEditing(colliderd);
            }
            else
            {
                EdgeCollider2D colliderd2 = collider as EdgeCollider2D;
                if (colliderd2 == null)
                {
                    throw new NotImplementedException(string.Format("PolygonEditorUtility does not support {0}", collider));
                }
                PolygonEditor.ApplyEditing(colliderd2);
            }
        }

        private bool DeleteCommandEvent(Event evt)
        {
            return (((evt.type == EventType.ExecuteCommand) || (evt.type == EventType.ValidateCommand)) && ((evt.commandName == "Delete") || (evt.commandName == "SoftDelete")));
        }

        private void DrawEdgesForSelectedPoint(Vector3 worldPos, Transform transform, bool leftIntersect, bool rightIntersect, bool loop)
        {
            Vector2 vector2;
            Vector2 vector3;
            bool flag = true;
            bool flag2 = true;
            int pointCount = PolygonEditor.GetPointCount(this.m_SelectedPath);
            int pointIndex = this.m_SelectedVertex - 1;
            if (pointIndex == -1)
            {
                pointIndex = pointCount - 1;
                flag = loop;
            }
            int num3 = this.m_SelectedVertex + 1;
            if (num3 == pointCount)
            {
                num3 = 0;
                flag2 = loop;
            }
            Vector2 offset = this.m_ActiveCollider.offset;
            PolygonEditor.GetPoint(this.m_SelectedPath, pointIndex, out vector2);
            PolygonEditor.GetPoint(this.m_SelectedPath, num3, out vector3);
            vector2 += offset;
            vector3 += offset;
            Vector3 vector4 = transform.TransformPoint((Vector3) vector2);
            Vector3 vector5 = transform.TransformPoint((Vector3) vector3);
            vector4.z = vector5.z = worldPos.z;
            float width = 4f;
            if (flag)
            {
                Handles.color = (!leftIntersect && !this.m_DeleteMode) ? Color.green : Color.red;
                Vector3[] points = new Vector3[] { worldPos, vector4 };
                Handles.DrawAAPolyLine(width, points);
            }
            if (flag2)
            {
                Handles.color = (!rightIntersect && !this.m_DeleteMode) ? Color.green : Color.red;
                Vector3[] vectorArray2 = new Vector3[] { worldPos, vector5 };
                Handles.DrawAAPolyLine(width, vectorArray2);
            }
            Handles.color = Color.white;
        }

        private Vector2 GetNearestPointOnEdge(Vector2 point, Vector2 start, Vector2 end)
        {
            Vector2 rhs = point - start;
            Vector2 vector4 = end - start;
            Vector2 normalized = vector4.normalized;
            float num = Vector2.Dot(normalized, rhs);
            if (num <= 0f)
            {
                return start;
            }
            if (num >= Vector2.Distance(start, end))
            {
                return end;
            }
            Vector2 vector3 = (Vector2) (normalized * num);
            return (start + vector3);
        }

        public void OnSceneGUI()
        {
            if ((this.m_ActiveCollider != null) && !Tools.viewToolActive)
            {
                float num;
                Vector2 offset = this.m_ActiveCollider.offset;
                Event current = Event.current;
                this.m_DeleteMode = current.command || current.control;
                Transform transform = this.m_ActiveCollider.transform;
                GUIUtility.keyboardControl = 0;
                HandleUtility.s_CustomPickDistance = 50f;
                Plane plane = new Plane(-transform.forward, Vector3.zero);
                Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                plane.Raycast(ray, out num);
                Vector3 point = ray.GetPoint(num);
                Vector2 vector3 = transform.InverseTransformPoint(point);
                if ((current.type == EventType.MouseMove) || this.m_FirstOnSceneGUIAfterReset)
                {
                    int num2;
                    int num3;
                    int num4;
                    float num5;
                    if (PolygonEditor.GetNearestPoint(vector3 - offset, out num2, out num3, out num5))
                    {
                        this.m_SelectedPath = num2;
                        this.m_SelectedVertex = num3;
                        this.m_SelectedDistance = num5;
                    }
                    else
                    {
                        this.m_SelectedPath = -1;
                    }
                    if (PolygonEditor.GetNearestEdge(vector3 - offset, out num2, out num3, out num4, out num5, this.m_LoopingCollider))
                    {
                        this.m_SelectedEdgePath = num2;
                        this.m_SelectedEdgeVertex0 = num3;
                        this.m_SelectedEdgeVertex1 = num4;
                        this.m_SelectedEdgeDistance = num5;
                    }
                    else
                    {
                        this.m_SelectedEdgePath = -1;
                    }
                    if (current.type == EventType.MouseMove)
                    {
                        current.Use();
                    }
                }
                else if (current.type == EventType.MouseUp)
                {
                    this.m_LeftIntersect = false;
                    this.m_RightIntersect = false;
                }
                bool flag = false;
                bool flag2 = false;
                if ((this.m_SelectedPath != -1) && (this.m_SelectedEdgePath != -1))
                {
                    Vector2 vector4;
                    PolygonEditor.GetPoint(this.m_SelectedPath, this.m_SelectedVertex, out vector4);
                    vector4 += offset;
                    float num6 = HandleUtility.GetHandleSize(transform.TransformPoint((Vector3) vector4)) * 0.2f;
                    flag2 = this.m_SelectedEdgeDistance < (this.m_SelectedDistance - num6);
                    flag = !flag2;
                }
                else if (this.m_SelectedPath != -1)
                {
                    flag = true;
                }
                else if (this.m_SelectedEdgePath != -1)
                {
                    flag2 = true;
                }
                if (this.m_DeleteMode && flag2)
                {
                    flag2 = false;
                    flag = true;
                }
                bool flag3 = false;
                if (flag2 && !this.m_DeleteMode)
                {
                    Vector2 vector6;
                    Vector2 vector7;
                    PolygonEditor.GetPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex0, out vector6);
                    PolygonEditor.GetPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex1, out vector7);
                    vector6 += offset;
                    vector7 += offset;
                    Vector3 start = transform.TransformPoint((Vector3) vector6);
                    Vector3 end = transform.TransformPoint((Vector3) vector7);
                    start.z = end.z = 0f;
                    Handles.color = Color.green;
                    Vector3[] points = new Vector3[] { start, end };
                    Handles.DrawAAPolyLine((float) 4f, points);
                    Handles.color = Color.white;
                    Vector2 vector10 = this.GetNearestPointOnEdge(transform.TransformPoint((Vector3) vector3), start, end);
                    EditorGUI.BeginChangeCheck();
                    float handleSize = HandleUtility.GetHandleSize((Vector3) vector10) * 0.04f;
                    Handles.color = Color.green;
                    vector10 = Handles.Slider2D((Vector3) vector10, new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), handleSize, new Handles.DrawCapFunction(Handles.DotCap), Vector3.zero);
                    Handles.color = Color.white;
                    if (EditorGUI.EndChangeCheck())
                    {
                        PolygonEditor.InsertPoint(this.m_SelectedEdgePath, this.m_SelectedEdgeVertex1, ((Vector2) ((vector6 + vector7) / 2f)) - offset);
                        this.m_SelectedPath = this.m_SelectedEdgePath;
                        this.m_SelectedVertex = this.m_SelectedEdgeVertex1;
                        this.m_SelectedDistance = 0f;
                        flag = true;
                        flag3 = true;
                    }
                }
                if (flag)
                {
                    Vector2 vector11;
                    PolygonEditor.GetPoint(this.m_SelectedPath, this.m_SelectedVertex, out vector11);
                    vector11 += offset;
                    Vector3 world = transform.TransformPoint((Vector3) vector11);
                    world.z = 0f;
                    Vector2 a = HandleUtility.WorldToGUIPoint(world);
                    float num8 = HandleUtility.GetHandleSize(world) * 0.04f;
                    if (((this.m_DeleteMode && (current.type == EventType.MouseDown)) && (Vector2.Distance(a, Event.current.mousePosition) < 50f)) || this.DeleteCommandEvent(current))
                    {
                        if ((current.type != EventType.ValidateCommand) && (PolygonEditor.GetPointCount(this.m_SelectedPath) > this.m_MinPathPoints))
                        {
                            PolygonEditor.RemovePoint(this.m_SelectedPath, this.m_SelectedVertex);
                            this.Reset();
                            flag3 = true;
                        }
                        current.Use();
                    }
                    EditorGUI.BeginChangeCheck();
                    Handles.color = !this.m_DeleteMode ? Color.green : Color.red;
                    Vector3 position = Handles.Slider2D(world, new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), num8, new Handles.DrawCapFunction(Handles.DotCap), Vector3.zero);
                    Handles.color = Color.white;
                    if (EditorGUI.EndChangeCheck() && !this.m_DeleteMode)
                    {
                        vector11 = transform.InverseTransformPoint(position) - offset;
                        PolygonEditor.TestPointMove(this.m_SelectedPath, this.m_SelectedVertex, vector11, out this.m_LeftIntersect, out this.m_RightIntersect, this.m_LoopingCollider);
                        PolygonEditor.SetPoint(this.m_SelectedPath, this.m_SelectedVertex, vector11);
                        flag3 = true;
                    }
                    if (!flag3)
                    {
                        this.DrawEdgesForSelectedPoint(position, transform, this.m_LeftIntersect, this.m_RightIntersect, this.m_LoopingCollider);
                    }
                }
                if (flag3)
                {
                    Undo.RecordObject(this.m_ActiveCollider, "Edit Collider");
                    PolygonEditor.ApplyEditing(this.m_ActiveCollider);
                }
                if (this.DeleteCommandEvent(current))
                {
                    Event.current.Use();
                }
                this.m_FirstOnSceneGUIAfterReset = false;
            }
        }

        public void Reset()
        {
            this.m_SelectedPath = -1;
            this.m_SelectedVertex = -1;
            this.m_SelectedEdgePath = -1;
            this.m_SelectedEdgeVertex0 = -1;
            this.m_SelectedEdgeVertex1 = -1;
            this.m_LeftIntersect = false;
            this.m_RightIntersect = false;
            this.m_FirstOnSceneGUIAfterReset = true;
        }

        public void StartEditing(Collider2D collider)
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            this.Reset();
            PolygonCollider2D colliderd = collider as PolygonCollider2D;
            if (colliderd != null)
            {
                this.m_ActiveCollider = collider;
                this.m_LoopingCollider = true;
                this.m_MinPathPoints = 3;
                PolygonEditor.StartEditing(colliderd);
            }
            else
            {
                EdgeCollider2D colliderd2 = collider as EdgeCollider2D;
                if (colliderd2 == null)
                {
                    throw new NotImplementedException(string.Format("PolygonEditorUtility does not support {0}", collider));
                }
                this.m_ActiveCollider = collider;
                this.m_LoopingCollider = false;
                this.m_MinPathPoints = 2;
                PolygonEditor.StartEditing(colliderd2);
            }
        }

        public void StopEditing()
        {
            PolygonEditor.StopEditing();
            this.m_ActiveCollider = null;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        private void UndoRedoPerformed()
        {
            if (this.m_ActiveCollider != null)
            {
                Collider2D activeCollider = this.m_ActiveCollider;
                this.StopEditing();
                this.StartEditing(activeCollider);
            }
        }
    }
}

