using System;
using UnityEditor;
using UnityEngine;

internal class PreviewGUI
{
    private static Rect s_Position;
    private static Vector2 s_ScrollPos;
    private static Rect s_ViewRect;
    private static int sliderHash = "Slider".GetHashCode();

    internal static void BeginScrollView(Rect position, Vector2 scrollPosition, Rect viewRect, GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar)
    {
        s_ScrollPos = scrollPosition;
        s_ViewRect = viewRect;
        s_Position = position;
        GUIClip.Push(position, new Vector2(Mathf.Round((-scrollPosition.x - viewRect.x) - ((viewRect.width - position.width) * 0.5f)), Mathf.Round((-scrollPosition.y - viewRect.y) - ((viewRect.height - position.height) * 0.5f))), Vector2.zero, false);
    }

    public static int CycleButton(int selected, GUIContent[] options)
    {
        Styles.Init();
        return EditorGUILayout.CycleButton(selected, options, Styles.preButton);
    }

    public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
    {
        int controlID = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
        Event current = Event.current;
        switch (current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition) && (position.width > 50f))
                {
                    GUIUtility.hotControl = controlID;
                    current.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                }
                return scrollPosition;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                }
                EditorGUIUtility.SetWantsMouseJumping(0);
                return scrollPosition;

            case EventType.MouseMove:
                return scrollPosition;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    scrollPosition -= (Vector2) (((current.delta * (!current.shift ? ((float) 1) : ((float) 3))) / Mathf.Min(position.width, position.height)) * 140f);
                    scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                    current.Use();
                    GUI.changed = true;
                }
                return scrollPosition;
        }
        return scrollPosition;
    }

    public static Vector2 EndScrollView()
    {
        GUIClip.Pop();
        Rect rect = s_Position;
        Rect rect2 = s_Position;
        Rect rect3 = s_ViewRect;
        Vector2 vector = s_ScrollPos;
        switch (Event.current.type)
        {
            case EventType.Layout:
                GUIUtility.GetControlID(sliderHash, FocusType.Passive);
                GUIUtility.GetControlID(sliderHash, FocusType.Passive);
                return vector;

            case EventType.Used:
                return vector;
        }
        bool flag = false;
        bool flag2 = false;
        if (flag2 || (rect3.width > rect.width))
        {
            flag2 = true;
        }
        if (flag || (rect3.height > rect.height))
        {
            flag = true;
        }
        int controlID = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
        if (flag2)
        {
            GUIStyle slider = "PreHorizontalScrollbar";
            GUIStyle thumb = "PreHorizontalScrollbarThumb";
            float num2 = (rect3.width - rect.width) * 0.5f;
            vector.x = GUI.Slider(new Rect(rect2.x, rect2.yMax - slider.fixedHeight, rect.width - (!flag ? 0f : slider.fixedHeight), slider.fixedHeight), vector.x, rect.width + num2, -num2, rect3.width, slider, thumb, true, controlID);
        }
        else
        {
            vector.x = 0f;
        }
        controlID = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
        if (flag)
        {
            GUIStyle style3 = "PreVerticalScrollbar";
            GUIStyle style4 = "PreVerticalScrollbarThumb";
            float num3 = (rect3.height - rect.height) * 0.5f;
            vector.y = GUI.Slider(new Rect(rect.xMax - style3.fixedWidth, rect.y, style3.fixedWidth, rect.height), vector.y, rect.height + num3, -num3, rect3.height, style3, style4, false, controlID);
            return vector;
        }
        vector.y = 0f;
        return vector;
    }

    internal class Styles
    {
        public static GUIStyle preButton;

        public static void Init()
        {
            preButton = "preButton";
        }
    }
}

