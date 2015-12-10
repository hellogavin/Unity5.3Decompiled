namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class FreeMove
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_StartMousePosition;
        private static Vector3 s_StartPosition;

        public static Vector3 Do(int id, Vector3 position, Quaternion rotation, float size, Vector3 snap, Handles.DrawCapFunction capFunc)
        {
            bool flag;
            Vector3 vector = Handles.matrix.MultiplyPoint(position);
            Matrix4x4 matrix = Handles.matrix;
            VertexSnapping.HandleKeyAndMouseMove(id);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2)))
                    {
                        int num3 = id;
                        GUIUtility.keyboardControl = num3;
                        GUIUtility.hotControl = num3;
                        s_CurrentMousePosition = s_StartMousePosition = current.mousePosition;
                        s_StartPosition = position;
                        HandleUtility.ignoreRaySnapObjects = null;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return position;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        HandleUtility.ignoreRaySnapObjects = null;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return position;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return position;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl != id)
                    {
                        return position;
                    }
                    flag = EditorGUI.actionKey && current.shift;
                    if (flag)
                    {
                        if (HandleUtility.ignoreRaySnapObjects == null)
                        {
                            Handles.SetupIgnoreRaySnapObjects();
                        }
                        object obj2 = HandleUtility.RaySnap(HandleUtility.GUIPointToWorldRay(current.mousePosition));
                        if (obj2 == null)
                        {
                            flag = false;
                            break;
                        }
                        RaycastHit hit = (RaycastHit) obj2;
                        float num = 0f;
                        if (Tools.pivotMode == PivotMode.Center)
                        {
                            float num2 = HandleUtility.CalcRayPlaceOffset(HandleUtility.ignoreRaySnapObjects, hit.normal);
                            if (num2 != float.PositiveInfinity)
                            {
                                num = Vector3.Dot(position, hit.normal) - num2;
                            }
                        }
                        position = Handles.s_InverseMatrix.MultiplyPoint(hit.point + ((Vector3) (hit.normal * num)));
                    }
                    break;

                case EventType.Repaint:
                {
                    Color white = Color.white;
                    if (id == GUIUtility.keyboardControl)
                    {
                        white = Handles.color;
                        Handles.color = Handles.selectedColor;
                    }
                    Handles.matrix = Matrix4x4.identity;
                    capFunc(id, vector, Camera.current.transform.rotation, size);
                    Handles.matrix = matrix;
                    if (id == GUIUtility.keyboardControl)
                    {
                        Handles.color = white;
                    }
                    return position;
                }
                case EventType.Layout:
                    Handles.matrix = Matrix4x4.identity;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(vector, size * 1.2f));
                    Handles.matrix = matrix;
                    return position;

                default:
                    return position;
            }
            if (!flag)
            {
                s_CurrentMousePosition += new Vector2(current.delta.x, -current.delta.y);
                Vector3 vector2 = Camera.current.WorldToScreenPoint(Handles.s_Matrix.MultiplyPoint(s_StartPosition)) + (s_CurrentMousePosition - s_StartMousePosition);
                position = Handles.s_InverseMatrix.MultiplyPoint(Camera.current.ScreenToWorldPoint(vector2));
                if ((Camera.current.transform.forward == Vector3.forward) || (Camera.current.transform.forward == -Vector3.forward))
                {
                    position.z = s_StartPosition.z;
                }
                if ((Camera.current.transform.forward == Vector3.up) || (Camera.current.transform.forward == -Vector3.up))
                {
                    position.y = s_StartPosition.y;
                }
                if ((Camera.current.transform.forward == Vector3.right) || (Camera.current.transform.forward == -Vector3.right))
                {
                    position.x = s_StartPosition.x;
                }
                if (Tools.vertexDragging)
                {
                    Vector3 vector3;
                    if (HandleUtility.ignoreRaySnapObjects == null)
                    {
                        Handles.SetupIgnoreRaySnapObjects();
                    }
                    if (HandleUtility.FindNearestVertex(current.mousePosition, null, out vector3))
                    {
                        position = Handles.s_InverseMatrix.MultiplyPoint(vector3);
                    }
                }
                if (EditorGUI.actionKey && !current.shift)
                {
                    Vector3 vector4 = position - s_StartPosition;
                    vector4.x = Handles.SnapValue(vector4.x, snap.x);
                    vector4.y = Handles.SnapValue(vector4.y, snap.y);
                    vector4.z = Handles.SnapValue(vector4.z, snap.z);
                    position = s_StartPosition + vector4;
                }
            }
            GUI.changed = true;
            current.Use();
            return position;
        }
    }
}

