namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class DragRectGUI
    {
        private static int dragRectHash = "DragRect".GetHashCode();
        private static int s_DragCandidateState = 0;
        private static float s_DragSensitivity = 1f;

        public static int DragRect(Rect position, int value, int minValue, int maxValue)
        {
            Event current = Event.current;
            int controlID = GUIUtility.GetControlID(dragRectHash, FocusType.Passive, position);
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && (current.button == 0))
                    {
                        GUIUtility.hotControl = controlID;
                        s_DragCandidateState = 1;
                        current.Use();
                    }
                    return value;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (s_DragCandidateState != 0))
                    {
                        GUIUtility.hotControl = 0;
                        s_DragCandidateState = 0;
                        current.Use();
                    }
                    return value;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return value;

                case EventType.MouseDrag:
                    if ((GUIUtility.hotControl == controlID) && (s_DragCandidateState == 1))
                    {
                        value += (int) (HandleUtility.niceMouseDelta * s_DragSensitivity);
                        GUI.changed = true;
                        current.Use();
                        if (value < minValue)
                        {
                            value = minValue;
                            return value;
                        }
                        if (value > maxValue)
                        {
                            value = maxValue;
                        }
                        return value;
                    }
                    return value;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(position, MouseCursor.SlideArrow);
                    return value;
            }
            return value;
        }
    }
}

