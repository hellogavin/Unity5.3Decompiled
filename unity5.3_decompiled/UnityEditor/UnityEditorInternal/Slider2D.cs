namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class Slider2D
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_StartPlaneOffset;
        private static Vector3 s_StartPosition;

        private static Vector2 CalcDeltaAlongDirections(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
        {
            Vector2 vector = new Vector2(0f, 0f);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if ((((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2))) && (GUIUtility.hotControl == 0))
                    {
                        Plane plane = new Plane(Handles.matrix.MultiplyVector(handleDir), Handles.matrix.MultiplyPoint(handlePos));
                        Ray ray = HandleUtility.GUIPointToWorldRay(current.mousePosition);
                        float enter = 0f;
                        plane.Raycast(ray, out enter);
                        int num5 = id;
                        GUIUtility.keyboardControl = num5;
                        GUIUtility.hotControl = num5;
                        s_CurrentMousePosition = current.mousePosition;
                        s_StartPosition = handlePos;
                        Vector3 lhs = Handles.s_InverseMatrix.MultiplyPoint(ray.GetPoint(enter)) - handlePos;
                        s_StartPlaneOffset.x = Vector3.Dot(lhs, slideDir1);
                        s_StartPlaneOffset.y = Vector3.Dot(lhs, slideDir2);
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return vector;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return vector;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return vector;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector3 a = Handles.matrix.MultiplyPoint(handlePos);
                        Vector3 normalized = Handles.matrix.MultiplyVector(slideDir1).normalized;
                        Vector3 vector6 = Handles.matrix.MultiplyVector(slideDir2).normalized;
                        Ray ray2 = HandleUtility.GUIPointToWorldRay(s_CurrentMousePosition);
                        Plane plane2 = new Plane(a, a + normalized, a + vector6);
                        float num2 = 0f;
                        if (plane2.Raycast(ray2, out num2))
                        {
                            Vector3 point = Handles.s_InverseMatrix.MultiplyPoint(ray2.GetPoint(num2));
                            vector.x = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir1);
                            vector.y = HandleUtility.PointOnLineParameter(point, s_StartPosition, slideDir2);
                            vector -= s_StartPlaneOffset;
                            if ((snap.x > 0f) || (snap.y > 0f))
                            {
                                vector.x = Handles.SnapValue(vector.x, snap.x);
                                vector.y = Handles.SnapValue(vector.y, snap.y);
                            }
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    return vector;

                case EventType.Repaint:
                    if (drawFunc != null)
                    {
                        Vector3 position = handlePos + offset;
                        Quaternion rotation = Quaternion.LookRotation(handleDir, slideDir1);
                        Color white = Color.white;
                        if (id == GUIUtility.keyboardControl)
                        {
                            white = Handles.color;
                            Handles.color = Handles.selectedColor;
                        }
                        drawFunc(id, position, rotation, handleSize);
                        if (id == GUIUtility.keyboardControl)
                        {
                            Handles.color = white;
                        }
                        if (drawHelper && (GUIUtility.hotControl == id))
                        {
                            Vector3[] verts = new Vector3[4];
                            float num3 = handleSize * 10f;
                            verts[0] = position + ((Vector3) ((slideDir1 * num3) + (slideDir2 * num3)));
                            verts[1] = verts[0] - ((Vector3) ((slideDir1 * num3) * 2f));
                            verts[2] = verts[1] - ((Vector3) ((slideDir2 * num3) * 2f));
                            verts[3] = verts[2] + ((Vector3) ((slideDir1 * num3) * 2f));
                            Color color = Handles.color;
                            Handles.color = Color.white;
                            float r = 0.6f;
                            Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.05f), new Color(r, r, r, 0.4f));
                            Handles.color = color;
                        }
                        return vector;
                    }
                    return vector;

                case EventType.Layout:
                    if (drawFunc != new Handles.DrawCapFunction(Handles.ArrowCap))
                    {
                        if (drawFunc == new Handles.DrawCapFunction(Handles.RectangleCap))
                        {
                            HandleUtility.AddControl(id, HandleUtility.DistanceToRectangle(handlePos + offset, Quaternion.LookRotation(handleDir, slideDir1), handleSize));
                            return vector;
                        }
                        HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(handlePos + offset, handleSize * 0.5f));
                        return vector;
                    }
                    HandleUtility.AddControl(id, HandleUtility.DistanceToLine(handlePos + offset, handlePos + ((Vector3) (handleDir * handleSize))));
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle((handlePos + offset) + ((Vector3) (handleDir * handleSize)), handleSize * 0.2f));
                    return vector;
            }
            return vector;
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper)
        {
            return Do(id, handlePos, new Vector3(0f, 0f, 0f), handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, float snap, bool drawHelper)
        {
            return Do(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, new Vector2(snap, snap), drawHelper);
        }

        public static Vector3 Do(int id, Vector3 handlePos, Vector3 offset, Vector3 handleDir, Vector3 slideDir1, Vector3 slideDir2, float handleSize, Handles.DrawCapFunction drawFunc, Vector2 snap, bool drawHelper)
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            Vector2 vector = CalcDeltaAlongDirections(id, handlePos, offset, handleDir, slideDir1, slideDir2, handleSize, drawFunc, snap, drawHelper);
            if (GUI.changed)
            {
                handlePos = (Vector3) ((s_StartPosition + (slideDir1 * vector.x)) + (slideDir2 * vector.y));
            }
            GUI.changed |= changed;
            return handlePos;
        }
    }
}

