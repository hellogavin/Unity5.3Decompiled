namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class SliderScale
    {
        private static Vector2 s_CurrentMousePosition;
        private static float s_ScaleDrawLength = 1f;
        private static Vector2 s_StartMousePosition;
        private static float s_StartScale;
        private static float s_ValueDrag;

        public static float DoAxis(int id, float scale, Vector3 position, Vector3 direction, Quaternion rotation, float size, float snap)
        {
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
                        s_StartScale = scale;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return scale;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return scale;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return scale;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        float val = 1f + (HandleUtility.CalcLineTranslation(s_StartMousePosition, s_CurrentMousePosition, position, direction) / size);
                        val = Handles.SnapValue(val, snap);
                        scale = s_StartScale * val;
                        GUI.changed = true;
                        current.Use();
                    }
                    return scale;

                case EventType.Repaint:
                {
                    Color white = Color.white;
                    if (id == GUIUtility.keyboardControl)
                    {
                        white = Handles.color;
                        Handles.color = Handles.selectedColor;
                    }
                    float num2 = size;
                    if (GUIUtility.hotControl == id)
                    {
                        num2 = (size * scale) / s_StartScale;
                    }
                    Handles.CubeCap(id, position + ((Vector3) ((direction * num2) * s_ScaleDrawLength)), rotation, size * 0.1f);
                    Handles.DrawLine(position, position + ((Vector3) (direction * ((num2 * s_ScaleDrawLength) - (size * 0.05f)))));
                    if (id == GUIUtility.keyboardControl)
                    {
                        Handles.color = white;
                    }
                    return scale;
                }
                case EventType.Layout:
                    HandleUtility.AddControl(id, HandleUtility.DistanceToLine(position, position + ((Vector3) (direction * size))));
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position + ((Vector3) (direction * size)), size * 0.2f));
                    return scale;
            }
            return scale;
        }

        public static float DoCenter(int id, float value, Vector3 position, Quaternion rotation, float size, Handles.DrawCapFunction capFunc, float snap)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((HandleUtility.nearestControl == id) && (current.button == 0)) || ((GUIUtility.keyboardControl == id) && (current.button == 2)))
                    {
                        int num = id;
                        GUIUtility.keyboardControl = num;
                        GUIUtility.hotControl = num;
                        s_StartScale = value;
                        s_ValueDrag = 0f;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return value;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        s_ScaleDrawLength = 1f;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return value;

                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return value;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_ValueDrag += HandleUtility.niceMouseDelta * 0.01f;
                        value = (Handles.SnapValue(s_ValueDrag, snap) + 1f) * s_StartScale;
                        s_ScaleDrawLength = value / s_StartScale;
                        GUI.changed = true;
                        current.Use();
                    }
                    return value;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == id) && (current.keyCode == KeyCode.Escape))
                    {
                        value = s_StartScale;
                        s_ScaleDrawLength = 1f;
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        current.Use();
                    }
                    return value;

                case EventType.Repaint:
                {
                    Color white = Color.white;
                    if (id == GUIUtility.keyboardControl)
                    {
                        white = Handles.color;
                        Handles.color = Handles.selectedColor;
                    }
                    capFunc(id, position, rotation, size * 0.15f);
                    if (id == GUIUtility.keyboardControl)
                    {
                        Handles.color = white;
                    }
                    return value;
                }
                case EventType.Layout:
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(position, size * 0.15f));
                    return value;
            }
            return value;
        }
    }
}

