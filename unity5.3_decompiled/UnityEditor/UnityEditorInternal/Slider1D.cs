namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class Slider1D
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_StartMousePosition;
        private static Vector3 s_StartPosition;

        internal static Vector3 Do(int id, Vector3 position, Vector3 direction, float size, Handles.DrawCapFunction drawFunc, float snap)
        {
            return Do(id, position, direction, direction, size, drawFunc, snap);
        }

        internal static Vector3 Do(int id, Vector3 position, Vector3 handleDirection, Vector3 slideDirection, float size, Handles.DrawCapFunction drawFunc, float snap)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if ((((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2))) && (GUIUtility.hotControl == 0))
                    {
                        int num2 = id;
                        GUIUtility.keyboardControl = num2;
                        GUIUtility.hotControl = num2;
                        s_CurrentMousePosition = s_StartMousePosition = current.mousePosition;
                        s_StartPosition = position;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return position;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
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
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        float num = Handles.SnapValue(HandleUtility.CalcLineTranslation(s_StartMousePosition, s_CurrentMousePosition, s_StartPosition, slideDirection), snap);
                        Vector3 vector = Handles.matrix.MultiplyVector(slideDirection);
                        Vector3 v = Handles.s_Matrix.MultiplyPoint(s_StartPosition) + ((Vector3) (vector * num));
                        position = Handles.s_InverseMatrix.MultiplyPoint(v);
                        GUI.changed = true;
                        current.Use();
                    }
                    return position;

                case EventType.Repaint:
                {
                    Color white = Color.white;
                    if ((id == GUIUtility.keyboardControl) && GUI.enabled)
                    {
                        white = Handles.color;
                        Handles.color = Handles.selectedColor;
                    }
                    drawFunc(id, position, Quaternion.LookRotation(handleDirection), size);
                    if (id == GUIUtility.keyboardControl)
                    {
                        Handles.color = white;
                    }
                    return position;
                }
                case EventType.Layout:
                    if (drawFunc != new Handles.DrawCapFunction(Handles.ArrowCap))
                    {
                        HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position, size * 0.2f));
                        return position;
                    }
                    HandleUtility.AddControl(id, HandleUtility.DistanceToLine(position, position + ((Vector3) (slideDirection * size))));
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position + ((Vector3) (slideDirection * size)), size * 0.2f));
                    return position;
            }
            return position;
        }
    }
}

