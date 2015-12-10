namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class ContainerWindow : ScriptableObject
    {
        private const float kBorderSize = 4f;
        private const float kTitleHeight = 24f;
        private const float kButtonWidth = 13f;
        private const float kButtonHeight = 13f;
        private const float kButtonSpacing = 3f;
        private const float kButtonTop = 0f;
        [SerializeField]
        private MonoReloadableIntPtr m_WindowPtr;
        [SerializeField]
        private Rect m_PixelRect;
        [SerializeField]
        private int m_ShowMode;
        [SerializeField]
        private string m_Title = string.Empty;
        [SerializeField]
        private View m_MainView;
        [SerializeField]
        private Vector2 m_MinSize = new Vector2(120f, 80f);
        [SerializeField]
        private Vector2 m_MaxSize = new Vector2(4000f, 4000f);
        internal bool m_DontSaveToLayout;
        [SerializeField]
        private SnapEdge m_Left;
        [SerializeField]
        private SnapEdge m_Right;
        [SerializeField]
        private SnapEdge m_Top;
        [SerializeField]
        private SnapEdge m_Bottom;
        private SnapEdge[] m_EdgesCache;
        private int m_ButtonCount;
        private float m_TitleBarWidth;
        private static List<ContainerWindow> s_AllWindows = new List<ContainerWindow>();
        private static Vector2 s_LastDragMousePos;
        public ContainerWindow()
        {
            base.hideFlags = HideFlags.DontSave;
            this.m_PixelRect = new Rect(0f, 0f, 400f, 300f);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetAlpha(float alpha);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetInvisible();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern bool IsZoomed();
        private void Internal_SetMinMaxSizes(Vector2 minSize, Vector2 maxSize)
        {
            INTERNAL_CALL_Internal_SetMinMaxSizes(this, ref minSize, ref maxSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_SetMinMaxSizes(ContainerWindow self, ref Vector2 minSize, ref Vector2 maxSize);
        private void Internal_Show(Rect r, int showMode, Vector2 minSize, Vector2 maxSize)
        {
            INTERNAL_CALL_Internal_Show(this, ref r, showMode, ref minSize, ref maxSize);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_Show(ContainerWindow self, ref Rect r, int showMode, ref Vector2 minSize, ref Vector2 maxSize);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_BringLiveAfterCreation(bool displayImmediately, bool setFocus);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetFreezeDisplay(bool freeze);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void DisplayAllViews();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Minimize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ToggleMaximize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void MoveInFrontOf(ContainerWindow other);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void MoveBehindOf(ContainerWindow other);
        public bool maximized { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void InternalClose();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void OnDestroy();
        public Rect position
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_position(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_position(ref value);
            }
        }
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_position(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_position(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_SetTitle(string title);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void GetOrderedWindowList();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_GetTopleftScreenPosition(out Vector2 pos);
        internal Rect FitWindowRectToScreen(Rect r, bool forceCompletelyVisible, bool useMouseScreen)
        {
            Rect rect;
            INTERNAL_CALL_FitWindowRectToScreen(this, ref r, forceCompletelyVisible, useMouseScreen, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_FitWindowRectToScreen(ContainerWindow self, ref Rect r, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);
        internal static Rect FitRectToScreen(Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen)
        {
            Rect rect;
            INTERNAL_CALL_FitRectToScreen(ref defaultRect, forceCompletelyVisible, useMouseScreen, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_FitRectToScreen(ref Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen, out Rect value);
        private IEnumerable<SnapEdge> edges
        {
            get
            {
                if (this.m_EdgesCache == null)
                {
                    this.m_EdgesCache = new SnapEdge[] { this.m_Left, this.m_Right, this.m_Top, this.m_Bottom };
                }
                return this.m_EdgesCache;
            }
        }
        internal static bool macEditor
        {
            get
            {
                return (Application.platform == RuntimePlatform.OSXEditor);
            }
        }
        internal ShowMode showMode
        {
            get
            {
                return (ShowMode) this.m_ShowMode;
            }
        }
        internal void ShowPopup()
        {
            this.m_ShowMode = 1;
            this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
            if (this.m_MainView != null)
            {
                this.m_MainView.SetWindowRecurse(this);
            }
            this.Internal_SetTitle(this.m_Title);
            this.Save();
            this.Internal_BringLiveAfterCreation(false, false);
        }

        public void Show(ShowMode showMode, bool loadPosition, bool displayImmediately)
        {
            if (showMode == ShowMode.AuxWindow)
            {
                showMode = ShowMode.Utility;
            }
            if ((showMode == ShowMode.Utility) || (showMode == ShowMode.PopupMenu))
            {
                this.m_DontSaveToLayout = true;
            }
            this.m_ShowMode = (int) showMode;
            if (showMode != ShowMode.PopupMenu)
            {
                this.Load(loadPosition);
            }
            this.Internal_Show(this.m_PixelRect, this.m_ShowMode, this.m_MinSize, this.m_MaxSize);
            if (this.m_MainView != null)
            {
                this.m_MainView.SetWindowRecurse(this);
            }
            this.Internal_SetTitle(this.m_Title);
            this.Internal_BringLiveAfterCreation(displayImmediately, true);
            if (this != null)
            {
                this.position = this.FitWindowRectToScreen(this.m_PixelRect, true, false);
                this.mainView.position = new Rect(0f, 0f, this.m_PixelRect.width, this.m_PixelRect.height);
                this.mainView.Reflow();
                this.Save();
            }
        }

        public void OnEnable()
        {
            if (this.m_MainView != null)
            {
                this.m_MainView.Initialize(this);
            }
        }

        public void SetMinMaxSizes(Vector2 min, Vector2 max)
        {
            this.m_MinSize = min;
            this.m_MaxSize = max;
            Rect position = this.position;
            Rect rect2 = position;
            rect2.width = Mathf.Clamp(position.width, min.x, max.x);
            rect2.height = Mathf.Clamp(position.height, min.y, max.y);
            if ((rect2.width != position.width) || (rect2.height != position.height))
            {
                this.position = rect2;
            }
            this.Internal_SetMinMaxSizes(min, max);
        }

        internal void InternalCloseWindow()
        {
            this.Save();
            if (this.m_MainView != null)
            {
                if (this.m_MainView is GUIView)
                {
                    ((GUIView) this.m_MainView).RemoveFromAuxWindowList();
                }
                Object.DestroyImmediate(this.m_MainView, true);
                this.m_MainView = null;
            }
            Object.DestroyImmediate(this, true);
        }

        public void Close()
        {
            this.Save();
            this.InternalClose();
            Object.DestroyImmediate(this, true);
        }

        internal bool IsNotDocked()
        {
            return (((this.m_ShowMode == 2) || (this.m_ShowMode == 5)) || ((((this.mainView is SplitView) && (this.mainView.children.Length == 1)) && ((this.mainView.children.Length == 1) && (this.mainView.children[0] is DockArea))) && (((DockArea) this.mainView.children[0]).m_Panes.Count == 1)));
        }

        private string NotDockedWindowID()
        {
            if (!this.IsNotDocked())
            {
                return null;
            }
            HostView mainView = this.mainView as HostView;
            if (mainView == null)
            {
                if (!(this.mainView is SplitView))
                {
                    return this.mainView.GetType().ToString();
                }
                mainView = (HostView) this.mainView.children[0];
            }
            return (((this.m_ShowMode != 2) && (this.m_ShowMode != 5)) ? ((DockArea) this.mainView.children[0]).m_Panes[0].GetType().ToString() : mainView.actualView.GetType().ToString());
        }

        public void Save()
        {
            if (((this.m_ShowMode != 4) && this.IsNotDocked()) && !this.IsZoomed())
            {
                string str = this.NotDockedWindowID();
                EditorPrefs.SetFloat(str + "x", this.m_PixelRect.x);
                EditorPrefs.SetFloat(str + "y", this.m_PixelRect.y);
                EditorPrefs.SetFloat(str + "w", this.m_PixelRect.width);
                EditorPrefs.SetFloat(str + "h", this.m_PixelRect.height);
            }
        }

        private void Load(bool loadPosition)
        {
            if ((this.m_ShowMode != 4) && this.IsNotDocked())
            {
                string str = this.NotDockedWindowID();
                Rect pixelRect = this.m_PixelRect;
                if (loadPosition)
                {
                    pixelRect.x = EditorPrefs.GetFloat(str + "x", this.m_PixelRect.x);
                    pixelRect.y = EditorPrefs.GetFloat(str + "y", this.m_PixelRect.y);
                }
                pixelRect.width = Mathf.Max(EditorPrefs.GetFloat(str + "w", this.m_PixelRect.width), this.m_MinSize.x);
                pixelRect.width = Mathf.Min(pixelRect.width, this.m_MaxSize.x);
                pixelRect.height = Mathf.Max(EditorPrefs.GetFloat(str + "h", this.m_PixelRect.height), this.m_MinSize.y);
                pixelRect.height = Mathf.Min(pixelRect.height, this.m_MaxSize.y);
                this.m_PixelRect = pixelRect;
            }
        }

        internal void OnResize()
        {
            if (this.mainView != null)
            {
                this.mainView.position = new Rect(0f, 0f, this.position.width, this.position.height);
                this.mainView.Reflow();
                this.Save();
            }
        }

        public string title
        {
            get
            {
                return this.m_Title;
            }
            set
            {
                this.m_Title = value;
                this.Internal_SetTitle(value);
            }
        }
        public static ContainerWindow[] windows
        {
            get
            {
                s_AllWindows.Clear();
                GetOrderedWindowList();
                return s_AllWindows.ToArray();
            }
        }
        internal void AddToWindowList()
        {
            s_AllWindows.Add(this);
        }

        public Vector2 WindowToScreenPoint(Vector2 windowPoint)
        {
            Vector2 vector;
            this.Internal_GetTopleftScreenPosition(out vector);
            return (windowPoint + vector);
        }

        public View mainView
        {
            get
            {
                return this.m_MainView;
            }
            set
            {
                this.m_MainView = value;
                this.m_MainView.SetWindowRecurse(this);
                this.m_MainView.position = new Rect(0f, 0f, this.position.width, this.position.height);
                this.m_MinSize = value.minSize;
                this.m_MaxSize = value.maxSize;
            }
        }
        internal string DebugHierarchy()
        {
            return this.mainView.DebugHierarchy(0);
        }

        internal Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize, PopupLocationHelper.PopupLocation[] locationPriorityOrder)
        {
            return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, this, locationPriorityOrder);
        }

        internal Rect GetDropDownRect(Rect buttonRect, Vector2 minSize, Vector2 maxSize)
        {
            return PopupLocationHelper.GetDropDownRect(buttonRect, minSize, maxSize, this);
        }

        internal Rect FitPopupWindowRectToScreen(Rect rect, float minimumHeight)
        {
            float num2 = 0f;
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                num2 = 10f;
            }
            float b = minimumHeight + num2;
            Rect r = rect;
            r.height = Mathf.Min(r.height, 900f);
            r.height += num2;
            r = this.FitWindowRectToScreen(r, true, true);
            float num4 = Mathf.Max(r.yMax - rect.y, b);
            r.y = r.yMax - num4;
            r.height = num4 - num2;
            return r;
        }

        public void HandleWindowDecorationEnd(Rect windowPosition)
        {
        }

        public void HandleWindowDecorationStart(Rect windowPosition)
        {
            if (((windowPosition.y == 0f) && (this.showMode != ShowMode.Utility)) && (this.showMode != ShowMode.PopupMenu))
            {
                if (Mathf.Abs((float) (windowPosition.xMax - this.position.width)) < 2f)
                {
                    GUIStyle buttonClose = Styles.buttonClose;
                    GUIStyle buttonMin = Styles.buttonMin;
                    GUIStyle buttonMax = Styles.buttonMax;
                    if (macEditor && ((GUIView.focusedView == null) || (GUIView.focusedView.window != this)))
                    {
                        buttonClose = buttonMin = buttonMax = Styles.buttonInactive;
                    }
                    this.BeginTitleBarButtons(windowPosition);
                    if (this.TitleBarButton(buttonClose))
                    {
                        this.Close();
                    }
                    if (macEditor && this.TitleBarButton(buttonMin))
                    {
                        this.Minimize();
                        GUIUtility.ExitGUI();
                    }
                    if (this.TitleBarButton(buttonMax))
                    {
                        this.ToggleMaximize();
                    }
                }
                this.HandleTitleBarDrag();
            }
        }

        private void BeginTitleBarButtons(Rect windowPosition)
        {
            this.m_ButtonCount = 0;
            this.m_TitleBarWidth = windowPosition.width;
        }

        private bool TitleBarButton(GUIStyle style)
        {
            Rect position = new Rect((this.m_TitleBarWidth - (13f * ++this.m_ButtonCount)) - 4f, 0f, 13f, 13f);
            return GUI.Button(position, GUIContent.none, style);
        }

        private void SetupWindowEdges()
        {
            Rect position = this.position;
            if (this.m_Left == null)
            {
                this.m_Left = new SnapEdge(this, SnapEdge.EdgeDir.Left, position.xMin, position.yMin, position.yMax);
                this.m_Right = new SnapEdge(this, SnapEdge.EdgeDir.Right, position.xMax, position.yMin, position.yMax);
                this.m_Top = new SnapEdge(this, SnapEdge.EdgeDir.Up, position.yMin, position.xMin, position.xMax);
                this.m_Bottom = new SnapEdge(this, SnapEdge.EdgeDir.Down, position.yMax, position.xMin, position.xMax);
            }
            this.m_Left.pos = position.xMin;
            this.m_Left.start = position.yMin;
            this.m_Left.end = position.yMax;
            this.m_Right.pos = position.xMax;
            this.m_Right.start = position.yMin;
            this.m_Right.end = position.yMax;
            this.m_Top.pos = position.yMin;
            this.m_Top.start = position.xMin;
            this.m_Top.end = position.xMax;
            this.m_Bottom.pos = position.yMax;
            this.m_Bottom.start = position.xMin;
            this.m_Bottom.end = position.xMax;
        }

        private void HandleTitleBarDrag()
        {
            this.SetupWindowEdges();
            EditorGUI.BeginChangeCheck();
            this.DragTitleBar(new Rect(0f, 0f, this.position.width, 24f));
            if (EditorGUI.EndChangeCheck())
            {
                Rect r = new Rect(this.m_Left.pos, this.m_Top.pos, this.m_Right.pos - this.m_Left.pos, this.m_Bottom.pos - this.m_Top.pos);
                if (macEditor)
                {
                    r = this.FitWindowRectToScreen(r, false, false);
                }
                this.position = r;
            }
        }

        private void DragTitleBar(Rect titleBarRect)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if ((titleBarRect.Contains(current.mousePosition) && (GUIUtility.hotControl == 0)) && (current.button == 0))
                    {
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                        s_LastDragMousePos = GUIUtility.GUIToScreenPoint(current.mousePosition);
                        IEnumerator<SnapEdge> enumerator = this.edges.GetEnumerator();
                        try
                        {
                            while (enumerator.MoveNext())
                            {
                                SnapEdge edge = enumerator.Current;
                                edge.startDragPos = edge.pos;
                                edge.startDragStart = edge.start;
                            }
                        }
                        finally
                        {
                            if (enumerator == null)
                            {
                            }
                            enumerator.Dispose();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        Vector2 vector = GUIUtility.GUIToScreenPoint(current.mousePosition);
                        Vector2 offset = vector - s_LastDragMousePos;
                        s_LastDragMousePos = vector;
                        GUI.changed = true;
                        IEnumerator<SnapEdge> enumerator2 = this.edges.GetEnumerator();
                        try
                        {
                            while (enumerator2.MoveNext())
                            {
                                enumerator2.Current.ApplyOffset(offset, true);
                            }
                        }
                        finally
                        {
                            if (enumerator2 == null)
                            {
                            }
                            enumerator2.Dispose();
                        }
                    }
                    break;

                case EventType.Repaint:
                    EditorGUIUtility.AddCursorRect(titleBarRect, MouseCursor.Arrow);
                    break;
            }
        }
        private static class Styles
        {
            public static GUIStyle buttonClose = (!ContainerWindow.macEditor ? "WinBtnClose" : "WinBtnCloseMac");
            public static GUIStyle buttonInactive = "WinBtnInactiveMac";
            public static GUIStyle buttonMax = (!ContainerWindow.macEditor ? "WinBtnMax" : "WinBtnMaxMac");
            public static GUIStyle buttonMin = (!ContainerWindow.macEditor ? "WinBtnClose" : "WinBtnMinMac");
        }
    }
}

