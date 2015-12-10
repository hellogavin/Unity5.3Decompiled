namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public static class SpriteEditorHandles
    {
        private static Vector2 s_CurrentMousePosition;
        private static Vector2 s_DragScreenOffset;
        private static Vector2 s_DragStartScreenPosition;
        private static int s_RectSelectionID = GUIUtility.GetPermanentControlID();

        private static Rect GetCurrentRect(bool screenSpace, float textureWidth, float textureHeight, Vector2 startPoint, Vector2 endPoint)
        {
            Rect rect = SpriteEditorUtility.ClampedRect(SpriteEditorUtility.RoundToInt(EditorGUIExt.FromToRect(Handles.s_InverseMatrix.MultiplyPoint((Vector3) startPoint), Handles.s_InverseMatrix.MultiplyPoint((Vector3) endPoint))), new Rect(0f, 0f, textureWidth, textureHeight), false);
            if (screenSpace)
            {
                Vector2 vector = Handles.matrix.MultiplyPoint((Vector3) new Vector2(rect.xMin, rect.yMin));
                Vector2 vector2 = Handles.matrix.MultiplyPoint((Vector3) new Vector2(rect.xMax, rect.yMax));
                rect = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
            }
            return rect;
        }

        internal static void HandleSliderRectMouseDown(int id, Event evt, Rect pos)
        {
            int num = id;
            GUIUtility.keyboardControl = num;
            GUIUtility.hotControl = num;
            s_CurrentMousePosition = evt.mousePosition;
            s_DragStartScreenPosition = evt.mousePosition;
            Vector2 vector = Handles.matrix.MultiplyPoint((Vector3) pos.center);
            s_DragScreenOffset = s_CurrentMousePosition - vector;
            EditorGUIUtility.SetWantsMouseJumping(1);
        }

        internal static Vector2 PivotSlider(Rect sprite, Vector2 pos, GUIStyle pivotDot, GUIStyle pivotDotActive)
        {
            int controlID = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
            pos = new Vector2(sprite.xMin + (sprite.width * pos.x), sprite.yMin + (sprite.height * pos.y));
            Vector2 vector = Handles.matrix.MultiplyPoint((Vector3) pos);
            Rect position = new Rect(vector.x - (pivotDot.fixedWidth * 0.5f), vector.y - (pivotDot.fixedHeight * 0.5f), pivotDotActive.fixedWidth, pivotDotActive.fixedHeight);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (((current.button == 0) && position.Contains(Event.current.mousePosition)) && !current.alt)
                    {
                        int num2 = controlID;
                        GUIUtility.keyboardControl = num2;
                        GUIUtility.hotControl = num2;
                        s_CurrentMousePosition = current.mousePosition;
                        s_DragStartScreenPosition = current.mousePosition;
                        Vector2 vector2 = Handles.matrix.MultiplyPoint((Vector3) pos);
                        s_DragScreenOffset = s_CurrentMousePosition - vector2;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector2 vector3 = pos;
                        Vector3 vector4 = Handles.s_InverseMatrix.MultiplyPoint((Vector3) (s_CurrentMousePosition - s_DragScreenOffset));
                        pos = new Vector2(vector4.x, vector4.y);
                        Vector2 vector5 = vector3 - pos;
                        if (!Mathf.Approximately(vector5.magnitude, 0f))
                        {
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    break;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && (current.keyCode == KeyCode.Escape))
                    {
                        pos = Handles.s_InverseMatrix.MultiplyPoint((Vector3) (s_DragStartScreenPosition - s_DragScreenOffset));
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        current.Use();
                    }
                    break;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(position, MouseCursor.Arrow, controlID);
                    if (GUIUtility.hotControl != controlID)
                    {
                        pivotDot.Draw(position, GUIContent.none, controlID);
                        break;
                    }
                    pivotDotActive.Draw(position, GUIContent.none, controlID);
                    break;
            }
            pos = new Vector2((pos.x - sprite.xMin) / sprite.width, (pos.y - sprite.yMin) / sprite.height);
            return pos;
        }

        internal static Vector2 PointSlider(Vector2 pos, MouseCursor cursor, GUIStyle dragDot, GUIStyle dragDotActive)
        {
            int controlID = GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard);
            Vector2 vector = Handles.matrix.MultiplyPoint((Vector3) pos);
            Rect position = new Rect(vector.x - (dragDot.fixedWidth * 0.5f), vector.y - (dragDot.fixedHeight * 0.5f), dragDot.fixedWidth, dragDot.fixedHeight);
            if (Event.current.GetTypeForControl(controlID) == EventType.Repaint)
            {
                if (GUIUtility.hotControl == controlID)
                {
                    dragDotActive.Draw(position, GUIContent.none, controlID);
                }
                else
                {
                    dragDot.Draw(position, GUIContent.none, controlID);
                }
            }
            return ScaleSlider(pos, cursor, position);
        }

        internal static Rect RectCreator(float textureWidth, float textureHeight, GUIStyle rectStyle)
        {
            Event current = Event.current;
            Vector2 mousePosition = current.mousePosition;
            int controlID = s_RectSelectionID;
            Rect rect = new Rect();
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (current.button == 0)
                    {
                        GUIUtility.hotControl = controlID;
                        Rect rect2 = new Rect(0f, 0f, textureWidth, textureHeight);
                        Vector2 vector2 = Handles.s_InverseMatrix.MultiplyPoint((Vector3) mousePosition);
                        float a = Mathf.Max(vector2.x, rect2.xMin);
                        vector2.x = Mathf.Min(a, rect2.xMax);
                        float introduced8 = Mathf.Max(vector2.y, rect2.yMin);
                        vector2.y = Mathf.Min(introduced8, rect2.yMax);
                        s_DragStartScreenPosition = Handles.s_Matrix.MultiplyPoint((Vector3) vector2);
                        s_CurrentMousePosition = mousePosition;
                        current.Use();
                    }
                    return rect;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && (current.button == 0))
                    {
                        if (ValidRect(s_DragStartScreenPosition, s_CurrentMousePosition))
                        {
                            rect = GetCurrentRect(false, textureWidth, textureHeight, s_DragStartScreenPosition, s_CurrentMousePosition);
                            GUI.changed = true;
                            current.Use();
                        }
                        GUIUtility.hotControl = 0;
                    }
                    return rect;

                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return rect;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        s_CurrentMousePosition = new Vector2(mousePosition.x, mousePosition.y);
                        current.Use();
                    }
                    return rect;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && (current.keyCode == KeyCode.Escape))
                    {
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        current.Use();
                    }
                    return rect;

                case EventType.Repaint:
                    if ((GUIUtility.hotControl == controlID) && ValidRect(s_DragStartScreenPosition, s_CurrentMousePosition))
                    {
                        SpriteEditorUtility.BeginLines((Color) (Color.green * 1.5f));
                        SpriteEditorUtility.DrawBox(GetCurrentRect(false, textureWidth, textureHeight, s_DragStartScreenPosition, s_CurrentMousePosition));
                        SpriteEditorUtility.EndLines();
                    }
                    return rect;
            }
            return rect;
        }

        internal static Vector2 ScaleSlider(Vector2 pos, MouseCursor cursor, Rect cursorRect)
        {
            return ScaleSlider(GUIUtility.GetControlID("Slider1D".GetHashCode(), FocusType.Keyboard), pos, cursor, cursorRect);
        }

        private static Vector2 ScaleSlider(int id, Vector2 pos, MouseCursor cursor, Rect cursorRect)
        {
            Vector2 vector = Handles.matrix.MultiplyPoint((Vector3) pos);
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((current.button == 0) && cursorRect.Contains(Event.current.mousePosition)) && !current.alt)
                    {
                        int num = id;
                        GUIUtility.keyboardControl = num;
                        GUIUtility.hotControl = num;
                        s_CurrentMousePosition = current.mousePosition;
                        s_DragStartScreenPosition = current.mousePosition;
                        s_DragScreenOffset = s_CurrentMousePosition - vector;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    return pos;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == id) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return pos;

                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return pos;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector2 vector2 = pos;
                        pos = Handles.s_InverseMatrix.MultiplyPoint((Vector3) s_CurrentMousePosition);
                        Vector2 vector3 = vector2 - pos;
                        if (!Mathf.Approximately(vector3.magnitude, 0f))
                        {
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    return pos;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == id) && (current.keyCode == KeyCode.Escape))
                    {
                        pos = Handles.s_InverseMatrix.MultiplyPoint((Vector3) (s_DragStartScreenPosition - s_DragScreenOffset));
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        current.Use();
                    }
                    return pos;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(cursorRect, cursor, id);
                    return pos;
            }
            return pos;
        }

        internal static Rect SliderRect(Rect pos)
        {
            int controlID = GUIUtility.GetControlID("SliderRect".GetHashCode(), FocusType.Keyboard);
            Event current = Event.current;
            if (SpriteEditorWindow.s_OneClickDragStarted && (current.type == EventType.Repaint))
            {
                HandleSliderRectMouseDown(controlID, current, pos);
                SpriteEditorWindow.s_OneClickDragStarted = false;
            }
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (((current.button == 0) && pos.Contains(Handles.s_InverseMatrix.MultiplyPoint((Vector3) Event.current.mousePosition))) && !current.alt)
                    {
                        HandleSliderRectMouseDown(controlID, current, pos);
                        current.Use();
                    }
                    return pos;

                case EventType.MouseUp:
                    if ((GUIUtility.hotControl == controlID) && ((current.button == 0) || (current.button == 2)))
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);
                    }
                    return pos;

                case EventType.MouseMove:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return pos;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        s_CurrentMousePosition += current.delta;
                        Vector2 center = pos.center;
                        pos.center = Handles.s_InverseMatrix.MultiplyPoint((Vector3) (s_CurrentMousePosition - s_DragScreenOffset));
                        Vector2 vector4 = center - pos.center;
                        if (!Mathf.Approximately(vector4.magnitude, 0f))
                        {
                            GUI.changed = true;
                        }
                        current.Use();
                    }
                    return pos;

                case EventType.KeyDown:
                    if ((GUIUtility.hotControl == controlID) && (current.keyCode == KeyCode.Escape))
                    {
                        pos.center = Handles.s_InverseMatrix.MultiplyPoint((Vector3) (s_DragStartScreenPosition - s_DragScreenOffset));
                        GUIUtility.hotControl = 0;
                        GUI.changed = true;
                        current.Use();
                    }
                    return pos;

                case EventType.Repaint:
                {
                    Vector2 vector2 = Handles.s_InverseMatrix.MultiplyPoint((Vector3) new Vector2(pos.xMin, pos.yMin));
                    Vector2 vector3 = Handles.s_InverseMatrix.MultiplyPoint((Vector3) new Vector2(pos.xMax, pos.yMax));
                    EditorGUIUtility.AddCursorRect(new Rect(vector2.x, vector2.y, vector3.x - vector2.x, vector3.y - vector2.y), MouseCursor.Arrow, controlID);
                    return pos;
                }
            }
            return pos;
        }

        private static bool ValidRect(Vector2 startPoint, Vector2 endPoint)
        {
            Vector2 vector = endPoint - startPoint;
            return ((Mathf.Abs(vector.x) > 5f) && (Mathf.Abs((endPoint - startPoint).y) > 5f));
        }
    }
}

