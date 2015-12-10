namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class PreviewResizer
    {
        [SerializeField]
        private float m_CachedPref;
        [SerializeField]
        private int m_ControlHash;
        private int m_Id;
        [SerializeField]
        private string m_PrefName;
        private static float s_CachedPreviewSizeWhileDragging;
        private static float s_DraggedPreviewSize;
        private static float s_MouseDownLocation;
        private static float s_MouseDownValue;
        private static bool s_MouseDragged;

        public bool GetExpanded()
        {
            if (GUIUtility.hotControl == this.id)
            {
                return (s_CachedPreviewSizeWhileDragging > 0f);
            }
            return (this.m_CachedPref > 0f);
        }

        public bool GetExpandedBeforeDragging()
        {
            return (this.m_CachedPref > 0f);
        }

        public float GetPreviewSize()
        {
            if (GUIUtility.hotControl == this.id)
            {
                return Mathf.Max(0f, s_CachedPreviewSizeWhileDragging);
            }
            return Mathf.Max(0f, this.m_CachedPref);
        }

        public void Init(string prefName)
        {
            if ((this.m_ControlHash == 0) || string.IsNullOrEmpty(this.m_PrefName))
            {
                this.m_ControlHash = prefName.GetHashCode();
                this.m_PrefName = "Preview_" + prefName;
                this.m_CachedPref = EditorPrefs.GetFloat(this.m_PrefName, 1f);
            }
        }

        public static float PixelPreciseCollapsibleSlider(int id, Rect position, float value, float min, float max, ref bool expanded)
        {
            Event current = Event.current;
            switch (current.GetTypeForControl(id))
            {
                case EventType.MouseDown:
                    if (((GUIUtility.hotControl == 0) && (current.button == 0)) && position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        s_MouseDownLocation = current.mousePosition.y;
                        s_MouseDownValue = value;
                        s_MouseDragged = false;
                        current.Use();
                    }
                    return value;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        GUIUtility.hotControl = 0;
                        if (!s_MouseDragged)
                        {
                            expanded = !expanded;
                        }
                        current.Use();
                    }
                    return value;

                case EventType.MouseMove:
                case EventType.KeyDown:
                case EventType.KeyUp:
                case EventType.ScrollWheel:
                    return value;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        value = Mathf.Clamp((current.mousePosition.y - s_MouseDownLocation) + s_MouseDownValue, min, max - 1f);
                        GUI.changed = true;
                        s_MouseDragged = true;
                        current.Use();
                    }
                    return value;

                case EventType.Repaint:
                    if (GUIUtility.hotControl == 0)
                    {
                        EditorGUIUtility.AddCursorRect(position, MouseCursor.SplitResizeUpDown);
                    }
                    if (GUIUtility.hotControl == id)
                    {
                        EditorGUIUtility.AddCursorRect(new Rect(position.x, position.y - 100f, position.width, position.height + 200f), MouseCursor.SplitResizeUpDown);
                    }
                    return value;
            }
            return value;
        }

        public float ResizeHandle(Rect windowPosition, float minSize, float minRemainingSize, float resizerHeight)
        {
            return this.ResizeHandle(windowPosition, minSize, minRemainingSize, resizerHeight, new Rect());
        }

        public float ResizeHandle(Rect windowPosition, float minSize, float minRemainingSize, float resizerHeight, Rect dragRect)
        {
            if (Mathf.Abs(this.m_CachedPref) < minSize)
            {
                this.m_CachedPref = minSize * Mathf.Sign(this.m_CachedPref);
            }
            float b = windowPosition.height - minRemainingSize;
            float a = (GUIUtility.hotControl != this.id) ? Mathf.Max(0f, this.m_CachedPref) : s_DraggedPreviewSize;
            bool expanded = this.m_CachedPref > 0f;
            float num3 = Mathf.Abs(this.m_CachedPref);
            Rect position = new Rect(0f, (windowPosition.height - a) - resizerHeight, windowPosition.width, resizerHeight);
            if (dragRect.width != 0f)
            {
                position.x = dragRect.x;
                position.width = dragRect.width;
            }
            bool flag3 = expanded;
            a = -PixelPreciseCollapsibleSlider(this.id, position, -a, -b, 0f, ref expanded);
            a = Mathf.Min(a, b);
            if (GUIUtility.hotControl == this.id)
            {
                s_DraggedPreviewSize = a;
            }
            if (a < minSize)
            {
                a = (a >= (minSize * 0.5f)) ? minSize : 0f;
            }
            if (expanded != flag3)
            {
                a = !expanded ? 0f : num3;
            }
            expanded = a >= (minSize / 2f);
            if (GUIUtility.hotControl == 0)
            {
                if (a > 0f)
                {
                    num3 = a;
                }
                float num4 = num3 * (!expanded ? ((float) (-1)) : ((float) 1));
                if (num4 != this.m_CachedPref)
                {
                    this.m_CachedPref = num4;
                    EditorPrefs.SetFloat(this.m_PrefName, this.m_CachedPref);
                }
            }
            s_CachedPreviewSizeWhileDragging = a;
            return a;
        }

        public void SetExpanded(bool expanded)
        {
            this.m_CachedPref = Mathf.Abs(this.m_CachedPref) * (!expanded ? ((float) (-1)) : ((float) 1));
            EditorPrefs.SetFloat(this.m_PrefName, this.m_CachedPref);
        }

        public void ToggleExpanded()
        {
            this.m_CachedPref = -this.m_CachedPref;
            EditorPrefs.SetFloat(this.m_PrefName, this.m_CachedPref);
        }

        private int id
        {
            get
            {
                if (this.m_Id == 0)
                {
                    this.m_Id = GUIUtility.GetControlID(this.m_ControlHash, FocusType.Passive, new Rect());
                }
                return this.m_Id;
            }
        }
    }
}

