namespace UnityEditor
{
    using System;
    using UnityEngine;

    public class PopupWindow : EditorWindow
    {
        private Rect m_ActivatorRect;
        private Vector2 m_LastWantedSize = Vector2.zero;
        private PopupWindowContent m_WindowContent;
        private static Rect s_LastActivatorRect;
        private static double s_LastClosedTime;

        internal PopupWindow()
        {
            base.hideFlags = HideFlags.DontSave;
            base.wantsMouseMove = true;
        }

        private void FitWindowToContent()
        {
            Vector2 windowSize = this.m_WindowContent.GetWindowSize();
            if (this.m_LastWantedSize != windowSize)
            {
                this.m_LastWantedSize = windowSize;
                Rect rect = base.m_Parent.window.GetDropDownRect(this.m_ActivatorRect, windowSize, windowSize);
                Vector2 vector2 = new Vector2(rect.width, rect.height);
                base.maxSize = vector2;
                base.minSize = vector2;
                base.position = rect;
            }
        }

        private void Init(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            this.m_WindowContent = windowContent;
            this.m_WindowContent.editorWindow = this;
            this.m_WindowContent.OnOpen();
            this.m_ActivatorRect = GUIUtility.GUIToScreenRect(activatorRect);
            base.ShowAsDropDown(this.m_ActivatorRect, this.m_WindowContent.GetWindowSize(), locationPriorityOrder);
        }

        private void OnDisable()
        {
            s_LastClosedTime = EditorApplication.timeSinceStartup;
            if (this.m_WindowContent != null)
            {
                this.m_WindowContent.OnClose();
            }
        }

        internal void OnGUI()
        {
            this.FitWindowToContent();
            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);
            this.m_WindowContent.OnGUI(rect);
            GUI.Label(rect, GUIContent.none, "grey_border");
        }

        private static bool ShouldShowWindow(Rect activatorRect)
        {
            if (((EditorApplication.timeSinceStartup - s_LastClosedTime) < 0.2) && !(activatorRect != s_LastActivatorRect))
            {
                return false;
            }
            s_LastActivatorRect = activatorRect;
            return true;
        }

        public static void Show(Rect activatorRect, PopupWindowContent windowContent)
        {
            Show(activatorRect, windowContent, null);
        }

        internal static void Show(Rect activatorRect, PopupWindowContent windowContent, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            if (ShouldShowWindow(activatorRect))
            {
                PopupWindow window = ScriptableObject.CreateInstance<PopupWindow>();
                if (window != null)
                {
                    window.Init(activatorRect, windowContent, locationPriorityOrder);
                }
                GUIUtility.ExitGUI();
            }
        }
    }
}

